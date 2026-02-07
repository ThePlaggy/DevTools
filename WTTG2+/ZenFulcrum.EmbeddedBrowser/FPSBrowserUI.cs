using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(Browser))]
[RequireComponent(typeof(MeshCollider))]
public class FPSBrowserUI : ClickMeshBrowserUI
{
	protected FPSCursorRenderer cursorRenderer;

	protected Transform worldPointer;

	protected override Ray LookRay => new Ray(worldPointer.position, worldPointer.forward);

	public void Start()
	{
		FPSCursorRenderer.SetUpBrowserInput(GetComponent<Browser>(), GetComponent<MeshCollider>());
	}

	public static FPSBrowserUI Create(MeshCollider meshCollider, Transform worldPointer, FPSCursorRenderer cursorRenderer)
	{
		FPSBrowserUI fPSBrowserUI = meshCollider.gameObject.GetComponent<FPSBrowserUI>();
		if (!fPSBrowserUI)
		{
			fPSBrowserUI = meshCollider.gameObject.AddComponent<FPSBrowserUI>();
		}
		fPSBrowserUI.meshCollider = meshCollider;
		fPSBrowserUI.worldPointer = worldPointer;
		fPSBrowserUI.cursorRenderer = cursorRenderer;
		return fPSBrowserUI;
	}

	protected override void SetCursor(BrowserCursor newCursor)
	{
		if (newCursor == null || base.MouseHasFocus)
		{
			cursorRenderer.SetCursor(newCursor, this);
		}
	}

	public override void InputUpdate()
	{
		if (!cursorRenderer.EnableInput)
		{
			base.MouseHasFocus = false;
			base.KeyboardHasFocus = false;
		}
		else
		{
			base.InputUpdate();
		}
	}
}
