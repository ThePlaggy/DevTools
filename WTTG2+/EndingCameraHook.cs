using Colorful;
using DG.Tweening;
using UnityEngine;

public class EndingCameraHook : MonoBehaviour
{
	public static EndingCameraHook Ins;

	private DirectionalBlur blurPP;

	private DoubleVision doubleVis;

	private AudioReverbFilter myARF;

	private void Awake()
	{
		Ins = this;
		myARF = GetComponent<AudioReverbFilter>();
		blurPP = GetComponent<DirectionalBlur>();
		doubleVis = GetComponent<DoubleVision>();
	}

	public void WakeUp()
	{
		base.transform.localRotation = Quaternion.Euler(new Vector3(8.7f, 13.73f, -20.29f));
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
		});
		float duration = 18f;
		sequence.Insert(0f, DOTween.To(() => base.transform.localRotation, delegate(Quaternion x)
		{
			base.transform.localRotation = x;
		}, Vector3.zero, 3f).SetEase(Ease.Linear).SetOptions());
		sequence.Insert(0f, DOTween.To(() => blurPP.Strength, delegate(float x)
		{
			blurPP.Strength = x;
		}, 0f, 5f).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.dryLevel, delegate(float x)
		{
			myARF.dryLevel = x;
		}, 0f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.room, delegate(float x)
		{
			myARF.room = x;
		}, -10000f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.roomHF, delegate(float x)
		{
			myARF.roomHF = x;
		}, -10000f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.roomLF, delegate(float x)
		{
			myARF.roomLF = x;
		}, 0f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.decayHFRatio, delegate(float x)
		{
			myARF.decayHFRatio = x;
		}, 1f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.reflectionsLevel, delegate(float x)
		{
			myARF.reflectionsLevel = x;
		}, -2602f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.reflectionsDelay, delegate(float x)
		{
			myARF.reflectionsDelay = x;
		}, 0f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.reverbLevel, delegate(float x)
		{
			myARF.reverbLevel = x;
		}, 200f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.reverbDelay, delegate(float x)
		{
			myARF.reverbDelay = x;
		}, 0.011f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.hfReference, delegate(float x)
		{
			myARF.hfReference = x;
		}, 5000f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.lfReference, delegate(float x)
		{
			myARF.lfReference = x;
		}, 250f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.diffusion, delegate(float x)
		{
			myARF.diffusion = x;
		}, 0f, duration).SetEase(Ease.Linear));
		sequence.Insert(5f, DOTween.To(() => myARF.density, delegate(float x)
		{
			myARF.density = x;
		}, 0f, duration).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void AddHeadHit(float SetAmt = 1f)
	{
		myARF.enabled = true;
		myARF.reverbPreset = AudioReverbPreset.Drugged;
		doubleVis.enabled = true;
		doubleVis.Displace.x = doubleVis.Displace.x - SetAmt;
		doubleVis.Displace.y = doubleVis.Displace.y + SetAmt;
	}

	public void TriggerKnifeDeath()
	{
		doubleVis.enabled = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.x, delegate(float x)
		{
			doubleVis.Displace.x = x;
		}, -50f, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => doubleVis.Displace.y, delegate(float x)
		{
			doubleVis.Displace.y = x;
		}, 50f, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => blurPP.Strength, delegate(float x)
		{
			blurPP.Strength = x;
		}, 1.25f, 0.75f).SetEase(Ease.Linear));
		sequence.Play();
	}

	public void EnableAFR()
	{
		myARF.enabled = true;
		myARF.reverbPreset = AudioReverbPreset.Drugged;
	}

	public void ClearAFR()
	{
		myARF.enabled = false;
	}
}
