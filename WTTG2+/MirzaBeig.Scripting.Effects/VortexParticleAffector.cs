using UnityEngine;

namespace MirzaBeig.Scripting.Effects
{

	public class VortexParticleAffector : ParticleAffector
	{
		[Header("Affector Controls")]
		public Vector3 axisOfRotationOffset = Vector3.zero;

		private Vector3 axisOfRotation;

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			base.Update();
		}

		protected override void LateUpdate()
		{
			base.LateUpdate();
		}

		protected override void OnDrawGizmosSelected()
		{
			if (base.enabled)
			{
				base.OnDrawGizmosSelected();
				Gizmos.color = Color.red;
				Vector3 vector;
				if (Application.isPlaying && base.enabled)
				{
					UpdateAxisOfRotation();
					vector = axisOfRotation;
				}
				else
				{
					vector = Quaternion.Euler(axisOfRotationOffset) * base.transform.up;
				}
				Gizmos.DrawLine(base.transform.position + offset, base.transform.position + offset + vector * base.scaledRadius);
			}
		}

		private void UpdateAxisOfRotation()
		{
			axisOfRotation = Quaternion.Euler(axisOfRotationOffset) * base.transform.up;
		}

		protected override void PerParticleSystemSetup()
		{
			UpdateAxisOfRotation();
		}

		protected override Vector3 GetForce()
		{
			return Vector3.Normalize(Vector3.Cross(axisOfRotation, parameters.scaledDirectionToAffectorCenter));
		}
	}
}