using UnityEngine;
using UnityEngine.UI;

public class DeadOrNotBehaviour : MonoBehaviour
{
	public int CHAT_LINE_OBJECT_POOL_COUNT = 60;

	[SerializeField]
	private Text viewersText;

	[SerializeField]
	private GameObject prepareText;

	[SerializeField]
	private CanvasGroup pleaseWaitCG;

	[SerializeField]
	private CanvasGroup castYourVotesTextCG;

	[SerializeField]
	private RectTransform castYourVotesHolderRT;

	[SerializeField]
	private Text deathVoteCountText;

	[SerializeField]
	private Text lifeVoteCountText;

	[SerializeField]
	private RectTransform chatHolderRT;

	[SerializeField]
	private RectTransform chatContentHolderRT;

	[SerializeField]
	private GameObject chatLineObject;

	[SerializeField]
	private string[] staticChatLines = new string[0];
}
