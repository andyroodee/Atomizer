using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Vacuum : AtomizerEffect 
{		
	public Vector3 VacuumPoint
	{ get; set; }
	
	public float MoveSpeed
	{ get; set; }

	public float Duration
	{ get; set; }

	private Vector3 centroid;
	private float[] particleToCentroidDistances;
	private float maxDistanceToCentroid;
	private Vector3 position;
	
	private void CalculateVectorsToCentroid(ParticleSystem.Particle[] particles)
	{
		particleToCentroidDistances = new float[particles.Length];
		for (int i = 0; i < particles.Length; ++i)
		{		
			particleToCentroidDistances[i] = Vector3.Distance(particles[i].position, centroid);
			if (particleToCentroidDistances[i] > maxDistanceToCentroid)
			{
				maxDistanceToCentroid = particleToCentroidDistances[i];
			}
		}
		maxDistanceToCentroid *= 1.1f;
	}
	
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		centroid = ParticleSystemHelper.GetParticleSystemCentroid(particles);
		yield return null;
		CalculateVectorsToCentroid(particles);
		IsReady = true;
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
			position = Vector3.MoveTowards(position, VacuumPoint, 
				(maxDistanceToCentroid - particleToCentroidDistances[i]) * Time.deltaTime * MoveSpeed);
			particles[i].position = position;
		}
	}
}