using DG.Tweening;
using UnityEngine;

public class EvilSkullBehavior : MonoBehaviour
{
	public GameObject SkullObject;

	public GameObject SkullTop;

	public GameObject SkullBot;

	public AudioFileDefinition EvilLaugh;

	public AudioFileDefinition SkullInSFX;

	public AudioFileDefinition SkullOutSFX;

	private Vector2 defaultSkullBotPOS;

	private Vector3 defaultSkullScale = Vector3.one;

	private Tweener dismissTween;

	private Sequence hackedLaughSeq;

	private Vector3 hackedSkullSmallScale = new Vector3(0.01f, 0.01f, 1f);

	private Sequence haHaHaSeq;

	private Vector2 laughSkullBotClose = new Vector2(0f, 145f);

	private Vector2 laughSkullBotClose2 = new Vector2(0f, 250f);

	private Vector2 laughSkullBotOpen = new Vector2(0f, -145f);

	private Vector2 laughSkullBotOpen2 = new Vector2(0f, -250f);

	private Vector3 presentSkullScale = new Vector3(0.22f, 0.22f, 1f);

	private Tweener presentSkullTween;

	private RectTransform skullBotRT;

	private Sequence skullLaughSeq;

	private CanvasGroup skullObjectCG;

	private RectTransform skullObjectRT;

	private void Awake()
	{
		skullObjectCG = SkullObject.GetComponent<CanvasGroup>();
		skullBotRT = SkullBot.GetComponent<RectTransform>();
		skullObjectRT = SkullObject.GetComponent<RectTransform>();
		defaultSkullBotPOS = SkullBot.GetComponent<RectTransform>().anchoredPosition;
		presentSkullTween = DOTween.To(() => skullObjectRT.localScale, delegate(Vector3 x)
		{
			skullObjectRT.localScale = x;
		}, presentSkullScale, 0.75f).SetEase(Ease.OutBack).OnComplete(haHaHa);
		presentSkullTween.Pause();
		presentSkullTween.SetAutoKill(autoKillOnCompletion: false);
		dismissTween = DOTween.To(() => skullObjectRT.localScale, delegate(Vector3 x)
		{
			skullObjectRT.localScale = x;
		}, defaultSkullScale, 0.4f).SetEase(Ease.InBack).OnComplete(killMe);
		dismissTween.Pause();
		dismissTween.SetAutoKill(autoKillOnCompletion: false);
		haHaHaSeq = DOTween.Sequence().OnComplete(dismissMe);
		haHaHaSeq.Insert(0f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(0.32f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(0.46f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(0.66f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(0.81f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.02f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.17f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.32f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.44f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.67f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(1.81f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.02f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.12f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.33f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.47f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.67f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(2.82f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(3.06f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(3.18f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Insert(3.72f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		haHaHaSeq.Pause();
		haHaHaSeq.SetAutoKill(autoKillOnCompletion: false);
		skullLaughSeq = DOTween.Sequence();
		skullLaughSeq.Insert(0f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotOpen2, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		skullLaughSeq.Insert(0.1f, DOTween.To(() => skullBotRT.anchoredPosition, delegate(Vector2 x)
		{
			skullBotRT.anchoredPosition = x;
		}, laughSkullBotClose2, 0.1f).SetEase(Ease.Linear).SetRelative(isRelative: true));
		skullLaughSeq.SetLoops(-1);
		skullLaughSeq.Pause();
		skullLaughSeq.SetAutoKill(autoKillOnCompletion: false);
		hackedLaughSeq = DOTween.Sequence().OnComplete(delegate
		{
			skullLaughSeq.Pause();
			skullBotRT.anchoredPosition = defaultSkullBotPOS;
			skullObjectRT.localScale = defaultSkullScale;
		});
		hackedLaughSeq.Insert(0f, DOTween.To(() => skullObjectRT.localScale, delegate(Vector3 x)
		{
			skullObjectRT.localScale = x;
		}, hackedSkullSmallScale, 0.75f).SetEase(Ease.OutCirc));
		hackedLaughSeq.Insert(0.56f, DOTween.To(() => SkullObject.GetComponent<CanvasGroup>().alpha, delegate(float x)
		{
			SkullObject.GetComponent<CanvasGroup>().alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear));
		hackedLaughSeq.Pause();
		hackedLaughSeq.SetAutoKill(autoKillOnCompletion: false);
	}

	public void PresentMe()
	{
		GameManager.AudioSlinger.PlaySound(SkullInSFX);
		BlueWhisperManager.Ins.ProcessSound(SkullInSFX);
		skullObjectCG.alpha = 1f;
		presentSkullTween.Restart();
	}

	public void HackedLaugh()
	{
		skullObjectCG.alpha = 1f;
		skullLaughSeq.Restart();
		hackedLaughSeq.Restart();
	}

	private void haHaHa()
	{
		GameManager.AudioSlinger.PlaySound(EvilLaugh);
		BlueWhisperManager.Ins.ProcessSound(EvilLaugh);
		haHaHaSeq.Restart();
	}

	private void dismissMe()
	{
		GameManager.AudioSlinger.PlaySound(SkullOutSFX);
		BlueWhisperManager.Ins.ProcessSound(SkullOutSFX);
		dismissTween.Restart();
	}

	private void killMe()
	{
		skullObjectCG.alpha = 0f;
		GameManager.HackerManager.PresentHackGame();
	}
}
