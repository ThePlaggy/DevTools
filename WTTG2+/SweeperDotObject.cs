using DG.Tweening;
using UnityEngine;

public class SweeperDotObject : MonoBehaviour
{
	public delegate void UpdateHotSpotActions();

	private const float DELAY_TIME = 0.015f;

	private const float FADE_TIME = 0.2f;

	private const float HOT_SPOT_DELAY_TIME = 0.1f;

	private const float HOT_SPOT_FADE_TIME = 0.1f;

	public GameObject MyHotSpot;

	private float fireDelay;

	private bool fireHotSpot;

	private float fireTimeStamp;

	private bool iAmHot;

	private CanvasGroup myCG;

	private CanvasGroup myHotSpotCG;

	private int myIndex;

	private RectTransform myRT;

	private Tweener showHotSpotSeq;

	private Tweener showMeSeq;

	public event UpdateHotSpotActions ActivateHotSpot;

	private void Awake()
	{
		myCG = GetComponent<CanvasGroup>();
		myHotSpotCG = MyHotSpot.GetComponent<CanvasGroup>();
		myRT = GetComponent<RectTransform>();
		showMeSeq = DOTween.To(() => myCG.alpha, delegate(float x)
		{
			myCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear).SetDelay(0.015f);
		showMeSeq.Pause();
		showMeSeq.SetAutoKill(autoKillOnCompletion: false);
		showHotSpotSeq = DOTween.To(() => myHotSpotCG.alpha, delegate(float x)
		{
			myHotSpotCG.alpha = x;
		}, 1f, 0.1f).SetEase(Ease.Linear).SetDelay(0.1f);
		showHotSpotSeq.Pause();
		showHotSpotSeq.SetAutoKill(autoKillOnCompletion: false);
	}

	private void Update()
	{
		if (fireHotSpot && Time.time - fireTimeStamp >= fireDelay)
		{
			fireHotSpot = false;
			if (this.ActivateHotSpot != null)
			{
				this.ActivateHotSpot();
			}
		}
	}

	public void BuildMe(int setIndex, int maxIndex)
	{
		myIndex = setIndex;
		showMeSeq.Restart(includeDelay: true, (float)setIndex * 0.015f);
		if (setIndex == maxIndex)
		{
			fireDelay = (float)setIndex * 0.015f;
			fireTimeStamp = Time.time;
			fireHotSpot = true;
		}
	}

	public void MakeMeHot()
	{
		iAmHot = true;
	}

	public void ActivateMyHotSpot(int setIndex)
	{
		if (iAmHot)
		{
			showHotSpotSeq.Restart(includeDelay: true, (float)setIndex * 0.1f);
		}
	}

	public bool GetAmHot()
	{
		return iAmHot;
	}

	public void Destroy()
	{
		myCG.alpha = 0f;
		myHotSpotCG.alpha = 0f;
		iAmHot = false;
		myRT.anchoredPosition = Vector2.zero;
		fireHotSpot = false;
	}
}
