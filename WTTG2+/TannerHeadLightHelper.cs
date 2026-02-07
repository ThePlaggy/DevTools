using UnityEngine;

public class TannerHeadLightHelper : MonoBehaviour
{
	public static TannerHeadLightHelper Ins;

	private Light myLight;

	private Light originalLight;

	private Light customLight;

	public void DisableLight()
	{
		Debug.Log("[Tanner] Disable Light");
		myLight.enabled = false;
	}

	public void EnableLight()
	{
		Debug.Log("[Tanner] Enable Light");
		myLight.enabled = true;
	}

	public void SwitchLightSource(bool toCustom)
	{
		if (toCustom)
		{
			if (customLight == null)
			{
				customLight = GameObject.Find("CustomTannerHeadLight").GetComponent<Light>();
			}
			myLight = customLight;
		}
		else
		{
			if (originalLight == null)
			{
				originalLight = GameObject.Find("TannerHeadLight").GetComponent<Light>();
			}
			myLight = originalLight;
		}
	}

	private void Awake()
	{
		Ins = this;
		SwitchLightSource(toCustom: false);
		DisableLight();
	}

	private void OnDestroy()
	{
		Ins = null;
	}
}
