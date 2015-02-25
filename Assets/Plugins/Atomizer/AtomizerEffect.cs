using UnityEngine;
using System.Collections;

public abstract class AtomizerEffect 
{
	public bool IsFinished
	{ get; set; }

	public bool IsPlaying
	{ get; set; }
	
	public int ParticleSkip
	{ 
		get { return particleSkip; }
		set { particleSkip = value; }
	}
	
	public bool IsReady
	{ get; protected set; }
	
#if UNITY_ANDROID || UNITY_IPHONE
	private int particleSkip = 3;
#else
	private int particleSkip = 2;
#endif
	protected int particleSkipIndex = 0;
	
	public void UpdateParticleSkipIndex()
	{
		particleSkipIndex = (particleSkipIndex + 1) % ParticleSkip;
	}
	
	public virtual IEnumerator SetParticles(ParticleSystem.Particle[] particles)
	{ 
		IsReady = true;
		yield break;
	}
	
	public abstract void Update(ParticleSystem.Particle[] particles, float activeTime);
}