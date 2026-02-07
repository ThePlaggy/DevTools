using DG.Tweening;
using TMPro;
using UnityEngine;

public class SidebarBTN : MonoBehaviour
{
	public TMP_Text text;

	public Color hoverColor;

	public GameObject parent;

	private Color normalColor;

	private void Awake()
	{
		normalColor = text.color;
	}

	public void OnHover()
	{
		text.color = hoverColor;
	}

	public void OnUnHover()
	{
		text.color = normalColor;
	}

	public void SetActive()
	{
		DOTween.To(() => parent.transform.localPosition.x, delegate(float x)
		{
			parent.transform.localPosition = new Vector3(x, parent.transform.localPosition.y, parent.transform.localPosition.z);
		}, -542f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
		});
	}

	public void SetInactive()
	{
		DOTween.To(() => parent.transform.localPosition.x, delegate(float x)
		{
			parent.transform.localPosition = new Vector3(x, parent.transform.localPosition.y, parent.transform.localPosition.z);
		}, -537f, 0.25f).SetEase(Ease.Linear).OnComplete(delegate
		{
		});
	}
}
