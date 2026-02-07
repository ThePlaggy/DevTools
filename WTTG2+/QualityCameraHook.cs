using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class QualityCameraHook : MonoBehaviour
{
	private PostProcessLayer myPPLayer;

	private void Awake()
	{
		myPPLayer = GetComponent<PostProcessLayer>();
		if (myPPLayer != null)
		{
			switch (QualitySettings.GetQualityLevel())
			{
			case 0:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
				break;
			case 1:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				break;
			case 2:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
				break;
			case 3:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				break;
			case 4:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				break;
			case 5:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				break;
			case 6:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
				break;
			default:
				myPPLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
				break;
			}
		}
	}
}
