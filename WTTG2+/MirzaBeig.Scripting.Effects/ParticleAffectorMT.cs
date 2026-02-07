using System;
using System.Threading;
using UnityEngine;

namespace MirzaBeig.Scripting.Effects;

[Serializable]
public class ParticleAffectorMT : MonoBehaviour
{
	public float force = 1f;

	public float speed = 1f;

	private readonly object locker = new object();

	private float deltaTime;

	private bool isDoneAssigning;

	private float offsetX;

	private float offsetY;

	private float offsetZ;

	private ParticleSystem.Particle[] particles;

	private ParticleSystem particleSystem;

	private bool processing;

	private float randomX;

	private float randomY;

	private float randomZ;

	private Thread t;

	private void Awake()
	{
	}

	private void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
		randomX = UnityEngine.Random.Range(-32f, 32f);
		randomY = UnityEngine.Random.Range(-32f, 32f);
		randomZ = UnityEngine.Random.Range(-32f, 32f);
		t = new Thread(process);
		t.Start();
		isDoneAssigning = true;
	}

	private void LateUpdate()
	{
		object obj = locker;
		lock (obj)
		{
			if (!processing && isDoneAssigning)
			{
				particles = new ParticleSystem.Particle[particleSystem.particleCount];
				particleSystem.GetParticles(particles);
				float time = Time.time;
				deltaTime = Time.deltaTime;
				offsetX = time * speed * randomX;
				offsetY = time * speed * randomY;
				offsetZ = time * speed * randomZ;
				processing = true;
				isDoneAssigning = false;
			}
		}
		if (t.ThreadState == ThreadState.Stopped)
		{
			t = new Thread(process);
			t.Start();
		}
		object obj2 = locker;
		lock (obj2)
		{
			if (!processing && !isDoneAssigning)
			{
				particleSystem.SetParticles(particles, particles.Length);
				isDoneAssigning = true;
			}
		}
	}

	private void OnDisable()
	{
	}

	private void OnApplicationQuit()
	{
	}

	private void process()
	{
		object obj = locker;
		lock (obj)
		{
			if (processing)
			{
				for (int i = 0; i < particles.Length; i++)
				{
					ParticleSystem.Particle particle = particles[i];
					Vector3 position = particle.position;
					Vector3 vector = new Vector3(Noise.perlin(offsetX + position.x, offsetX + position.y, offsetX + position.z), Noise.perlin(offsetY + position.x, offsetY + position.y, offsetY + position.z), Noise.perlin(offsetZ + position.x, offsetZ + position.y, offsetZ + position.z)) * force;
					vector *= deltaTime;
					particle.velocity += vector;
					particles[i] = particle;
				}
				processing = false;
			}
		}
	}
}
