using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class QualityVolumeHook : MonoBehaviour
{
	public PostProcessProfile[] PostProcessProfiles;

	private PostProcessVolume myPPVolume;

	private void Awake()
	{
		myPPVolume = GetComponent<PostProcessVolume>();
		if (myPPVolume != null)
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (PostProcessProfiles != null && PostProcessProfiles[qualityLevel] != null)
			{
				myPPVolume.profile = PostProcessProfiles[qualityLevel];
				myPPVolume.enabled = true;
			}
		}
	}
}
