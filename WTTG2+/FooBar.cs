using UnityEngine;

public class FooBar : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			ScreenCapture.CaptureScreenshot("ScreenShot.png");
		}
	}

	private void test()
	{
	}

	private void foo()
	{
	}
}
