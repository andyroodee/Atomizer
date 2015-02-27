using UnityEngine;
using System.Collections;

public static class ParticleSystemHelper
{
	public static Vector3 GetParticleSystemCentroid(ParticleSystem.Particle[] particles)
	{		
		Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);

        if (particles.Length == 0)
        {
            return sum;
        }
		
		for (int i = 0; i < particles.Length; ++i)
		{				
			Vector3 currentPosition = particles[i].position;
			sum = sum + currentPosition;
		}
		
		return sum / particles.Length;
	}

    public static int[] GenerateRandomOrdering(ParticleSystem.Particle[] particles)
    {
        int[] randomOrdering = new int[particles.Length];
        if (randomOrdering.Length > 0)
        {
            randomOrdering[0] = 0;
            for (int i = 1; i < randomOrdering.Length; ++i)
            {
                int j = Random.Range(0, i);
                if (j != i)
                {
                    randomOrdering[i] = randomOrdering[j];
                }
                randomOrdering[j] = i;
            }
        }

        return randomOrdering;
    }
}
