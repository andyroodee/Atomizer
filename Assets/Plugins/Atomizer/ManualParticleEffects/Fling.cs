using UnityEngine;
using System.Collections;

public class Fling : AtomizerEffect
{
    public override IEnumerator SetParticles(ParticleSystem.Particle[] particles)
    {
        IsReady = true;
        yield break;
    }

    public override void Update(ParticleSystem.Particle[] particles, float activeTime)
    {
        
    }
}
