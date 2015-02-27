using UnityEngine;
using System.Collections;

public class Jiggle : AtomizerEffect
{
	public float Noise
	{ get; set; }

	public float Duration
	{ get; set; }
    
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{					
		if (activeTime > Duration)
		{
			IsFinished = true;
		}
				
		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{				
			position = particles[i].position;
			position.x += ((0.5f - Random.value) * Noise * Time.deltaTime);
			position.y += ((0.5f - Random.value) * Noise * Time.deltaTime);
			position.z += ((0.5f - Random.value) * Noise * Time.deltaTime);
			particles[i].position = position;
		}
	}

    private Vector3 position;
}
