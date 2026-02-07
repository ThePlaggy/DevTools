using UnityEngine;
using UnityEngine.UI;

public class DeadOrNotChatObject : MonoBehaviour
{
	[SerializeField]
	private Text userNameText;

	[SerializeField]
	private Text chatText;

	[SerializeField]
	private RectTransform chatRT;

	[SerializeField]
	private Font chatFont;

	private RectTransform myRT;

	public float Height { get; private set; }

	public float CurrentY => myRT.anchoredPosition.y;
}
