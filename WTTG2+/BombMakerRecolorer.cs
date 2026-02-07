using UnityEngine;

public class BombMakerRecolorer : MonoBehaviour
{
	public static Material chosenMat;

	public Material defaultMat;

	public Material recolorMat;

	private void Awake()
	{
		if (DifficultyManager.Nightmare)
		{
			Debug.Log("[BombMakerRecolorer] - Red Color");
			chosenMat = recolorMat;
		}
		else
		{
			Debug.Log("[BombMakerRecolorer] - Blue Color");
			chosenMat = defaultMat;
		}
		Object.Destroy(base.gameObject);
	}
}
