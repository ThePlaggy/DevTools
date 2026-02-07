using UnityEngine;

public class TitleLookUp : MonoBehaviour
{
	public static TitleLookUp Ins;

	public AudioFileDefinition PresentSubMenuSFX;

	public AudioFileDefinition TitleMenuHoverSFX;

	public AudioFileDefinition TitleMenuClickSFX;

	private void Awake()
	{
		Ins = this;
	}
}
