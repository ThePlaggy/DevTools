using DG.Tweening;
using UnityEngine;

public class CameraTilterHook : MonoBehaviour
{
	public static CameraTilterHook Ins;

	private Vector3 defaultPOS;

	private DOTweenAnimation presentAni;

	private void Awake()
	{
		Ins = this;
		presentAni = GetComponent<DOTweenAnimation>();
		defaultPOS = base.transform.position;
		TitleManager.Ins.TitlePresent.Event += triggerPresentAni;
		TitleManager.Ins.TitleDismissing.Event += presentToGame;
		TitleManager.Ins.OptionsPresent.Event += presentToOptions;
		TitleManager.Ins.OptionsDismissing.Event += dismissFromOptions;
	}

	private void OnDestroy()
	{
		TitleManager.Ins.TitlePresent.Event -= triggerPresentAni;
		TitleManager.Ins.TitleDismissing.Event -= presentToGame;
		TitleManager.Ins.OptionsPresent.Event -= presentToOptions;
		TitleManager.Ins.OptionsDismissing.Event -= dismissFromOptions;
	}

	private void triggerPresentAni()
	{
		presentAni.delay = 0f;
		presentAni.DOPlay();
	}

	private void presentToGame()
	{
		DOTween.To(() => base.transform.rotation, delegate(Quaternion x)
		{
			base.transform.rotation = x;
		}, new Vector3(90f, 0f, 0f), 1.5f).SetEase(Ease.Linear);
		DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-0.0009f, 2.63f, -0.00081f), 2.4f).SetEase(Ease.InQuint);
	}

	private void presentToOptions()
	{
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			TitleManager.Ins.OptionsPresented.Execute();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-0.0009f, 2.117f, defaultPOS.z), 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0.6f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(-0.0009f, 2.117f, 0.64f), 0.75f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void dismissFromOptions()
	{
		Sequence sequence = DOTween.Sequence().OnComplete(delegate
		{
			TitleManager.Ins.OptionsDismissed.Execute();
		});
		sequence.Insert(0f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, new Vector3(defaultPOS.z, 2.117f, defaultPOS.z), 0.75f).SetEase(Ease.Linear));
		sequence.Insert(0.6f, DOTween.To(() => base.transform.position, delegate(Vector3 x)
		{
			base.transform.position = x;
		}, defaultPOS, 0.75f).SetEase(Ease.Linear));
		sequence.Play();
	}
}
