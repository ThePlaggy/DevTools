using UnityEngine;
using UnityEngine.UI;

public class TutorialBTN : MonoBehaviour
{
	[SerializeField]
	private Image bgImage;

	[SerializeField]
	private Sprite hoverSprite;

	public CustomEvent ClickAction = new CustomEvent(2);
}
