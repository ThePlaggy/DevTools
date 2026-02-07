using UnityEngine;

public class KeypadCanvasManager : MonoBehaviour
{
	public void ButtonAction(int id)
	{
		if (KeypadManager.Ins != null)
		{
			KeypadManager.Ins.ButtonAction(id);
		}
	}
}
