using UnityEngine;
using UnityEngine.UI;

public class WallpaperUtils : MonoBehaviour
{
	public static WallpaperUtils Ins;

	public static bool Staged;

	public static Image desktopWallpaper;

	public static Image theAdos;

	private Image adosImage;

	private bool wallpaperActive;

	private Image wallpaperImage;

	private void Awake()
	{
		Ins = this;
		stageMe();
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	private void stageMe()
	{
		if (Staged)
		{
			Debug.Log("[WARNING] I was already staged, Why are you trying to stage me again??");
			return;
		}
		Staged = true;
		LoadADOS();
	}

	public void ApplyWallpaperSprite(Sprite texture)
	{
		wallpaperImage.sprite = texture;
		wallpaperImage.color = Color.white;
		adosImage.gameObject.SetActive(value: false);
		wallpaperActive = true;
	}

	private void LoadADOS()
	{
		wallpaperImage = desktopWallpaper;
		adosImage = theAdos;
	}

	public static void FindADOS()
	{
		Image[] array = Object.FindObjectsOfType<Image>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].gameObject.name == "WallPaper")
			{
				desktopWallpaper = array[i];
			}
			if (array[i].gameObject.name == "ADOS")
			{
				theAdos = array[i];
			}
		}
	}
}
