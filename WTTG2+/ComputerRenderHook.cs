using UnityEngine;

public class ComputerRenderHook : MonoBehaviour
{
	private Material renderMat;

	private void Awake()
	{
		renderMat = GetComponent<MeshRenderer>().material;
		renderMat.SetTexture("_MainTex", ComputerCameraManager.Ins.FinalRenderTexture);
	}
}
