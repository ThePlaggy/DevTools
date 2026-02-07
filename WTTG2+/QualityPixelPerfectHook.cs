using UnityEngine;

public class QualityPixelPerfectHook : MonoBehaviour
{
	private Canvas myCanvas;

	private void Awake()
	{
	}

	private void Start()
	{
		myCanvas = GetComponent<Canvas>();
		int qualityLevel = QualitySettings.GetQualityLevel();
		if (qualityLevel <= 1)
		{
			myCanvas.pixelPerfect = false;
		}
	}
}
