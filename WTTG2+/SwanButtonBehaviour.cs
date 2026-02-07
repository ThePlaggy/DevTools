using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwanButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField]
	private Text myTextHook;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (TheSwanBehaviour.Ins.Staged && TheSwanBehaviour.Ins.shouldTick && !TheSwanBehaviour.Ins.systemFailure && TheSwanBehaviour.Ins.doomsdayClock <= 60f)
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.swanKeyPress);
			if (myTextHook.text == "EXECUTE")
			{
				TheSwanBehaviour.Ins.EnterCode();
			}
			else if (myTextHook.text == "<")
			{
				TheSwanBehaviour.Ins.DeleteChar();
			}
			else
			{
				TheSwanBehaviour.Ins.SubmitNumber(myTextHook.text);
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}
}
