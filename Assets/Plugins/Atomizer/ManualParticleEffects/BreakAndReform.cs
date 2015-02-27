using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/** 
 * BreakAndReform has two phases. 
 * Phase one scatters the particles.
 * Phase two sets them back to their original positions.
 **/
public class BreakAndReform : AtomizerEffect
{				
	public float BreakApartSpeed
	{ get; set; }
	
	public float BreakPhaseDuration
	{ get; set; }
	
	public float ReformPhaseDuration
	{ get; set; }
			
	private void ProcessParticles(ParticleSystem.Particle[] particles, int startIndex, int count)
	{
		Vector3 randomDirection = new Vector3(0.0f, 0.0f, 0.0f);
		for (int i = startIndex; i < count; ++i)
		{
			originalPositions[i] = particles[i].position;
			randomDirection.x = Random.value - 0.5f;
			randomDirection.y = Random.value - 0.5f;
			randomDirection.z = Random.value - 0.5f;
			randomDirections[i] = randomDirection;
		}
	}
	
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		endPositionsStored = false;
		// Store the original particle positions so that we can go back to them.
		// We'll also generate some random directions to send each particle along.
		originalPositions = new Vector3[particles.Length];
		randomDirections = new Vector3[particles.Length];
		endPositions = new Vector3[particles.Length];
		
		ProcessParticles(particles, 0, particles.Length / 2);
		yield return null;
		
		ProcessParticles(particles, particles.Length / 2, particles.Length);
		
		IsReady = true;
		yield break;
	}		

	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{				
		if (activeTime <= BreakPhaseDuration)
		{
			// Breaking phase. Fly out in a random direction.
			for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
			{				
				position = particles[i].position;
				position.x += randomDirections[i].x * Time.deltaTime * BreakApartSpeed;
				position.y += randomDirections[i].y * Time.deltaTime * BreakApartSpeed;
				position.z += randomDirections[i].z * Time.deltaTime * BreakApartSpeed;
				particles[i].position = position;
			}
		}
		else
		{
			// Time to reform!
			if (!endPositionsStored)
			{
				// Store the end positions the particles ended up.
				endPositionsStored = true;
				for (int i = 0; i < particles.Length; ++i)
				{				
					endPositions[i] = particles[i].position;
				}
			}
			
			// Now move the particles back from their end position to their original positions.
			float t = (activeTime - BreakPhaseDuration) / ReformPhaseDuration;
			for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
			{
				particles[i].position = Vector3.Lerp(endPositions[i], originalPositions[i], t);
			}
			if (t >= 1.0f)
			{
				IsFinished = true;
			}
		}
	}

    private Vector3[] endPositions;
    private Vector3[] originalPositions;
    private Vector3[] randomDirections;
    private bool endPositionsStored;
    private Vector3 position;
}
