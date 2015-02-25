using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class AtomizeOnClick : MonoBehaviour 
{	
	private readonly int MaxEffects = 10;

	[SerializeField] private Vector2 uiBasePoint;
	private string uiText;
	private bool canAtomize;

	void Start()
	{
		canAtomize = true;
	}

	void OnFinish()
	{
        StartCoroutine(SetVisible(true));
		canAtomize = true;
	}
	
	private AtomizerEffectGroup GetRandomEffect()
	{
		AtomizerEffectGroup effectGroup = Atomizer.CreateEffectGroup();
		
		bool combine = (Random.value > 0.5f);
		int numberOfEffects = Random.Range(1, 4);
		uiText = numberOfEffects + " Effect(s):\n";
		for (int i = 0; i < numberOfEffects; ++i)
		{
			int selection = Random.Range(0, MaxEffects);
			switch (selection)
			{
				case 0: 
				{
					Disintegrate effect = new Disintegrate();
					effect.Duration = Random.Range(1.0f, 5.0f);
					effect.FadeColour = new Color(Random.value, Random.value, Random.value, Random.value);
					if (combine)
					{
						uiText += "\n\nCombined Effect: Disintegrate\nDuration: " + effect.Duration + "\nFadeColour: " + effect.FadeColour;
						effectGroup.Combine(effect);
					}
					else
					{
						uiText += "\n\nChained Effect: Disintegrate\nDuration: " + effect.Duration + "\nFadeColour: " + effect.FadeColour;
						effectGroup.Chain(effect);
					}
				}
				break;
				case 1: 
				{
					Percolate effect = new Percolate();
					effect.PercolationTime = Random.Range(1.0f, 4.0f);
					effect.PercolationSpeed = Random.Range(0.5f, 6.0f);
					effect.Duration = 2.0f * effect.PercolationTime;
					int directionIndex = Random.Range(0, System.Enum.GetValues(typeof(PercolationDirection)).Length);
					effect.Direction = (PercolationDirection)directionIndex;
					if (combine)
					{
						uiText += "\n\nCombined Effect: ParticlePercolator\nPercTime: " + effect.PercolationTime +
							"\nPercSpeed: " + effect.PercolationSpeed + "\nDuration: " + effect.Duration +
							"\nDirection: " + effect.Direction;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: ParticlePercolator\nPercTime: " + effect.PercolationTime +
							"\nPercSpeed: " + effect.PercolationSpeed + "\nDuration: " + effect.Duration +
							"\nDirection: " + effect.Direction;
						effectGroup.Chain(effect);
					}
				}
				break;
	            case 2: 
				{
					BreakAndReform effect = new BreakAndReform();
					effect.BreakApartSpeed = Random.Range(1.0f, 8.0f);
					effect.BreakPhaseDuration = Random.Range(1.0f, 3.0f);
					effect.ReformPhaseDuration = Random.Range(1.0f, 3.0f);
					if (combine)
					{
						uiText += "\n\nCombined Effect: BreakAndReform\nBreak Speed: " + effect.BreakApartSpeed + 
							"\nBreak Phase Duration: " + effect.BreakPhaseDuration +
							"\nReform Phase Duration: " + effect.ReformPhaseDuration;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: BreakAndReform\nBreak Speed: " + effect.BreakApartSpeed + 
							"\nBreak Phase Duration: " + effect.BreakPhaseDuration +
							"\nReform Phase Duration: " + effect.ReformPhaseDuration;
						effectGroup.Chain(effect);
					}
				}
				break;
            	case 3: 
				{
					SwapColours effect = new SwapColours();
					int modeIndex = Random.Range(0, System.Enum.GetValues(typeof(ColourSwapMode)).Length);
					effect.ColourSwapMode = (ColourSwapMode)modeIndex;
					effect.Duration = Random.Range(1.0f, 4.0f);
					if (combine)
					{
						uiText += "\n\nCombined Effect: SwapColours\nColour Swap Mode: " + effect.ColourSwapMode +
							"\nDuration: " + effect.Duration;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: SwapColours\nColour Swap Mode: " + effect.ColourSwapMode +
							"\nDuration: " + effect.Duration;
						effectGroup.Chain(effect);
					}
				}
				break;
            	case 4: 
				{ 
					Pulse effect = new Pulse();
					effect.PulseLength = Random.Range(0.1f, 0.6f);
					effect.PulseStrength = Random.Range(0.2f, 0.8f);
					effect.Duration = 4.0f * effect.PulseLength;
					if (combine)
					{
						uiText += "\n\nCombined Effect: Pulse\nPulse Length: " + effect.PulseLength +
							"\nStrength: " + effect.PulseStrength +
							"\nDuration: " + effect.Duration;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: Pulse\nPulse Length: " + effect.PulseLength +
							"\nStrength: " + effect.PulseStrength +
							"\nDuration: " + effect.Duration;
						effectGroup.Chain(effect);
					}
				}
				break;
            	case 5: 
				{
					Vacuum effect = new Vacuum();
					effect.VacuumPoint = new Vector3(
						transform.position.x + Random.Range(-1.0f, 1.0f),
						transform.position.y + Random.Range(-1.0f, 1.0f),
						transform.position.z + Random.Range(-1.0f, 1.0f));
					effect.MoveSpeed = Random.Range(0.1f, 3.0f);
					effect.Duration = 5.0f;
					if (combine)
					{
						uiText += "\n\nCombined Effect: Vacuum\nPoint: " + effect.VacuumPoint +
							"\nMove Speed: " + effect.MoveSpeed +
							"\nDuration: " + effect.Duration;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: Vacuum\nPoint: " + effect.VacuumPoint +
							"\nMove Speed: " + effect.MoveSpeed +
							"\nDuration: " + effect.Duration;
						effectGroup.Chain(effect);
					}
				}
				break;
           	 	case 6: 
				{
					Warp effect = new Warp();
					effect.WarpTime = Random.Range(1.0f, 2.0f);
					effect.TargetPoint = new Vector3(
						transform.position.x + Random.Range(-1.0f, 1.0f),
						transform.position.y + Random.Range(-1.0f, 1.0f),
						transform.position.z + Random.Range(-1.0f, 1.0f));
					effect.StartScale = Random.Range(0.5f, 3.0f);					
					if (combine)
					{
						uiText += "\n\nCombined Effect: Warp\nTime: " + effect.WarpTime +
							"\nTarget: " + effect.TargetPoint +
							"\nStart Scale: " + effect.StartScale;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: Warp\nTime: " + effect.WarpTime +
							"\nTarget: " + effect.TargetPoint +
							"\nStart Scale: " + effect.StartScale;
						effectGroup.Chain(effect);
					}
				}
				break;
            	case 7:
				{
					Smolder effect = new Smolder();
					effect.SmolderColour = new Color(Random.value, Random.value, Random.value, Random.value);
					effect.BurntColour = new Color(Random.value, Random.value, Random.value, Random.value);
					effect.EndColour = Color.clear;
					effect.SmolderDuration = Random.Range(0.5f, 2.5f);
					effect.BurntDuration = Random.Range(0.5f, 2.5f);
					effect.Smoothness = Random.Range(0.1f, 0.9f);
					effect.DispersionForce = Random.Range(0.0f, 4.0f);
					effect.SmolderSpeed = Random.Range(0.1f, 2.0f);
					effect.Source = Input.mousePosition;					
					if (combine)
					{
						uiText += "\n\nCombined Effect: Smolder" +
							"\nSmolderColour : " + effect.SmolderColour +
							"\nBurntColour : " + effect.BurntColour +
							"\nEndColour : " + effect.EndColour +
							"\nSmolderDuration : " + effect.SmolderDuration +
							"\nBurntDuration : " + effect.BurntDuration +
							"\nSmoothness : " + effect.Smoothness +
							"\nDispersionForce : " + effect.DispersionForce +					
							"\nSmolderSpeed : " + effect.SmolderSpeed;
						effectGroup.Combine(effect);						
					}
					else
					{
						uiText += "\n\nChained Effect: Smolder" +
						"\nSmolderColour : " + effect.SmolderColour +
						"\nBurntColour : " + effect.BurntColour +
						"\nEndColour : " + effect.EndColour +
						"\nSmolderDuration : " + effect.SmolderDuration +
						"\nBurntDuration : " + effect.BurntDuration +
						"\nSmoothness : " + effect.Smoothness +
						"\nDispersionForce : " + effect.DispersionForce +					
						"\nSmolderSpeed : " + effect.SmolderSpeed;
						effectGroup.Chain(effect);
					}
				}
				break;
				case 8: 
				{
					Jiggle effect = new Jiggle();
					effect.Noise = Random.Range(0.1f, 1.0f);
					effect.Duration = Random.Range(0.5f, 3.0f);					
					if (combine)
					{
						uiText += "\n\nCombined Effect: Jiggle\nNoise: " + effect.Noise +
							"\nDuration: " + effect.Duration;
						effectGroup.Combine(effect);
					}
					else
					{
						uiText += "\n\nChained Effect: Jiggle\nNoise: " + effect.Noise +
							"\nDuration: " + effect.Duration;
						effectGroup.Chain(effect);
					}
				}			
				break;
				case 9:
				{
					Pop effect = new Pop();
					effect.Duration = Random.Range(1.0f, 3.0f);
					effect.Force = Random.Range(0.1f, 4.0f);
					if (combine)
					{
						uiText += "\n\nCombined Effect: Pop\nForce: " + effect.Force +
							"\nDuration: " + effect.Duration;
						effectGroup.Combine(effect);
					}
					else
					{
						uiText += "\n\nChained Effect: Pop\nForce: " + effect.Force +
							"\nDuration: " + effect.Duration;
						effectGroup.Chain(effect);
					}
				}
				break;
			}
		}

		return effectGroup;
	}

	public void Play()
	{
        uiText = "";
		AtomizerEffectGroup effectGroup = GetRandomEffect();
		Atomizer.Atomize(gameObject, effectGroup, OnFinish);
        StartCoroutine(SetVisible(false));
	}

    private IEnumerator SetVisible(bool visible)
    {
        yield return new WaitForEndOfFrame();
        List<Renderer> renderers = gameObject.GetAllComponents<Renderer>();
        if (renderers != null)
        {
            renderers.ForEach(item => item.enabled = visible);
        }
        Image img = GetComponentInChildren<Image>();
        if (img != null)
        {
            GetComponentInChildren<Image>().enabled = visible;
        }
    }

	void OnGUI()
	{
		GUI.Label(new Rect(uiBasePoint.x, uiBasePoint.y, 200, 600), uiText);
	}

	void OnMouseUpAsButton()
	{
		if (canAtomize)
		{
			canAtomize = false;
			Play();
		}
	}
}
