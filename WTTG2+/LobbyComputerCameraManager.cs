using Colorful;
using UnityEngine;

public class LobbyComputerCameraManager : MonoBehaviour
{
	[SerializeField]
	private RenderTexture myRenderTexture;

	private Glitch clitchPost;

	private Camera mainCamera;

	private Camera myCamera;

	private void Awake()
	{
		myCamera = GetComponent<Camera>();
		myRenderTexture.height = Screen.height;
		myRenderTexture.width = Screen.width;
		myCamera.orthographicSize = (float)Screen.height / 2f;
		base.transform.localPosition = new Vector3((float)Screen.width / 2f, 0f - (float)(Screen.height / 2), base.transform.localPosition.z);
		myCamera.targetTexture = myRenderTexture;
		clitchPost = GetComponent<Glitch>();
		CameraManager.Get(CAMERA_ID.MAIN, out mainCamera);
	}

	public void BecomeMaster()
	{
		myCamera.targetTexture = null;
		mainCamera.enabled = false;
	}

	public void BecomeSlave()
	{
		myCamera.targetTexture = myRenderTexture;
		mainCamera.enabled = true;
	}

	public void TriggerGlitch()
	{
		clitchPost.enabled = true;
		GameManager.TimeSlinger.FireTimer(3f, delegate
		{
			clitchPost.enabled = false;
		});
	}
}
