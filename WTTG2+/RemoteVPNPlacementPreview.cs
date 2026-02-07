using UnityEngine;
using UnityEngine.UI;

public class RemoteVPNPlacementPreview : MonoBehaviour
{
	[SerializeField]
	private Material myMat;

	public void ChangePreview(Color color, Sprite sprite)
	{
		myMat.SetColor("_EmissionColor", color);
		LookUp.PlayerUI.RemoteVPNIcon.GetComponent<Image>().sprite = sprite;
	}
}
