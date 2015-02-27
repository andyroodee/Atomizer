using UnityEngine;
using System.Collections;
using System.Linq;

public class Smolder : AtomizerEffect
{
	private enum SmolderState
	{
		Untouched,
		Smoldering,
		Burning,
		Burnt
	}

	public Color32 SmolderColour
	{ get; set; }

	public Color32 BurntColour
	{ get; set;	}

	public Color32 EndColour
	{ get; set;	}

	public float SmolderDuration
	{ get; set; }

	public float BurntDuration
	{ get; set; }

	public float Smoothness
	{ get; set; }

	public float DispersionForce
	{ get; set; }

	public float SmolderSpeed
	{ get; set; }

	public Vector3 Source
	{ get; set; }
    
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		smolderDistance = 0.0f;
		burntCount = 0;
		smolderDurations = new float[particles.Length];
		burnDurations = new float[particles.Length];
		smolderStates = new SmolderState[particles.Length];
		for (int i = 0; i < particles.Length; ++i)
		{
			smolderDurations[i] = 0.0f;
			burnDurations[i] = 0.0f;
			smolderStates[i] = SmolderState.Untouched;
		}

		// Find the closest particle to the Source
		float minDistance = float.MaxValue;
		foreach (ParticleSystem.Particle p in particles)
		{
			float dist = Vector2.Distance(Camera.main.WorldToScreenPoint(p.position), Source);
			if (dist < minDistance)
			{
				minDistance = dist;
				sourceParticle = p;
			}
		}
		Smoothness = Mathf.Clamp(Smoothness, 0.01f, 0.95f);
		IsReady = true;
		yield break;
	}
	
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{
		if (burntCount >= particles.Length)
		{
			IsFinished = true;
		}
		else
		{
			smolderDistance += SmolderSpeed * Time.deltaTime;

			for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
			{
				if (smolderStates[i] == SmolderState.Untouched)
				{
					if (Random.value > Smoothness &&
					    Vector3.Distance(particles[i].position, sourceParticle.position) < smolderDistance)
					{
						smolderStates[i] = SmolderState.Smoldering;
						particles[i].color = SmolderColour;
					}
				}
				else if (smolderStates[i] == SmolderState.Smoldering)
				{
					smolderDurations[i] += Time.deltaTime;
					particles[i].color = Color.Lerp(SmolderColour, BurntColour, smolderDurations[i] / SmolderDuration);
					if (smolderDurations[i] > SmolderDuration)
					{
						smolderStates[i] = SmolderState.Burning;
						particles[i].color = BurntColour;
					} 
				} 
				else if (smolderStates[i] == SmolderState.Burning)
				{
					burnDurations[i] += Time.deltaTime;
					particles[i].color = Color.Lerp(BurntColour, EndColour, burnDurations[i] / BurntDuration);
					if (burnDurations[i] > BurntDuration)
					{
						smolderStates[i] = SmolderState.Burnt;
						particles[i].color = EndColour;
						++burntCount;
					} 
					if (Random.value > Smoothness)
					{
						Vector3 position = particles[i].position;
						Vector3 dir = position - sourceParticle.position;
						particles[i].position += dir.normalized * Time.deltaTime * DispersionForce;
					}
				}
			}
		}
	}

    private ParticleSystem.Particle sourceParticle;
    private int burntCount;
    private float[] smolderDurations;
    private float[] burnDurations;
    private SmolderState[] smolderStates;
    private float smolderDistance;
}
