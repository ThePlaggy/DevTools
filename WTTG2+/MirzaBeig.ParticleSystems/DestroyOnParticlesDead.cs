using UnityEngine;

namespace MirzaBeig.ParticleSystems
{

	public class DestroyOnParticlesDead : ParticleSystems
	{
		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
			base.onParticleSystemsDeadEvent += onParticleSystemsDead;
		}

		protected override void Update()
		{
			base.Update();
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
		}

		private void onParticleSystemsDead()
		{
			Object.Destroy(base.gameObject);
		}
	}
}