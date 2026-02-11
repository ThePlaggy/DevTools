using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	public class ClickMeshBrowserUI : MonoBehaviour, IBrowserUI
	{
		private static readonly KeyCode[] keysToCheck = new KeyCode[2]
		{
		KeyCode.LeftShift,
		KeyCode.RightShift
		};

		[HideInInspector]
		public float maxDistance = float.PositiveInfinity;

		protected List<Event> keyEvents = new List<Event>();

		protected List<Event> keyEventsLast = new List<Event>();

		protected MeshCollider meshCollider;

		protected bool mouseWasOver;

		protected virtual Ray LookRay => Camera.main.ScreenPointToRay(Input.mousePosition);

		public bool MouseHasFocus { get; protected set; }

		public Vector2 MousePosition { get; protected set; }

		public MouseButton MouseButtons { get; protected set; }

		public Vector2 MouseScroll { get; protected set; }

		public bool KeyboardHasFocus { get; protected set; }

		public List<Event> KeyEvents => keyEventsLast;

		public BrowserCursor BrowserCursor { get; protected set; }

		public BrowserInputSettings InputSettings { get; protected set; }

		public void Awake()
		{
			BrowserCursor = new BrowserCursor();
			BrowserCursor.cursorChange += CursorUpdated;
			InputSettings = new BrowserInputSettings();
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
			Ray lookRay = LookRay;
			Physics.Raycast(lookRay, out var hitInfo, maxDistance);
			if (hitInfo.transform != meshCollider.transform)
			{
				MousePosition = new Vector3(0f, 0f);
				MouseButtons = (MouseButton)0;
				MouseScroll = new Vector2(0f, 0f);
				MouseHasFocus = false;
				KeyboardHasFocus = false;
				LookOff();
				return;
			}
			LookOn();
			MouseHasFocus = true;
			KeyboardHasFocus = true;
			Vector2 textureCoord = hitInfo.textureCoord;
			MousePosition = textureCoord;
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
			for (int i = 0; i < keysToCheck.Length; i++)
			{
				if (Input.GetKeyDown(keysToCheck[i]))
				{
					keyEventsLast.Insert(0, new Event
					{
						type = EventType.KeyDown,
						keyCode = keysToCheck[i]
					});
				}
				else if (Input.GetKeyUp(keysToCheck[i]))
				{
					keyEventsLast.Add(new Event
					{
						type = EventType.KeyUp,
						keyCode = keysToCheck[i]
					});
				}
			}
		}

		public static ClickMeshBrowserUI Create(MeshCollider meshCollider)
		{
			ClickMeshBrowserUI clickMeshBrowserUI = meshCollider.gameObject.AddComponent<ClickMeshBrowserUI>();
			clickMeshBrowserUI.meshCollider = meshCollider;
			return clickMeshBrowserUI;
		}

		protected void LookOn()
		{
			if (BrowserCursor != null)
			{
				CursorUpdated();
			}
			mouseWasOver = true;
		}

		protected void LookOff()
		{
			if (BrowserCursor != null && mouseWasOver)
			{
				SetCursor(null);
			}
			mouseWasOver = false;
		}

		protected void CursorUpdated()
		{
			SetCursor(BrowserCursor);
		}

		protected virtual void SetCursor(BrowserCursor newCursor)
		{
			if (MouseHasFocus || newCursor == null)
			{
				if (newCursor == null)
				{
					Cursor.visible = true;
					Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				}
				else if (newCursor.Texture != null)
				{
					Cursor.visible = true;
					Cursor.SetCursor(newCursor.Texture, newCursor.Hotspot, CursorMode.Auto);
				}
				else
				{
					Cursor.visible = false;
					Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
				}
			}
		}
	}
}