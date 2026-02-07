using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class TrackerManager
{
	public static CanvasGroup TrackIcon;

	private static bool CurrentlyTracked;

	public static bool LocationServicesBought;

	public static void NotifyUserBeingTracked(float seconds = 10f)
	{
		if (CurrentlyTracked)
		{
			return;
		}
		CurrentlyTracked = true;
		Sequence sequence = DOTween.Sequence();
		sequence.Insert(0f, DOTween.To(() => TrackIcon.alpha, delegate(float x)
		{
			TrackIcon.alpha = x;
		}, 1f, 1f).SetEase(Ease.Linear));
		sequence.Play();
		sequence.OnComplete(delegate
		{
			if (LocationServicesBought)
			{
				DoAlert();
			}
		});
		if (seconds <= 0f)
		{
			return;
		}
		GameManager.TimeSlinger.FireTimer(seconds, delegate
		{
			Sequence sequence2 = DOTween.Sequence().OnComplete(delegate
			{
				CurrentlyTracked = false;
			});
			sequence2.Insert(0f, DOTween.To(() => TrackIcon.alpha, delegate(float x)
			{
				TrackIcon.alpha = x;
			}, 0f, 1f).SetEase(Ease.Linear));
			sequence2.Play();
		});
	}

	public static void ClearTrackState()
	{
		TrackIcon.alpha = 0f;
		CurrentlyTracked = false;
	}

	public static void Build()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(GameObject.Find("WifiButton"));
		gameObject.name = "TrackedIcon";
		TrackIcon = gameObject.AddComponent<CanvasGroup>();
		TrackIcon.alpha = 0f;
		gameObject.transform.SetParent(GameObject.Find("UI/UIComputer/DesktopUI/TopBar/TopRightIconHolder").transform);
		gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(-178f, 0f);
		gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(new Vector3(0f, 0f, 5f));
		gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 1f, 1f);
		GameObject.Find("TrackedIcon/wifiIcon").GetComponent<Image>().sprite = CustomSpriteLookUp.locIcon;
		UnityEngine.Object.Destroy(gameObject.GetComponent<MouseClickSoundScrub>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<GraphicsRayCasterCatcher>());
		UnityEngine.Object.Destroy(gameObject.GetComponent<TopMenuIconBehaviour>());
		Debug.Log("[TrackerManager] Built!");
		LocationServicesBought = false;
	}

	public static void DoAlert()
	{
		if (StateManager.BeingHacked)
		{
			GameManager.TimeSlinger.FireTimer(1f, DoAlert);
			return;
		}
		GameManager.TimeSlinger.FireTimer(2.5f, (Action)delegate
		{
			GameObject.Find("TrackedIcon/wifiIcon").GetComponent<Image>().DOColor(new Color(1f, 0.35f, 0.4f, 1f), 0.2f)
				.OnComplete(delegate
				{
					GameObject.Find("TrackedIcon/wifiIcon").GetComponent<Image>().DOColor(Color.white, CustomSoundLookUp.LocationServices.AudioClip.length / 2f);
				});
			if (EnvironmentManager.PowerState == POWER_STATE.ON && !ComputerMuteBehaviour.Ins.Muted && ComputerPowerHook.Ins.PowerOn)
			{
				GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.LocationServices);
			}
		}, 5);
	}
}
