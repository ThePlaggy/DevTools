using Colorful;
using DG.Tweening;
using UnityEngine;

public class MainCameraHook : MonoBehaviour
{
	public static MainCameraHook Ins;

	private DoubleVision doubleVis;

	private LensDistortionBlur lensDistBlurPost;

	private AudioReverbFilter myARF;

	private Camera myCamera;

	public bool GetMyARFEnabled => myARF.enabled;

	private void Awake()
	{
		Ins = this;
		myCamera = GetComponent<Camera>();
		lensDistBlurPost = GetComponent<LensDistortionBlur>();
		doubleVis = GetComponent<DoubleVision>();
		myARF = GetComponent<AudioReverbFilter>();
		lensDistBlurPost.enabled = false;
		doubleVis.enabled = false;
		myARF.enabled = false;
	}

	public void TriggerFlashBlur()
	{
		lensDistBlurPost.Distortion = 1.5f;
		lensDistBlurPost.CubicDistortion = 1.1f;
		lensDistBlurPost.enabled = true;
		myARF.enabled = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => lensDistBlurPost.Distortion, delegate(float x)
		{
			lensDistBlurPost.Distortion = x;
		}, 0f, 2.5f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => lensDistBlurPost.CubicDistortion, delegate(float x)
		{
			lensDistBlurPost.CubicDistortion = x;
		}, 0f, 2.5f).SetEase(Ease.Linear));
		sequence.SetDelay(5f);
		sequence.Play();
		Debug.Log("ARF Check:" + myARF.enabled);
	}

	public void TriggerHitManJump()
	{
		myARF.reverbPreset = AudioReverbPreset.Livingroom;
		myARF.enabled = true;
		Debug.Log("ARF Check:" + myARF.enabled);
	}

	public void AddBodyHit()
	{
		myARF.enabled = true;
		doubleVis.Displace.x = doubleVis.Displace.x - 0.4f;
		doubleVis.Displace.y = doubleVis.Displace.y + 0.4f;
		Debug.Log("ARF Check:" + myARF.enabled);
	}

	public void AddHeadHit(float SetAmt = 1f)
	{
		myARF.enabled = true;
		doubleVis.enabled = true;
		doubleVis.Displace.x = doubleVis.Displace.x - SetAmt;
		doubleVis.Displace.y = doubleVis.Displace.y + SetAmt;
		Debug.Log("ARF Check:" + myARF.enabled);
	}

	public void RemoveHeadHit()
	{
		myARF.enabled = false;
		doubleVis.enabled = false;
		Debug.Log("Remove Head Hit");
	}

	public void BlackOut(float Delay, float Fade)
	{
		LookUp.PlayerUI.BlackScreenCG.alpha = 1f;
		DOTween.To(() => LookUp.PlayerUI.BlackScreenCG.alpha, delegate(float x)
		{
			LookUp.PlayerUI.BlackScreenCG.alpha = x;
		}, 0f, Fade).SetDelay(Delay);
	}

	public void FadeDoubleVis(float Duration, float Value)
	{
		doubleVis.enabled = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.x, delegate(float x)
		{
			doubleVis.Displace.x = x;
		}, 0f - Value, Duration).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.y, delegate(float x)
		{
			doubleVis.Displace.y = x;
		}, Value, Duration).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void ClearARF(float ClearTime = 2f)
	{
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			Debug.Log("Clear ARF Sequence");
			myARF.enabled = false;
		});
		sequence.Insert(0f, DOTween.To(() => myARF.dryLevel, delegate(float x)
		{
			myARF.dryLevel = x;
		}, 0f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.room, delegate(float x)
		{
			myARF.room = x;
		}, -10000f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.roomHF, delegate(float x)
		{
			myARF.roomHF = x;
		}, -10000f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.roomLF, delegate(float x)
		{
			myARF.roomLF = x;
		}, 0f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.decayHFRatio, delegate(float x)
		{
			myARF.decayHFRatio = x;
		}, 1f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.reflectionsLevel, delegate(float x)
		{
			myARF.reflectionsLevel = x;
		}, -2602f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.reflectionsDelay, delegate(float x)
		{
			myARF.reflectionsDelay = x;
		}, 0f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.reverbLevel, delegate(float x)
		{
			myARF.reverbLevel = x;
		}, 200f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.reverbDelay, delegate(float x)
		{
			myARF.reverbDelay = x;
		}, 0.011f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.hfReference, delegate(float x)
		{
			myARF.hfReference = x;
		}, 5000f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.lfReference, delegate(float x)
		{
			myARF.lfReference = x;
		}, 250f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.diffusion, delegate(float x)
		{
			myARF.diffusion = x;
		}, 0f, ClearTime).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => myARF.density, delegate(float x)
		{
			myARF.density = x;
		}, 0f, ClearTime).SetEase(Ease.Linear));
		sequence.Play();
		Debug.Log("Clear ARF");
	}

	public void ResetARF()
	{
		myARF.dryLevel = 0f;
		myARF.room = -1000f;
		myARF.roomHF = -151f;
		myARF.roomLF = 0f;
		myARF.decayTime = 7.56f;
		myARF.decayHFRatio = 0.91f;
		myARF.reflectionsLevel = -626f;
		myARF.reflectionsDelay = 0f;
		myARF.reverbLevel = 774f;
		myARF.reverbDelay = 0.03f;
		myARF.hfReference = 5000f;
		myARF.lfReference = 250f;
		myARF.diffusion = 100f;
		myARF.density = 100f;
		Debug.Log("Reset ARF");
	}

	public void ClearDoubleVis(float Time)
	{
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			doubleVis.enabled = false;
		});
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.x, delegate(float x)
		{
			doubleVis.Displace.x = x;
		}, 0f, Time).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.y, delegate(float x)
		{
			doubleVis.Displace.y = x;
		}, 0f, Time).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void HardClearARF()
	{
		Debug.Log("Hard Clear ARF wtf???");
		myARF.enabled = false;
	}
}
