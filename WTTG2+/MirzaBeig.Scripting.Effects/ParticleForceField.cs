using System;
using System.Collections.Generic;
using UnityEngine;

namespace MirzaBeig.Scripting.Effects;

public abstract class ParticleForceField : MonoBehaviour
{
	protected struct GetForceParameters
	{
		public float distanceToForceFieldCenterSqr;

		public Vector3 scaledDirectionToForceFieldCenter;

		public Vector3 particlePosition;
	}

	[Header("Common Controls")]
	[Tooltip("Force field spherical range.")]
	public float radius = float.PositiveInfinity;

	[Tooltip("Maximum baseline force.")]
	public float force = 5f;

	[Tooltip("Internal force field position offset.")]
	public Vector3 center = Vector3.zero;

	[Tooltip("Force scale as determined by distance to individual particles.")]
	public AnimationCurve forceOverDistance = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	[Tooltip("If nothing no particle systems are assigned, this force field will operate globally on ALL particle systems in the scene (NOT recommended).\n\nIf attached to a particle system, the force field will operate only on that system.\n\nIf specific particle systems are assigned, then the force field will operate on those systems only, even if attached to a particle system.")]
	public List<ParticleSystem> _particleSystems;

	[Tooltip("If TRUE, update even if target particle system(s) are invisible/offscreen.\n\nIf FALSE, update only if particles of the target system(s) are visible/onscreen.")]
	public bool alwaysUpdate;

	private float _radius;

	protected ParticleSystem currentParticleSystem;

	private float forceDeltaTime;

	protected GetForceParameters parameters;

	private ParticleSystem particleSystem;

	private float[] particleSystemExternalForcesMultipliers;

	private ParticleSystem.MainModule[] particleSystemMainModules;

	private ParticleSystem.Particle[][] particleSystemParticles;

	private Renderer[] particleSystemRenderers;

	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	private int particleSystemsCount;

	private float radiusSqr;

	private Vector3 transformPosition;

