using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TannerRoamJumper : MonoBehaviour
{
	public static TannerRoamJumper Ins;

	private roamController myRoamController;

	private PostProcessVolume druggedPPVol;

	private PostProcessVolume jumpPPVol;

	public void ClearPPVol()
	{
		if (jumpPPVol != null)
		{
			RuntimeUtilities.DestroyVolume(jumpPPVol, destroyProfile: false);
		}
		if (druggedPPVol != null)
		{
			RuntimeUtilities.DestroyVolume(druggedPPVol, destroyProfile: false);
		}
	}

	public void SetJumpPPVol()
	{
		DepthOfField depthOfField = ScriptableObject.CreateInstance<DepthOfField>();
		depthOfField.enabled.Override(x: true);
		depthOfField.focusDistance.Override(0.23f);
		depthOfField.aperture.Override(25.4f);
		depthOfField.focalLength.Override(28f);
		jumpPPVol = PostProcessManager.instance.QuickVolume(myRoamController.transform.Find("PPLayer").gameObject.layer, 100f, depthOfField);
		jumpPPVol.weight = 0f;
		DOTween.To(() => jumpPPVol.weight, delegate(float x)
		{
			jumpPPVol.weight = x;
		}, 1f, 1f).SetEase(Ease.Linear);
	}

	public void SetDruggedPPVol(float intensity = 1f)
	{
		ChromaticAberration chromaticAberration = ScriptableObject.CreateInstance<ChromaticAberration>();
		chromaticAberration.enabled.Override(x: true);
		chromaticAberration.intensity.Override(intensity);
		chromaticAberration.fastMode.Override(x: true);
		druggedPPVol = PostProcessManager.instance.QuickVolume(myRoamController.transform.Find("PPLayer").gameObject.layer, 100f, chromaticAberration);
		druggedPPVol.weight = 0f;
		DOTween.To(() => druggedPPVol.weight, delegate(float x)
		{
			druggedPPVol.weight = x;
		}, 1f, 1.5f).SetEase(Ease.Linear);
	}

	private void Awake()
	{
		Ins = this;
		myRoamController = GetComponent<roamController>();
	}

	private void OnDestroy()
	{
		Ins = null;
		ClearPPVol();
	}
}
