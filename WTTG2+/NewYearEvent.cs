using UnityEngine;
using UnityEngine.UI;

public class NewYearEvent : MonoBehaviour
{
	private AudioPodObj AudioPod;

	private bool eventActive;

	private bool staged;

	private GameObject TopClock1;

	private GameObject TopClock2;

	private void Start()
	{
		if (EventSlinger.NewYearsEvent)
		{
			GameManager.TimeKeeper.UpdateClockEvents.Event += updateClock;
			StageEvent();
		}
	}

	private void FireEvent()
	{
		if (!eventActive)
		{
			if (AudioPod == null)
			{
				Init();
			}
			KitchenWindowHook.Ins.OpenWindow();
			GameManager.TimeSlinger.FireTimer(150f, delegate
			{
				GameManager.TimeKeeper.UpdateClockEvents.Event -= updateClock;
				eventActive = false;
			});
			eventActive = true;
			AudioPod.MyAudioHubObject.PlaySound(CustomSoundLookUp.FireworksMain);
		}
	}

	private void StageEvent()
	{
		Debug.Log("[ASoftDLL] New Years event spawned");
		if (!staged)
		{
			staged = true;
			Object.Instantiate(CustomObjectLookUp.fireworkBox).transform.position = new Vector3(-5.413f, 39.275f, 5.586f);
			if (AudioPod == null)
			{
				Init();
			}
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick1);
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick2);
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick3);
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick4);
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick5);
			AudioPod.MagicFireSFXS.Add(CustomSoundLookUp.FireworksQuick6);
			AudioPod.MagicFire = true;
			AudioPod.MagicFireWindowMin = 30f;
			AudioPod.MagicFireWindowMax = 40f;
			AudioPod.GenerateMagicFire();
			GameManager.TimeSlinger.FireTimer(Random.Range(40, 70), PlayLongFireworks);
		}
	}

	private void updateClock(string ClockValue)
	{
		if (ClockValue == "12:00 AM")
		{
			FireEvent();
		}
		if (eventActive)
		{
			TopClock1.GetComponent<Text>().text = "HAPPY NEW YEAR";
			TopClock2.GetComponent<Text>().text = "HAPPY NEW YEAR";
		}
	}

	private void PlayLongFireworks()
	{
		AudioPod.MyAudioHubObject.PlaySound(CustomSoundLookUp.FireworksLong1);
		GameManager.TimeSlinger.FireTimer(Random.Range(100, 180), PlayLongFireworks);
	}

	private void Init()
	{
		AudioPod = GameObject.Find("Randoms4").AddComponent<AudioPodObj>();
		TopClock1 = GameObject.Find("topClock1");
		TopClock2 = GameObject.Find("topClock2");
	}
}
