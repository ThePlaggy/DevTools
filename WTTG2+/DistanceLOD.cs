using System.Collections.Generic;
using UnityEngine;

public class DistanceLOD : MonoBehaviour
{
	public bool OverwriteCulling;

	[SerializeField]
	private GameObject gatherChildrenTarget;

	[SerializeField]
	private float updateDelay = 0.1f;

	[SerializeField]
	private float cullDistance = 10f;

	[SerializeField]
	private bool enableForceEnableHotZones;

	[SerializeField]
	private HotZoneTrigger[] enableHotZones = new HotZoneTrigger[0];

	[SerializeField]
	private Renderer[] renderers = new Renderer[0];

	[SerializeField]
	private Light[] lights = new Light[0];

	[SerializeField]
	private InteractiveLight[] interactiveLights = new InteractiveLight[0];

	[SerializeField]
	private Vector3 center = Vector3.zero;

	[SerializeField]
	private Vector3 size = Vector3.one;

	[SerializeField]
	private bool hideGizmo;

	private Bounds bounds;

	private float distance;

	private Camera mainCamera;

	private void Awake()
	{
		mainCamera = Camera.main;
		UpdateBounds();
	}

	private void OnEnable()
	{
		InvokeRepeating("OnUpdate", 0f, updateDelay);
	}

	private void OnDisable()
	{
		CancelInvoke("OnUpdate");
	}

	private void OnDrawGizmos()
	{
		if (!hideGizmo)
		{
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			UpdateBounds();
			UpdateDistance();
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(bounds.center, bounds.size);
			if (mainCamera != null)
			{
				Vector3 position = mainCamera.transform.position;
				Vector3 to = bounds.ClosestPoint(position);
				Gizmos.DrawLine(position, to);
			}
		}
	}

	private void OnUpdate()
	{
		UpdateDistance();
		DoEnable(cullDistance > distance);
	}

	private void DoEnable(bool enable)
	{
		if (CustomElevatorManager.Elevating)
		{
			enable = true;
		}
		if (enableForceEnableHotZones)
		{
			for (int i = 0; i < enableHotZones.Length; i++)
			{
				if (enableHotZones[i].IsHot)
				{
					enable = true;
					i = enableHotZones.Length;
				}
			}
		}
		if (OverwriteCulling)
		{
			enable = true;
		}
		for (int j = 0; j < renderers.Length; j++)
		{
			if (!(renderers[j] == null))
			{
				renderers[j].enabled = enable;
			}
		}
		for (int k = 0; k < lights.Length; k++)
		{
			if (!(lights[k] == null))
			{
				lights[k].enabled = enable;
			}
		}
		for (int l = 0; l < interactiveLights.Length; l++)
		{
			if (!(interactiveLights[l] == null) && !interactiveLights[l].SetToOff)
			{
				interactiveLights[l].MyLight.enabled = enable;
			}
		}
	}

	private void UpdateDistance()
	{
		if (!(mainCamera == null))
		{
			Vector3 position = mainCamera.transform.position;
			if (bounds.Contains(position))
			{
				distance = 0f;
				return;
			}
			Vector3 vector = bounds.ClosestPoint(position);
			distance = (vector - position).magnitude;
		}
	}

	private void UpdateBounds()
	{
		bounds = new Bounds(base.transform.TransformPoint(center), size);
	}

	[ContextMenu("Recalculate Bounds")]
	private void RecalculateBounds()
	{
		if (renderers.Length < 1)
		{
			bounds = new Bounds(base.transform.position, Vector3.zero);
			return;
		}
		bool flag = true;
		for (int i = 0; i < renderers.Length; i++)
		{
			if (!(renderers[i] == null))
			{
				if (flag)
				{
					bounds = renderers[i].bounds;
					flag = false;
				}
				else
				{
					bounds.Encapsulate(renderers[i].bounds);
				}
			}
		}
		center = base.transform.InverseTransformPoint(bounds.center);
		size = bounds.size;
	}

	[ContextMenu("Gather Target Children")]
	private void GatherTargetChildren()
	{
		if (gatherChildrenTarget == null)
		{
			gatherChildrenTarget = base.gameObject;
		}
		List<Renderer> list = new List<Renderer>(renderers);
		list.AddRange(gatherChildrenTarget.GetComponentsInChildren<Renderer>());
		renderers = list.ToArray();
		List<Light> list2 = new List<Light>(lights);
		list2.AddRange(gatherChildrenTarget.GetComponentsInChildren<Light>());
		lights = list2.ToArray();
		renderers = new List<Renderer>(new HashSet<Renderer>(renderers)).ToArray();
		lights = new List<Light>(new HashSet<Light>(lights)).ToArray();
		gatherChildrenTarget = null;
	}
}
