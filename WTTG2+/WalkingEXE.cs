using DG.Tweening;
using UnityEngine;

public class WalkingEXE : MonoBehaviour
{
	public static float WALK_TIME = 2.8f;

	public static bool WIDE = false;

	private void Awake()
	{
		base.transform.position = new Vector3(0f, 39.55f, -6.2436f);
		if (WIDE)
		{
			base.transform.localScale = new Vector3(3f, 1f, 1f);
		}
		base.transform.DOMoveX(-4.5f, WALK_TIME).SetEase(Ease.Linear).OnComplete(delegate
		{
			Object.Destroy(base.gameObject);
		});
	}

	public static void stageMe()
	{
		Object.Instantiate(CustomObjectLookUp.ExecutionerPrefab).AddComponent<WalkingEXE>().GetComponent<Animator>()
			.SetBool("walking", value: true);
	}
}
