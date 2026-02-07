using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ComputerScreenHook : MonoBehaviour
{
	public static ComputerScreenHook Ins;

	public MeshRenderer MeshRenderer { get; private set; }

	private void Awake()
	{
		Ins = this;
		MeshRenderer = GetComponent<MeshRenderer>();
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}

	public void ShowBack()
	{
		base.gameObject.SetActive(value: true);
	}
}
