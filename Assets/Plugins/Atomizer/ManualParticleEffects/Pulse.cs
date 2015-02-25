using UnityEngine;
using System.Collections;

public class Pulse : AtomizerEffect 
{		
	public float PulseLength
	{ get; set; }
	
	public float PulseStrength
	{ get; set; }

	public float Duration
	{ get; set; }
	
	private Vector3 centroid;
	private Vector3[] particleToCentroid;
	private bool isGrowing;
	private float lastPulseTime;
	private Vector3 position;
	
	private void CalculateVectorsToCentroid(ParticleSystem.Particle[] particles)
	{
		particleToCentroid = new Vector3[particles.Length];
		for (int i = 0; i < particles.Length; ++i)
		{		
			particleToCentroid[i] = particles[i].position - centroid;
		}
	}
	
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		isGrowing = true;
		lastPulseTime = 0.0f;
		centroid = ParticleSystemHelper.GetParticleSystemCentroid(particles);
		yield return null;
		CalculateVectorsToCentroid(particles);
		IsReady = true;
		yield break;
	}
	
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{
		if (activeTime > Duration)
		{
			IsFinished = true;
		}

		// Modulate the particle size based on how far it is from
		// the centroid. 
		// We'll alternate increasing and decreasing the size in phases
		// so that it makes a sort of pulsating effect. 
		lastPulseTime += Time.deltaTime;
		if (lastPulseTime > PulseLength)
		{
			isGrowing = !isGrowing;
			lastPulseTime = 0.0f;
		}

		float scale = PulseStrength * Time.deltaTime;
		
		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{
			position = particles[i].position;
			if (isGrowing)
			{
				particles[i].size += particleToCentroid[i].magnitude * scale;
				position.x += particleToCentroid[i].x * scale;
				position.y += particleToCentroid[i].y * scale;
				particles[i].position = position;
			}
			else
			{
				particles[i].size -= particleToCentroid[i].magnitude * scale;
				position.x -= particleToCentroid[i].x * scale;
				position.y -= particleToCentroid[i].y * scale;
				particles[i].position = position;
			}
		}
	}
}