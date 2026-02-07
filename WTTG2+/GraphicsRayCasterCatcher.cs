using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class GraphicsRayCasterCatcher : MonoBehaviour
{
	private bool myLastState;

	private GraphicRaycaster myRayCaster;

	private void Awake()
	{
		myRayCaster = GetComponent<GraphicRaycaster>();
		GameManager.PauseManager.GamePaused += gameWasPaused;
		GameManager.PauseManager.GameUnPaused += gameWasUnPaused;
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= gameWasPaused;
		GameManager.PauseManager.GameUnPaused -= gameWasUnPaused;
	}

	private void gameWasPaused()
	{
		myLastState = myRayCaster.enabled;
		myRayCaster.enabled = false;
	}

	private void gameWasUnPaused()
	{
		myRayCaster.enabled = myLastState;
	}
}
