using UnityEngine;
using System.Collections;

public class Pop : AtomizerEffect
{	
	public float Force
	{ get; set; }

	public float Duration
	{ get; set; }
	
	private void ProcessParticles(ParticleSystem.Particle[] particles, int startIndex, int count)
	{
		Vector3 randomDirection = new Vector3(0.0f, 0.0f, 0.0f);
		for (int i = startIndex; i < count; ++i)
		{
			randomDirection.x = Random.value - 0.5f;
			randomDirection.y = Random.value - 0.5f;
			randomDirection.z = Random.value - 0.5f;
			randomDirections[i] = randomDirection;
		}
	}
	
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		randomDirections = new Vector3[particles.Length];
		
		ProcessParticles(particles, 0, particles.Length / 2);
		yield return null;
		
		ProcessParticles(particles, particles.Length / 2, particles.Length);
		
		IsReady = true;
		yield break;
	}
	
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{					
		if (activeTime > Duration)
		{
			IsFinished = true;
		}

		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{				
			position = particles[i].position;
			position.x += randomDirections[i].x * Time.deltaTime * Force;
			position.y += randomDirections[i].y * Time.deltaTime * Force;
			position.z += randomDirections[i].z * Time.deltaTime * Force;
			particles[i].position = position;
		}
	}

    private Vector3[] randomDirections;
    private Vector3 position;
}
