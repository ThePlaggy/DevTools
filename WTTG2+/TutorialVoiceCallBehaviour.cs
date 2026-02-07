using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TutorialVoiceCallBehaviour : MonoBehaviour
{
	[SerializeField]
	private Text timeText;

	[SerializeField]
	private TutorialBTN hangUpBTN;

	public CustomEvent HangUpEvents = new CustomEvent(2);

	public CustomEvent WasPresentedEvents = new CustomEvent(2);
}
