using UnityEngine;
using UnityEngine.EventSystems;

public class MemCoreObject : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler
{
	public delegate void CoreActions();

	public GameObject KeyText;

	public GameObject RefreshIMG;

	public AudioFileDefinition MemCoreShowSFX;

	public AudioFileDefinition MemCoreHideSFX;

	public AudioFileDefinition KeyFlashSFX;

	public event CoreActions KeyWasPresented;

	public event CoreActions CoreWasShown;

	public event CoreActions KeysWereFlashed;

	public void OnPointerEnter(PointerEventData eventData)
	{
	}

	public void OnPointerExit(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
