using UnityEngine;

public class TutorialStartBehaviour : TutorialStepper
{
	public static TutorialStartBehaviour Ins;

	[SerializeField]
	private GameObject exampleHashObject;

	[SerializeField]
	private GameObject exampleKeyDiscovery;

	[SerializeField]
	private CanvasGroup noirIconCG;

	[SerializeField]
	private CanvasGroup[] docIconCGs = new CanvasGroup[0];

	private void Awake()
	{
		Ins = this;
	}

	public void InitializeDesktop()
	{
		bool flag = !DifficultyManager.HackerMode && !DifficultyManager.LeetMode && !DifficultyManager.Nightmare;
		for (int i = 0; i < docIconCGs.Length; i++)
		{
			docIconCGs[i].gameObject.SetActive(flag);
			docIconCGs[i].alpha = (flag ? 1f : 0f);
		}
		noirIconCG.gameObject.SetActive(value: true);
		noirIconCG.alpha = 1f;
	}
}
