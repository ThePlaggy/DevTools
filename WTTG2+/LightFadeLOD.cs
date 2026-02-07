using UnityEngine;

public class LightFadeLOD : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Fades shadow strength to 0 when camera is leaving range.")]
	private bool shadowStrength = true;

	[SerializeField]
	[Tooltip("Disables shadows when camera is out of range. (Does not affect shadow strength)")]
	private bool toggleShadows = true;

	[SerializeField]
	[Tooltip("Fades light range to 0 when camera is leaving range.")]
	private bool range;

	[SerializeField]
	[Tooltip("Fades light intensity to 0 when camera is leaving range.")]
	private bool intensity = true;

	[SerializeField]
	private float beginFadeDistance = 10f;

	[SerializeField]
	private float endFadeDistance = 20f;

	[SerializeField]
	private Vector3 center = Vector3.zero;

	[SerializeField]
	private Light lodLight;

	[SerializeField]
	private bool hideGizmo;

	private float defaultIntensity;

	private float defaultRange;

	private LightShadows defaultShadows;

	private float defaultShadowStrength;

	private float distance;

	private float fadePercent;

	private Camera mainCamera;

	private void Awake()
	{
		mainCamera = Camera.main;
		defaultShadows = lodLight.shadows;
		defaultRange = lodLight.range;
		defaultIntensity = lodLight.intensity;
		defaultShadowStrength = lodLight.shadowStrength;
	}

	private void Update()
	{
		UpdateDistance();
		UpdateFadePercent();
		UpdateLightValues();
	}

	private void OnDrawGizmos()
	{
		if (!hideGizmo)
		{
			if (lodLight == null)
			{
				lodLight = GetComponent<Light>();
			}
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			UpdateDistance();
			UpdateFadePercent();
			Vector3 vector = ((!(lodLight != null)) ? base.transform.TransformPoint(center) : lodLight.transform.TransformPoint(center));
			Gizmos.color = new Color(0.7f, 0.7f, 0f, 1f);
			Gizmos.DrawWireSphere(vector, beginFadeDistance);
			Gizmos.color = new Color(0.7f, 0.7f, 0f, 0.5f);
			Gizmos.DrawWireSphere(vector, endFadeDistance);
			if (Application.isPlaying && mainCamera != null)
			{
				Gizmos.DrawLine(vector, mainCamera.transform.position);
			}
		}
	}

	private void OnValidate()
	{
		if (lodLight == null)
		{
			lodLight = GetComponent<Light>();
		}
		float min = ((!(lodLight != null)) ? float.MinValue : lodLight.range);
		beginFadeDistance = Mathf.Clamp(beginFadeDistance, min, endFadeDistance);
		endFadeDistance = Mathf.Clamp(endFadeDistance, beginFadeDistance, float.MaxValue);
	}

	private void UpdateDistance()
	{
		if (lodLight == null || mainCamera == null)
		{
			distance = 0f;
			return;
		}
		Vector3 vector = lodLight.transform.TransformPoint(center);
		Vector3 position = mainCamera.transform.position;
		distance = (vector - position).magnitude;
	}

	private void UpdateFadePercent()
	{
		if (lodLight == null || mainCamera == null || distance < beginFadeDistance)
		{
			fadePercent = 1f;
		}
		else
		{
			fadePercent = Mathf.Clamp01(1f - (distance - beginFadeDistance) / (endFadeDistance - beginFadeDistance));
		}
	}

	private void UpdateLightValues()
	{
		if (!(lodLight == null))
		{
			if (intensity)
			{
				lodLight.intensity = Mathf.Lerp(0f, defaultIntensity, fadePercent);
			}
			else
			{
				lodLight.intensity = defaultIntensity;
			}
			if (range)
			{
				lodLight.range = Mathf.Lerp(0f, defaultRange, fadePercent);
			}
			else
			{
				lodLight.range = defaultRange;
			}
			if (shadowStrength)
			{
				lodLight.shadowStrength = Mathf.Lerp(0f, defaultShadowStrength, fadePercent);
			}
			else
			{
				lodLight.shadowStrength = defaultShadowStrength;
			}
			if (toggleShadows)
			{
				lodLight.shadows = ((fadePercent > 0f) ? defaultShadows : LightShadows.None);
				return;
			}
			LightShadows shadows = ((lodLight.shadowStrength > 0f) ? defaultShadows : LightShadows.None);
			lodLight.shadows = shadows;
		}
	}
}
