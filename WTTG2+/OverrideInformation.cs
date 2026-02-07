using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class OverrideInformation : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public OverrideInformationWindow oiw;

	private bool active;

	private bool locked;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (locked)
		{
			return;
		}
		if (!active)
		{
			locked = true;
			active = true;
			oiw.MyTextCG.DOFade(1f, 0.75f);
			oiw.MyBGCG.DOFade(0.5f, 0.75f);
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				locked = false;
			});
		}
		else
		{
			locked = true;
			active = false;
			oiw.MyTextCG.DOFade(0f, 0.75f);
			oiw.MyBGCG.DOFade(0f, 0.75f);
			GameManager.TimeSlinger.FireTimer(1f, delegate
			{
				locked = false;
			});
		}
	}

	private void Start()
	{
		oiw.MyTextCG.alpha = 0f;
		oiw.MyBGCG.alpha = 0f;
		string n = (DifficultyManager.Nightmare ? "OverrideInfoNM(Clone)" : "OverrideInfo(Clone)");
		base.transform.Find(n).SetParent(GameObject.Find("DesktopUI").transform);
	}

	private void FixedUpdate()
	{
		oiw.UpdateMe(SpeedPoll.speedManipulatorData.ToString().ToLower(), ((int)(GameManager.TheCloud.speedFireWindow - (Time.time - GameManager.TheCloud.speedTimeStamp))).ToString(), KeyPoll.keyManipulatorData.ToString().ToLower(), ((int)(GameManager.TheCloud.keyFireWindow - (Time.time - GameManager.TheCloud.keyTimeStamp))).ToString(), SpeedPoll.speedManipulatorActive && GameManager.TheCloud.speedActive, KeyPoll.keyManipulatorData != KEY_CUE_MODE.DEFAULT && GameManager.TheCloud.keyActive);
	}
}
