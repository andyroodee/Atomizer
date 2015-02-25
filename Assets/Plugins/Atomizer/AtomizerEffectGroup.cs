using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ParticleSystem))]
public class AtomizerEffectGroup : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IPHONE
	public static readonly int DefaultMaxParticleCount = 8192;
	public static readonly float DefaultParticleSize = 0.01f;
#else
	public static readonly float DefaultParticleSize = 0.01f;
	public static readonly int DefaultMaxParticleCount = 32768;
#endif
	public static readonly bool DefaultForceParticleSize = true;
	private static readonly Camera DefaultSceneCamera = Camera.main;

	public float ParticleSize
	{ get; set; }
	
	public float MaxParticleCount
	{ get; set; }
	
	public bool ForceParticleSize
	{ get; set; }
	
	protected int ParticleCount
	{ get; set; }
	
	protected ParticleSystem.Particle[] Particles
	{ get; set; }	

	public Camera SceneCamera
	{ get; set; }

	protected float ActiveTime
	{ get; set; }
	
	protected bool IsPlaying
	{ get; set; }

	protected bool IsFinished
	{ get; set; }

	public Action OnFinish;
    
    private void Awake()
    {
        chainedEffectIndex = 0;
        currentEffectIndex = 0;
        SceneCamera = DefaultSceneCamera;
        ActiveTime = 0.0f;
        IsPlaying = false;
        allEffects = new Dictionary<int, List<AtomizerEffect>>();
    }

	public AtomizerEffectGroup Combine(AtomizerEffect effect)
	{
		if (allEffects.ContainsKey(chainedEffectIndex))
		{
			List<AtomizerEffect> effects = allEffects[chainedEffectIndex];
			effects.Add(effect);
		}
		else
		{
			List<AtomizerEffect> effects = new List<AtomizerEffect>();
			effects.Add(effect);
			allEffects.Add(chainedEffectIndex, effects);
		}
		return this;
	}

	public AtomizerEffectGroup Chain(AtomizerEffect effect)
	{
		if (allEffects.ContainsKey(chainedEffectIndex))
		{
			++chainedEffectIndex;
		}
		return Combine(effect);
	}
	
	public void Play()
	{
		if (allEffects.ContainsKey(currentEffectIndex))
		{
			foreach (AtomizerEffect effect in allEffects[currentEffectIndex])
			{
				effect.IsPlaying = true;
			}
			IsPlaying = true;
		}
	}
	
	private void Update()
	{
		if (IsPlaying)
		{
			ActiveTime += Time.deltaTime;
			
			if (IsFinished)
			{
				if (OnFinish != null)
				{
					OnFinish();
				}
				Destroy(gameObject);
			}
			else
			{
				bool finishedThisCombination = true;
				if (allEffects.ContainsKey(currentEffectIndex))
				{
					foreach (AtomizerEffect effect in allEffects[currentEffectIndex])
					{
						if (!effect.IsFinished)
						{
							finishedThisCombination = false;
							if (effect.IsReady)
							{
								effect.Update(Particles, ActiveTime);
								effect.UpdateParticleSkipIndex();
							}
						}
					}
				}
				if (finishedThisCombination)	
				{
					ActiveTime = 0.0f;
					++currentEffectIndex;
					if (allEffects.ContainsKey(currentEffectIndex))
					{
						SetParticles(Particles);
						Play();
					}
					else
					{
						IsFinished = true;
					}
				}
				if (!IsFinished)
				{
					particleSystem.SetParticles(Particles, ParticleCount);
				}
			}
		}
	}

    private void BuildCanvasEffect(Snapshot snapshot, Camera atomizerCamera, GameObject atomizerTarget)
    {
        if (MaxParticleCount < 1)
        {
            MaxParticleCount = 1;
        }
                
        Color32[] validPixels = GetValidPixels(snapshot.Image);

        int imageResizeRatio = Mathf.RoundToInt(Mathf.Pow(2.0f, mipLevel));
        if (imageResizeRatio < 1)
        {
            imageResizeRatio = 1;
        }

        int imageHeight = snapshot.Image.height / imageResizeRatio;
        int imageWidth = snapshot.Image.width / imageResizeRatio;

        Color32[] colourData = snapshot.ColourData.GetPixels32();
        int vScale = Mathf.RoundToInt((float)snapshot.Image.width / snapshot.ColourData.width);
        int hScale = Mathf.RoundToInt((float)snapshot.Image.height / snapshot.ColourData.height);
        vScale = Mathf.Clamp(vScale, 1, int.MaxValue);
        hScale = Mathf.Clamp(hScale, 1, int.MaxValue);
        int invScale = Mathf.RoundToInt(vScale * hScale);
        int pixelsLength = colourData.Length;
        int colourDataWidth = snapshot.ColourData.width;
        int validPixelCount = 0;

        for (int i = 0; i < colourData.Length; ++i)
        {
            if (colourData[i].a > 0)
            {
                validPixelCount += invScale;
            }
        }

        ParticleSystem.Particle[] particles = EmitParticles(validPixelCount);
        
        float adjustedParticleSize = ParticleSize;
        if (!ForceParticleSize)
        {
            adjustedParticleSize = ParticleSize * imageResizeRatio;
            adjustedParticleSize = Mathf.Clamp(adjustedParticleSize, 0.01f, 1.0f);
        }

        float minDistance = Vector3.Distance(atomizerCamera.transform.position, atomizerTarget.transform.position);
        Color32 color = new Color32(0, 0, 0, 0);
        Vector3 screenPoint = new Vector3(0.0f, 0.0f, minDistance - 1.0f);
        int validPixelIndex = 0;
        int midPoint = colourDataWidth / 2;

        for (int i = 0; i < imageHeight; ++i)
        {
            float screenPointY = snapshot.ScreenRect.yMin + i * imageResizeRatio;
            screenPoint.y = screenPointY;
            for (int j = 0; j < imageWidth; ++j)
            {
                int index = j + i * imageWidth;
                int sourceIndex = j / hScale + (i / vScale) * colourDataWidth;
                if (snapshot.ReflectOnYAxis)
                {
                    int temp = sourceIndex % colourDataWidth;
                    temp -= (2 * (temp - midPoint) + 1);
                    sourceIndex = temp + (i / vScale) * colourDataWidth;
                }
                if (sourceIndex < pixelsLength && colourData[sourceIndex].a > 0 && validPixelIndex < particles.Length)
                {
                    ParticleSystem.Particle particle = particles[validPixelIndex];
                    color = validPixels[index];
                    color.a = 255;
                    particle.color = color;
                    particle.size = adjustedParticleSize;
                    screenPoint.x = snapshot.ScreenRect.xMin + j * imageResizeRatio;
                    particle.position = atomizerCamera.ScreenToWorldPoint(screenPoint);
                    particles[validPixelIndex] = particle;
                    ++validPixelIndex;
                }
            }
        }

        Destroy(snapshot.Image);
        Destroy(snapshot.ColourData);

        SetParticles(particles);
    }

    private void BuildEffect(Snapshot snapshot, Camera atomizerCamera, GameObject atomizerTarget)
    {
        if (MaxParticleCount < 1)
        {
            MaxParticleCount = 1;
        }

        Color32[] validPixels = GetValidPixels(snapshot.Image);

        int imageResizeRatio = Mathf.RoundToInt(Mathf.Pow(2.0f, mipLevel));
        if (imageResizeRatio < 1)
        {
            imageResizeRatio = 1;
        }

        int imageHeight = snapshot.Image.height / imageResizeRatio;
        int imageWidth = snapshot.Image.width / imageResizeRatio;
        int pixelsLength = validPixels.Length;
        int validPixelCount = 0;

        for (int i = 0; i < pixelsLength; ++i)
        {
            if (validPixels[i].a > 0 && validPixels[i].r > 0 &&
                validPixels[i].g > 0 && validPixels[i].b > 0)
            {
                ++validPixelCount;
            }
        }

        ParticleSystem.Particle[] particles = EmitParticles(validPixelCount);

        int validPixelIndex = 0;
        float adjustedParticleSize = ParticleSize;
        if (false == ForceParticleSize)
        {
            adjustedParticleSize = ParticleSize * imageResizeRatio;
            adjustedParticleSize = Mathf.Clamp(adjustedParticleSize, 0.01f, 1.0f);
        }

        float minDistance = Vector3.Distance(atomizerCamera.transform.position, atomizerTarget.transform.position);
        Color32 color = new Color32(0, 0, 0, 0);
        Vector3 screenPoint = new Vector3(0.0f, 0.0f, minDistance - 1.0f);

        for (int i = 0; i < imageHeight; ++i)
        {
            float screenPointY = snapshot.ScreenRect.yMin + i * imageResizeRatio;
            screenPoint.y = screenPointY;
            for (int j = 0; j < imageWidth; ++j)
            {
                int index = j + i * imageWidth;                
                if (index < pixelsLength && validPixels[index].a > 0 && validPixels[index].r > 0 &&
                    validPixels[index].g > 0 && validPixels[index].b > 0)
                {
                    ParticleSystem.Particle particle = particles[validPixelIndex];
                    color = validPixels[index];
                    color.a = 255;
                    particle.color = color;
                    particle.size = adjustedParticleSize;
                    screenPoint.x = snapshot.ScreenRect.xMin + j * imageResizeRatio;
                    particle.position = atomizerCamera.ScreenToWorldPoint(screenPoint);
                    particles[validPixelIndex] = particle;
                    ++validPixelIndex;
                }
            }
        }

        Destroy(snapshot.Image);

        SetParticles(particles);
    }

    private Color32[] GetValidPixels(Texture2D image)
    {
        mipLevel = image.mipmapCount > 0 ? image.mipmapCount - 1 : 0;
        Color32[] pixels = image.GetPixels32(mipLevel);

        Color32[] validPixels = pixels;
        while (pixels.Length < MaxParticleCount && mipLevel > 0)
        {
            --mipLevel;
            validPixels = pixels;
            pixels = image.GetPixels32(mipLevel);
            if (pixels.Length > MaxParticleCount)
            {
                ++mipLevel;
                break;
            }
            if (mipLevel == 0)
            {
                validPixels = pixels;
            }
        }

        return validPixels;
    }

    public void Build(Snapshot snapshot, Camera atomizerCamera, GameObject atomizerTarget)
    {
        if (snapshot.ColourData != null)
        {
            BuildCanvasEffect(snapshot, atomizerCamera, atomizerTarget);
        }
        else
        {
            BuildEffect(snapshot, atomizerCamera, atomizerTarget);
        }
    }
	
	public ParticleSystem.Particle[] EmitParticles(int particleCount)
	{
		ParticleSystem.Particle[] emittedParticles = new ParticleSystem.Particle[particleCount];	
		
		particleSystem.Emit(particleCount);
		particleSystem.GetParticles(emittedParticles);
		
		return emittedParticles;
	}
	
	public void SetParticles(ParticleSystem.Particle[] particles)
	{
		if (!IsFinished)
		{
			Particles = particles;
			ParticleCount = particles.Length;
			particleSystem.SetParticles(Particles, ParticleCount);
			if (allEffects.ContainsKey(currentEffectIndex))
			{
				foreach (AtomizerEffect effect in allEffects[currentEffectIndex])
				{
					StartCoroutine(effect.SetParticles(Particles));
				}
			}
		}
	}
		
	public void Initialize()
	{
		ParticleSize = DefaultParticleSize;
		MaxParticleCount = DefaultMaxParticleCount;
		ForceParticleSize = DefaultForceParticleSize;
		particleSystem.Stop();
		particleSystem.Clear();
		ParticleCount = 0;
	}

    private Dictionary<int, List<AtomizerEffect>> allEffects;
    private int chainedEffectIndex;
    private int currentEffectIndex;
    private int mipLevel;
}