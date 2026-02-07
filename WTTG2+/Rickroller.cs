using UnityEngine;

public class Rickroller : MonoBehaviour
{
	[SerializeField]
	private Texture2D xydxdy;

	public static bool VACation { get; set; }

	private void Start()
	{
		if (EventSlinger.AprilFoolsEvent)
		{
			bool flag = false;
			WallpaperUtils.Ins.ApplyWallpaperSprite(CustomSpriteLookUp.LOLpaper);
			GameManager.AudioSlinger.PlaySound(VACation ? CustomSoundLookUp.TheRealVacation : CustomSoundLookUp.getcalculated);
			LOBOTOMYDASH(VACation ? CustomSpriteLookUp.TheRealVacation : CustomSpriteLookUp.rickastleymaboi);
		}
	}

	public static void LOBOTOMYDASH(Texture2D NI_)
	{
		MeshRenderer[] array = Object.FindObjectsOfType<MeshRenderer>();
		MeshRenderer[] array2 = array;
		foreach (MeshRenderer meshRenderer in array2)
		{
			if (meshRenderer.material != null && meshRenderer.material.HasProperty("_MainTex"))
			{
				meshRenderer.material.SetTexture("_MainTex", NI_);
			}
		}
	}
}
