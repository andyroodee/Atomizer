using UnityEngine;
using System.Collections;

/** In this effect we make particles fade to a certain 
 * colour over time, then out completely.
 */
public class Disintegrate : AtomizerEffect
{
	public float Duration
	{ get; set; }

	public Color FadeColour
	{ get; set; }

	private int[] randomOrdering;

	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
        randomOrdering = ParticleSystemHelper.GenerateRandomOrdering(particles);
		IsReady = true;
		yield break;
	}

	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{
		if (activeTime > Duration)
		{
			IsFinished = true;
		}

		float portion = activeTime / Duration;

		for (int i = particleSkipIndex; i < portion * randomOrdering.Length; i += ParticleSkip)
		{
			if (i < randomOrdering.Length && randomOrdering[i] < particles.Length)
			{
				particles[randomOrdering[i]].color = FadeColour;
			}
		}
	}
}