	public float scaledRadius => radius * base.transform.lossyScale.x;

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		particleSystem = GetComponent<ParticleSystem>();
	}

	protected virtual void Update()
	{
	}

	protected virtual void LateUpdate()
	{
		_radius = scaledRadius;
		radiusSqr = _radius * _radius;
		forceDeltaTime = force * Time.deltaTime;
		transformPosition = base.transform.position + center;
		if (_particleSystems.Count != 0)
		{
			if (particleSystems.Count != _particleSystems.Count)
			{
				particleSystems.Clear();
				particleSystems.AddRange(_particleSystems);
			}
			else
			{
				for (int i = 0; i < _particleSystems.Count; i++)
				{
					particleSystems[i] = _particleSystems[i];
				}
			}
		}
		else if ((bool)particleSystem)
		{
			if (particleSystems.Count == 1)
			{
				particleSystems[0] = particleSystem;
			}
			else
			{
				particleSystems.Clear();
				particleSystems.Add(particleSystem);
			}
		}
		else
		{
			particleSystems.Clear();
			particleSystems.AddRange(UnityEngine.Object.FindObjectsOfType<ParticleSystem>());
		}
		parameters = default(GetForceParameters);
		particleSystemsCount = particleSystems.Count;
		if (particleSystemParticles == null || particleSystemParticles.Length < particleSystemsCount)
		{
			particleSystemParticles = new ParticleSystem.Particle[particleSystemsCount][];
			particleSystemMainModules = new ParticleSystem.MainModule[particleSystemsCount];
			particleSystemRenderers = new Renderer[particleSystemsCount];
			particleSystemExternalForcesMultipliers = new float[particleSystemsCount];
			for (int j = 0; j < particleSystemsCount; j++)
			{
				particleSystemMainModules[j] = particleSystems[j].main;
				particleSystemRenderers[j] = particleSystems[j].GetComponent<Renderer>();
				particleSystemExternalForcesMultipliers[j] = particleSystems[j].externalForces.multiplier;
			}
		}
		for (int k = 0; k < particleSystemsCount; k++)
		{
			if (!particleSystemRenderers[k].isVisible && !alwaysUpdate)
			{
				continue;
			}
			int maxParticles = particleSystemMainModules[k].maxParticles;
			if (particleSystemParticles[k] == null || particleSystemParticles[k].Length < maxParticles)
			{
				particleSystemParticles[k] = new ParticleSystem.Particle[maxParticles];
			}
			currentParticleSystem = particleSystems[k];
			PerParticleSystemSetup();
			int particles = currentParticleSystem.GetParticles(particleSystemParticles[k]);
			ParticleSystemSimulationSpace simulationSpace = particleSystemMainModules[k].simulationSpace;
			ParticleSystemScalingMode scalingMode = particleSystemMainModules[k].scalingMode;
			Transform transform = currentParticleSystem.transform;
			Transform customSimulationSpace = particleSystemMainModules[k].customSimulationSpace;
			if (simulationSpace == ParticleSystemSimulationSpace.World)
			{
				for (int l = 0; l < particles; l++)
				{
					parameters.particlePosition = particleSystemParticles[k][l].position;
					parameters.scaledDirectionToForceFieldCenter.x = transformPosition.x - parameters.particlePosition.x;
					parameters.scaledDirectionToForceFieldCenter.y = transformPosition.y - parameters.particlePosition.y;
					parameters.scaledDirectionToForceFieldCenter.z = transformPosition.z - parameters.particlePosition.z;
					parameters.distanceToForceFieldCenterSqr = parameters.scaledDirectionToForceFieldCenter.sqrMagnitude;
					if (parameters.distanceToForceFieldCenterSqr < radiusSqr)
					{
						float time = parameters.distanceToForceFieldCenterSqr / radiusSqr;
						float num = forceOverDistance.Evaluate(time);
						Vector3 vector = GetForce();
						float num2 = forceDeltaTime * num * particleSystemExternalForcesMultipliers[k];
						vector.x *= num2;
						vector.y *= num2;
						vector.z *= num2;
						Vector3 velocity = particleSystemParticles[k][l].velocity;
						velocity.x += vector.x;
						velocity.y += vector.y;
						velocity.z += vector.z;
						particleSystemParticles[k][l].velocity = velocity;
					}
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				Quaternion identity = Quaternion.identity;
				Vector3 one = Vector3.one;
				Transform transform2 = transform;
				switch (simulationSpace)
				{
				default:
					throw new NotSupportedException($"Unsupported scaling mode '{simulationSpace}'.");
				case ParticleSystemSimulationSpace.Custom:
					transform2 = customSimulationSpace;
					zero = transform2.position;
					identity = transform2.rotation;
					one = transform2.localScale;
					break;
				case ParticleSystemSimulationSpace.Local:
					zero = transform2.position;
					identity = transform2.rotation;
					one = transform2.localScale;
					break;
				}
				for (int m = 0; m < particles; m++)
				{
					parameters.particlePosition = particleSystemParticles[k][m].position;
					if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
					{
						switch (scalingMode)
						{
						case ParticleSystemScalingMode.Hierarchy:
							parameters.particlePosition = transform2.TransformPoint(particleSystemParticles[k][m].position);
							break;
						case ParticleSystemScalingMode.Local:
							parameters.particlePosition = Vector3.Scale(parameters.particlePosition, one);
							parameters.particlePosition = identity * parameters.particlePosition;
							parameters.particlePosition += zero;
							break;
						case ParticleSystemScalingMode.Shape:
							parameters.particlePosition = identity * parameters.particlePosition;
							parameters.particlePosition += zero;
							break;
						default:
							throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
						}
					}
					parameters.scaledDirectionToForceFieldCenter.x = transformPosition.x - parameters.particlePosition.x;
					parameters.scaledDirectionToForceFieldCenter.y = transformPosition.y - parameters.particlePosition.y;
					parameters.scaledDirectionToForceFieldCenter.z = transformPosition.z - parameters.particlePosition.z;
					parameters.distanceToForceFieldCenterSqr = parameters.scaledDirectionToForceFieldCenter.sqrMagnitude;
					if (!(parameters.distanceToForceFieldCenterSqr < radiusSqr))
					{
						continue;
					}
					float time2 = parameters.distanceToForceFieldCenterSqr / radiusSqr;
					float num3 = forceOverDistance.Evaluate(time2);
					Vector3 vector2 = GetForce();
					float num4 = forceDeltaTime * num3 * particleSystemExternalForcesMultipliers[k];
					vector2.x *= num4;
					vector2.y *= num4;
					vector2.z *= num4;
					if (simulationSpace == ParticleSystemSimulationSpace.Local || simulationSpace == ParticleSystemSimulationSpace.Custom)
					{
						switch (scalingMode)
						{
						case ParticleSystemScalingMode.Hierarchy:
							vector2 = transform2.InverseTransformVector(vector2);
							break;
						case ParticleSystemScalingMode.Local:
							vector2 = Quaternion.Inverse(identity) * vector2;
							vector2 = Vector3.Scale(vector2, new Vector3(1f / one.x, 1f / one.y, 1f / one.z));
							break;
						case ParticleSystemScalingMode.Shape:
							vector2 = Quaternion.Inverse(identity) * vector2;
							break;
						default:
							throw new NotSupportedException($"Unsupported scaling mode '{scalingMode}'.");
						}
					}
					Vector3 velocity2 = particleSystemParticles[k][m].velocity;
					velocity2.x += vector2.x;
					velocity2.y += vector2.y;
					velocity2.z += vector2.z;
					particleSystemParticles[k][m].velocity = velocity2;
				}
			}
			currentParticleSystem.SetParticles(particleSystemParticles[k], particles);
		}
	}

	private void OnApplicationQuit()
	{
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position + center, scaledRadius);
	}

	protected virtual void PerParticleSystemSetup()
	{
	}

	protected virtual Vector3 GetForce()
	{
		return Vector3.zero;
	}

	public void AddParticleSystem(ParticleSystem particleSystem)
	{
		_particleSystems.Add(particleSystem);
	}

	public void RemoveParticleSystem(ParticleSystem particleSystem)
	{
		_particleSystems.Remove(particleSystem);
	}
}
