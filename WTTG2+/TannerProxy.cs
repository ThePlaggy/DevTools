using UnityEngine;

public class TannerProxy : MonoBehaviour
{
	private GameObject doorvpn;

	private GameObject balconvpn;

	private void Awake()
	{
		doorvpn = GameObject.CreatePrimitive(PrimitiveType.Cube);
		doorvpn.name = "TannerProxy1";
		doorvpn.transform.position = new Vector3(-2.0411f, 40.5182f, -6.3f);
		doorvpn.transform.localScale = new Vector3(4.25f, 3f, 1f);
		doorvpn.GetComponent<MeshRenderer>().enabled = false;
		doorvpn.layer = 8;
		doorvpn.SetActive(value: false);
		balconvpn = GameObject.CreatePrimitive(PrimitiveType.Cube);
		balconvpn.name = "TannerProxy2";
		balconvpn.transform.position = new Vector3(1.0695f, 40.1391f, 4.1022f);
		balconvpn.transform.localScale = new Vector3(4.25f, 3f, 1f);
		balconvpn.GetComponent<MeshRenderer>().enabled = false;
		balconvpn.layer = 8;
		balconvpn.SetActive(value: false);
	}

	public void ToggleDoorProxy(bool state)
	{
		doorvpn.SetActive(state);
	}

	public void ToggleBalconyProxy(bool state)
	{
		balconvpn.SetActive(state);
	}
}
