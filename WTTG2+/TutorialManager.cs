using DG.Tweening;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static TutorialManager Ins;

	[SerializeField]
	private GameObject tutorialHolder;

	[SerializeField]
	private CanvasGroup blackGG;

	[SerializeField]
	private RectTransform incomingCallLargeRT;

	[SerializeField]
	private CanvasGroup incomingCallLargeCG;

	[SerializeField]
	private RectTransform adamLargeRT;

	[SerializeField]
	private TutorialBTN incomingCallLargeAcceptBTN;

	[SerializeField]
	private TutorialBTN incomingCallLargeDeclineBTN;

	[SerializeField]
	private AudioFileDefinition phoneRingSFX;

	[SerializeField]
	private AudioFileDefinition acceptCallSFX;

	[SerializeField]
	private AudioFileDefinition declineCallSFX;

	private Tweener adamScaleTween;

	public AudioFileDefinition DeclineCallSFX => declineCallSFX;
}
