using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BinBehaviour : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	private Image myImg;

	public static bool BinMode;

	private const float ANIM_DURATION = 0.5f;

	private Vector3 defaultVec = Vec3(0.8f);

	private Vector3 largeVec = Vec3(1.3f);

	private void Awake()
	{
		myImg = GetComponent<Image>();
		myImg.sprite = CustomSpriteLookUp.binopened;
		base.gameObject.AddComponent<MouseClickSoundScrub>();
		base.gameObject.AddComponent<GraphicsRayCasterCatcher>();
		base.gameObject.AddComponent<QualityPixelPerfectHook>();
		Vector3 position = base.transform.position;
		position.x = 20f;
		base.transform.position = position;
		base.transform.localScale = defaultVec;
		base.transform.SetParent(base.transform.parent.parent);
	}

	private void Start()
	{
		Vector3 position = LookUp.DesktopUI.MIN_WINDOW_TAB_HOLDER.transform.position;
		position.x = 40f;
		LookUp.DesktopUI.MIN_WINDOW_TAB_HOLDER.transform.position = position;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		BinMode = !BinMode;
		float num = (BinMode ? 0.5f : 1f);
		myImg.DOColor(new Color(num, 1f, num, 1f), 0.5f);
		myImg.transform.DOScale(largeVec, 0.25f).OnComplete(delegate
		{
			Tweener tweener = myImg.transform.DOScale(defaultVec, 0.25f);
		});
	}

	private static Vector3 Vec3(float x)
	{
		return new Vector3(x, x, x);
	}
}
