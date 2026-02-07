using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class FPSCursorRenderer : MonoBehaviour
{
	private static FPSCursorRenderer _instance;

	[Tooltip("How large should we render the cursor?")]
	public float scale = 0.5f;

	[Tooltip("How far can we reach to push buttons and such?")]
	public float maxDistance = 7f;

	[Tooltip("What are we using to point at things? Leave as null to use Camera.main")]
	public Transform pointer;

	protected BrowserCursor baseCursor;

	protected BrowserCursor currentCursor;

	public static FPSCursorRenderer Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = Object.FindObjectOfType<FPSCursorRenderer>();
				if (!_instance)
				{
					GameObject gameObject = new GameObject("Cursor Crosshair");
					_instance = gameObject.AddComponent<FPSCursorRenderer>();
				}
			}
			return _instance;
		}
	}

	public bool EnableInput { get; set; }

	public void Start()
	{
		EnableInput = true;
		baseCursor = new BrowserCursor();
		baseCursor.SetActiveCursor(BrowserNative.CursorType.Cross);
	}

	public void OnGUI()
	{
		if (EnableInput)
		{
			BrowserCursor browserCursor = currentCursor ?? baseCursor;
			Texture2D texture = browserCursor.Texture;
			if (!(texture == null))
			{
				Rect position = new Rect((float)Screen.width / 2f, (float)Screen.height / 2f, (float)texture.width * scale, (float)texture.height * scale);
				position.x -= browserCursor.Hotspot.x * scale;
				position.y -= browserCursor.Hotspot.y * scale;
				GUI.DrawTexture(position, texture);
			}
		}
	}

	public static void SetUpBrowserInput(Browser browser, MeshCollider mesh)
	{
		FPSCursorRenderer instance = Instance;
		Transform transform = instance.pointer;
		if (!transform)
		{
			transform = Camera.main.transform;
		}
		FPSBrowserUI fPSBrowserUI = FPSBrowserUI.Create(mesh, transform, instance);
		fPSBrowserUI.maxDistance = instance.maxDistance;
		browser.UIHandler = fPSBrowserUI;
	}

	public void SetCursor(BrowserCursor newCursor, FPSBrowserUI ui)
	{
		currentCursor = newCursor;
	}
}
