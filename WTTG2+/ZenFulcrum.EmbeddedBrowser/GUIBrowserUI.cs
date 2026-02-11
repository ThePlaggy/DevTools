using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZenFulcrum.EmbeddedBrowser
{

	[RequireComponent(typeof(RawImage))]
	[RequireComponent(typeof(Browser))]
	public class GUIBrowserUI : MonoBehaviour, IBrowserUI, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		public bool enableInput = true;

		protected bool _keyboardHasFocus;

		protected bool _mouseHasFocus;

		protected Browser browser;

		protected List<Event> keyEvents = new List<Event>();

		protected List<Event> keyEventsLast = new List<Event>();

		protected RawImage myImage;

		protected BaseRaycaster raycaster;

		protected RectTransform rTransform;

		public bool MouseHasFocus => _mouseHasFocus && enableInput;

		public Vector2 MousePosition { get; private set; }

		public MouseButton MouseButtons { get; private set; }

		public Vector2 MouseScroll { get; private set; }

		public bool KeyboardHasFocus => _keyboardHasFocus && enableInput;

		public List<Event> KeyEvents => keyEventsLast;

		public BrowserCursor BrowserCursor { get; private set; }

		public BrowserInputSettings InputSettings { get; private set; }

		protected void Awake()
		{
			BrowserCursor = new BrowserCursor();
			InputSettings = new BrowserInputSettings();
			browser = GetComponent<Browser>();
			myImage = GetComponent<RawImage>();
			browser.afterResize += UpdateTexture;
			browser.UIHandler = this;
			BrowserCursor.cursorChange += delegate
			{
				SetCursor(BrowserCursor);
			};
			rTransform = GetComponent<RectTransform>();
		}

		protected void OnEnable()
		{
			StartCoroutine(WatchResize());
		}

		public void OnGUI()
		{
			Event current = Event.current;
			if (current.type == EventType.KeyDown || current.type == EventType.KeyUp)
			{
				keyEvents.Add(new Event(current));
			}
		}

		public virtual void InputUpdate()
		{
			List<Event> list = keyEvents;
			keyEvents = keyEventsLast;
			keyEventsLast = list;
			keyEvents.Clear();
			if (MouseHasFocus)
			{
				if (!raycaster)
				{
					raycaster = GetComponentInParent<BaseRaycaster>();
				}
				RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)base.transform, Input.mousePosition, raycaster.eventCamera, out var localPoint);
				localPoint.x = localPoint.x / rTransform.rect.width + 0.5f;
				localPoint.y = localPoint.y / rTransform.rect.height + 0.5f;
				MousePosition = localPoint;
				MouseButton mouseButton = (MouseButton)0;
				if (Input.GetMouseButton(0))
				{
					mouseButton |= MouseButton.Left;
				}
				if (Input.GetMouseButton(1))
				{
					mouseButton |= MouseButton.Right;
				}
				if (Input.GetMouseButton(2))
				{
					mouseButton |= MouseButton.Middle;
				}
				MouseButtons = mouseButton;
				MouseScroll = Input.mouseScrollDelta;
			}
			else
			{
				MouseButtons = (MouseButton)0;
			}
			if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
			{
				keyEventsLast.Insert(0, new Event
				{
					type = EventType.KeyDown,
					keyCode = KeyCode.LeftShift
				});
			}
			if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
			{
				keyEventsLast.Add(new Event
				{
					type = EventType.KeyUp,
					keyCode = KeyCode.LeftShift
				});
			}
		}

		public void OnDeselect(BaseEventData eventData)
		{
			_keyboardHasFocus = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_mouseHasFocus = true;
			SetCursor(BrowserCursor);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_mouseHasFocus = false;
			SetCursor(null);
		}

		public void OnSelect(BaseEventData eventData)
		{
			_keyboardHasFocus = true;
		}

		private IEnumerator WatchResize()
		{
			Rect currentSize = default(Rect);
			while (base.enabled)
			{
				Rect rect = rTransform.rect;
				if (rect.size.x <= 0f || rect.size.y <= 0f)
				{
					rect.size = new Vector2(512f, 512f);
				}
				if (rect.size != currentSize.size)
				{
					browser.Resize((int)rect.size.x, (int)rect.size.y);
					currentSize = rect;
				}
				yield return null;
			}
		}

		protected void UpdateTexture(Texture2D texture)
		{
			myImage.texture = texture;
			myImage.uvRect = new Rect(0f, 0f, 1f, 1f);
		}

		protected virtual void SetCursor(BrowserCursor newCursor)
		{
		}
	}
}