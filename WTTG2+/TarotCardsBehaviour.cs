using UnityEngine;

public class TarotCardsBehaviour : MonoBehaviour
{
	public static TarotCardsBehaviour Ins;

	public static bool Owned;

	public static bool CantRefillNow;

	public static bool CardsOnTable;

	public InteractionHook myInteractionHook;

	public AudioHubObject myAudioHub;

	public MeshRenderer[] myMeshRenderer;

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= leftClickAction;
		Debug.Log("Tarot Card Module Unloaded!");
	}

	public void ShowInteractionIcon()
	{
		UIInteractionManager.Ins.ShowHoldMode();
		UIInteractionManager.Ins.ShowLeftMouseButtonAction();
	}

	public void HideInteractionIcon()
	{
		UIInteractionManager.Ins.HideHoldMode();
		UIInteractionManager.Ins.HideLeftMouseButtonAction();
	}

	public void SoftBuild()
	{
		Ins = this;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		for (int i = 0; i < myMeshRenderer.Length; i++)
		{
			myMeshRenderer[i].enabled = false;
		}
		myInteractionHook.LeftClickAction += leftClickAction;
	}

	private void leftClickAction()
	{
		CantRefillNow = true;
		myInteractionHook.ForceLock = true;
		TarotCardPullAnim.Ins.DoPull();
		UIInteractionManager.Ins.HideHoldMode();
		UIInteractionManager.Ins.HideLeftMouseButtonAction();
		GameManager.TimeSlinger.FireTimer(2f, ReEnableLeftClick);
	}

	public void MoveMe(Vector3 SetPOS, Vector3 SetROT, Vector3 SetSCL)
	{
		Owned = true;
		CardsOnTable = true;
		for (int i = 0; i < myMeshRenderer.Length; i++)
		{
			myMeshRenderer[i].enabled = true;
		}
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
		base.transform.localScale = SetSCL;
	}

	private void ReEnableLeftClick()
	{
		if (TarotCardPullAnim.currentCard < 10)
		{
			myInteractionHook.ForceLock = false;
		}
		else
		{
			CardsOnTable = false;
		}
		CantRefillNow = false;
	}
}
