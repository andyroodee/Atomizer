using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Warp : AtomizerEffect 
{			
	public float WarpTime
	{ get; set; }

	public Vector3 TargetPoint
	{ get; set; }

	public float StartScale
	{ get; set; }
		
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		centroid = ParticleSystemHelper.GetParticleSystemCentroid(particles);

		targetPositions = new Vector3[particles.Length];
		originalPositions = new Vector3[particles.Length];
		for (int i = 0; i < particles.Length; ++i)
		{
			targetPositions[i] = (particles[i].position - centroid) + TargetPoint;
			originalPositions[i] = particles[i].position;
		}

		if (particles.Length > 0)
		{
			originalSize = particles[0].size;
		}
		else
		{
			originalSize = 1.0f;
		}
		IsReady = true;
		yield break;
	}	

	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{
		float t = activeTime / WarpTime;
		if (t > 1.0f)
		{
			t = 1.0f;
			IsFinished = true;
		}

		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{
			if (Random.value < t)
			{						
				position = particles[i].position;
				position = Vector3.Lerp(originalPositions[i], targetPositions[i], t + Random.value);
				particles[i].position = position;

				float sizeLerp = Vector3.Distance(particles[i].position, targetPositions[i]) / 
					Vector3.Distance(originalPositions[i], targetPositions[i]);
				particles[i].size = Mathf.Lerp(StartScale * originalSize, originalSize, 1.0f - sizeLerp);
			}
		}
    }

    private Vector3 position;
    private Vector3[] targetPositions;
    private Vector3[] originalPositions;
    private Vector3 centroid;
    private float originalSize;
}
