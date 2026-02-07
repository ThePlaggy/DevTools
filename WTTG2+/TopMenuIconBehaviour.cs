using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TopMenuIconBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField]
	private RectTransform hoverIMGRT;

	[SerializeField]
	private CanvasGroup hoverIMGCG;

	[SerializeField]
	private UnityEvent clickAction;

	private Vector3 fullScale = Vector3.one;

	private Sequence hideOverSeq;

	private Sequence showHoverSeq;

	private Vector3 smallScale = new Vector3(0.25f, 0.25f, 0.25f);

	private void Awake()
	{
		showHoverSeq = DOTween.Sequence();
		showHoverSeq.Insert(0f, DOTween.To(() => smallScale, delegate(Vector3 x)
		{
			hoverIMGRT.localScale = x;
		}, fullScale, 0.3f).SetEase(Ease.Linear));
		showHoverSeq.Insert(0f, DOTween.To(() => 0f, delegate(float x)
		{
			hoverIMGCG.alpha = x;
		}, 1f, 0.2f).SetEase(Ease.Linear));
		showHoverSeq.Pause();
		showHoverSeq.SetAutoKill(autoKillOnCompletion: false);
		hideOverSeq = DOTween.Sequence();
		hideOverSeq.Insert(0f, DOTween.To(() => fullScale, delegate(Vector3 x)
		{
			hoverIMGRT.localScale = x;
		}, smallScale, 0.2f).SetEase(Ease.Linear));
		hideOverSeq.Insert(0f, DOTween.To(() => 1f, delegate(float x)
		{
			hoverIMGCG.alpha = x;
		}, 0f, 0.2f).SetEase(Ease.Linear));
		hideOverSeq.Pause();
		hideOverSeq.SetAutoKill(autoKillOnCompletion: false);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (clickAction != null)
		{
			clickAction.Invoke();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		showHoverSeq.Restart();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hideOverSeq.Restart();
	}
}
