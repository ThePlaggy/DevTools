using UnityEngine;
using UnityEngine.UI;

public class TennantTrackFloorSelectObject : MonoBehaviour
{
	public bool Active;

	[SerializeField]
	private Image activeBGImage;

	[SerializeField]
	private Text floorNumberText;

	[SerializeField]
	private Color activeColorText;

	[SerializeField]
	private Color idleColorText;

	[SerializeField]
	private int myFloorNumber;

	public int FloorNumber => myFloorNumber;

	public void ActivateMe()
	{
		Active = true;
		activeBGImage.enabled = true;
		floorNumberText.color = activeColorText;
	}

	public void DeActivateMe()
	{
		Active = false;
		activeBGImage.enabled = false;
		floorNumberText.color = idleColorText;
	}
}
