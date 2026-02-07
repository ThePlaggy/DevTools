using Colorful;
using UnityEngine;

public class TitleCameraManager : MonoBehaviour
{
	private AnalogTV analogPost;

	private Glitch glitchPost;

	private Camera myCamera;

	private Noise noisePost;

	private void Awake()
	{
		myCamera = GetComponent<Camera>();
		analogPost = GetComponent<AnalogTV>();
		noisePost = GetComponent<Noise>();
		glitchPost = GetComponent<Glitch>();
		analogPost.enabled = false;
		noisePost.enabled = false;
		glitchPost.enabled = false;
		TitleManager.Ins.TitlePresent.Event += titleIsPresenting;
	}

	private void OnDestroy()
	{
		TitleManager.Ins.TitlePresent.Event -= titleIsPresenting;
	}

	private void titleIsPresenting()
	{
		analogPost.enabled = true;
		noisePost.enabled = true;
		glitchPost.enabled = true;
	}
}
