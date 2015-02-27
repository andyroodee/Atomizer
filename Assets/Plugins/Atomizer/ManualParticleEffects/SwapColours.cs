using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum ColourSwapMode
{
	RGBtoRBG,
	RGBtoGRB,
	RGBtoGBR,
	RGBtoBRG,
	RGBtoBGR,
	Inverted
}

public class SwapColours : AtomizerEffect 
{
	public ColourSwapMode ColourSwapMode
	{ get; set; }

	public float Duration
	{ get; set; }

	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{				
		// For each particle, figure out the target colour we
		// want to end up as.
		originalColours = new Color32[particles.Length];
		targetColours = new Color32[particles.Length];
		Color32 targetColour = new Color32(0, 0, 0, 255);
		for (int i = 0; i < particles.Length; ++i)
		{
			Color32 oldColour = particles[i].color;
			originalColours[i] = oldColour;
			switch (ColourSwapMode)
			{
			case ColourSwapMode.RGBtoRBG:
				targetColour.r = oldColour.r;
				targetColour.g = oldColour.b;
				targetColour.b = oldColour.g;
				targetColours[i] = targetColour;
				break;
			case ColourSwapMode.RGBtoGRB:
				targetColour.r = oldColour.g;
				targetColour.g = oldColour.r;
				targetColour.b = oldColour.b;
				targetColours[i] = targetColour;
				break;
			case ColourSwapMode.RGBtoGBR:
				targetColour.r = oldColour.g;
				targetColour.g = oldColour.b;
				targetColour.b = oldColour.r;
				targetColours[i] = targetColour;
				break;
			case ColourSwapMode.RGBtoBRG:
				targetColour.r = oldColour.b;
				targetColour.g = oldColour.r;
				targetColour.b = oldColour.g;
				targetColours[i] = targetColour;
				break;
			case ColourSwapMode.RGBtoBGR:
				targetColour.r = oldColour.b;
				targetColour.g = oldColour.g;
				targetColour.b = oldColour.r;
				targetColours[i] = targetColour;
				break;
			case ColourSwapMode.Inverted:
				targetColour.r = (byte)(255 - oldColour.r);
				targetColour.g = (byte)(255 - oldColour.g);
				targetColour.b = (byte)(255 - oldColour.b);
				targetColours[i] = targetColour;
				break;
			}
		}
		IsReady = true;
		yield break;
	}
	
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{					
		if (activeTime > Duration)
		{
			IsFinished = true;
		}

		float t = activeTime / Duration;
		
		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{	
			// Move the colour towards the target.
			particles[i].color = Color32.Lerp(originalColours[i], targetColours[i], t);
		}
	}

    private Color32[] originalColours;
    private Color32[] targetColours;
}
