using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public enum PercolationDirection
{
	Up,
	Right,
	Down,
	Left
}

public enum PercolationMethod
{
    Orderly,
    Rotating,
    Random
}

/** 
 * This class demonstrates one simple way to manually animate a particle system.
 * It attempts to "percolate" the particles up in a cardinal direction, starting at the
 * max coordinate particle and moving in the opposite direction.
 **/
public class Percolate : AtomizerEffect 
{			
	public float PercolationTime
	{ get; set; }
	
	public float PercolationSpeed
	{ get; set; }

	public float Duration
	{ get; set; }

	public PercolationDirection Direction
	{ get; set; }

    public PercolationMethod Method
    { get; set; }
		
	private float maxVal;
	private float minVal;
	private float currentVal;
		
	public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{
		maxVal = 0.0f;
		minVal = 0.0f;
		currentVal = 0.0f;

		switch (Direction)
		{
		case PercolationDirection.Up:
			if (particles.Length > 0)
			{
				maxVal = particles.Max(arg => arg.position.y);
				currentVal = maxVal;
				yield return null;
				minVal = particles.Min(arg => arg.position.y);
			}
			break;
		case PercolationDirection.Right:
			if (particles.Length > 0)
			{
				maxVal = particles.Max(arg => arg.position.x);
				currentVal = maxVal;
				yield return null;
				minVal = particles.Min(arg => arg.position.x);
			}
			break;
		case PercolationDirection.Down:
			if (particles.Length > 0)
			{
				maxVal = particles.Min(arg => arg.position.y);
				currentVal = maxVal;
				yield return null;
				minVal = particles.Max(arg => arg.position.y);
			}
			break;
		case PercolationDirection.Left:
			if (particles.Length > 0)
			{
				maxVal = particles.Min(arg => arg.position.x);
				currentVal = maxVal;
				yield return null;
				minVal = particles.Max(arg => arg.position.x);
			}
			break;
		}
		IsReady = true;
		yield break;
	}

	private bool ShouldPercolate(ParticleSystem.Particle particle)
	{
		switch (Direction)
		{
		case PercolationDirection.Up:
			return particle.position.y >= currentVal;
		case PercolationDirection.Down:
			return particle.position.y <= currentVal;
		case PercolationDirection.Right:
			return particle.position.x >= currentVal;
		case PercolationDirection.Left:
			return particle.position.x <= currentVal;
		default: 
			return false;
		}
	}

	private Vector3 PercolateParticle(ParticleSystem.Particle particle, float amount)
	{		
		Vector3 newPosition = particle.position;
		switch (Direction)
		{
		case PercolationDirection.Up:
                if (Method == PercolationMethod.Orderly)
                {
                    newPosition.y += amount;
                }
                else
                {
                    newPosition.y += Random.Range(0.0f, amount) * 5.0f;
                    newPosition.x += Random.Range(-amount, amount) * 5.0f;
                }
			break;
		case PercolationDirection.Down:
			newPosition.y -= amount;
			break;
		case PercolationDirection.Right:
			newPosition.x += amount;
			break;
		case PercolationDirection.Left:
			newPosition.x -= amount;
			break;
		}

		return newPosition;
	}
	
	public override void Update(ParticleSystem.Particle[] particles, float activeTime)
	{		
		// We want to move every particle with a coordinate greater than our cutoff
		// a little bit. There's a bit of randomization in there to make things a little more
		// interesting.
		for (int i = particleSkipIndex; i < particles.Length; i += ParticleSkip)
		{
			if (ShouldPercolate(particles[i]))
			{
				particles[i].position = PercolateParticle(particles[i], PercolationSpeed * Time.deltaTime * Random.value);
			}
		}
		
		// t is used to track how far along we should be in the animation. 
		// When t = 1.0, it means that all particles should be moving.
		float t = activeTime / PercolationTime;
		if (t > 1.0f)
		{
			t = 1.0f;
		}
		currentVal = (1.0f - t) * maxVal + t * minVal;
		
		if (activeTime > Duration)
		{
			IsFinished = true;
		}
	}
}
