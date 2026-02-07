using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

internal class BrowserInput
{
	private class ButtonHistory
	{
		public Vector3 lastPosition;

		public float lastPressTime;

		public int repeatCount;

		public void ButtonPress(Vector3 mousePos, IBrowserUI uiHandler, Vector2 browserSize)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - lastPressTime > uiHandler.InputSettings.multiclickSpeed)
			{
				repeatCount = 0;
			}
			if (repeatCount > 0)
			{
				Vector2 a = Vector2.Scale(mousePos, browserSize);
				Vector2 b = Vector2.Scale(lastPosition, browserSize);
				if (Vector2.Distance(a, b) > uiHandler.InputSettings.multiclickTolerance)
				{
					repeatCount = 0;
				}
			}
			repeatCount++;
			lastPressTime = realtimeSinceStartup;
			lastPosition = mousePos;
		}
	}

	private readonly Browser browser;

	private readonly ButtonHistory leftClickHistory = new ButtonHistory();

	private bool kbWasFocused;

	private bool mouseWasFocused;

	private MouseButton prevButtons;

	private Vector2 prevPos;

	public BrowserInput(Browser browser)
	{
		this.browser = browser;
	}

	public void HandleInput()
	{
		browser.UIHandler.InputUpdate();
		if (browser.UIHandler.MouseHasFocus || mouseWasFocused)
		{
			HandleMouseInput();
		}
		mouseWasFocused = browser.UIHandler.MouseHasFocus;
		if (browser.UIHandler.KeyboardHasFocus)
		{
			if (!kbWasFocused)
			{
				BrowserNative.zfb_setFocused(browser.browserId, kbWasFocused = true);
			}
			HandleKeyInput();
		}
		else if (kbWasFocused)
		{
			BrowserNative.zfb_setFocused(browser.browserId, kbWasFocused = false);
		}
	}

	private void HandleMouseInput()
	{
		IBrowserUI uIHandler = browser.UIHandler;
		Vector2 mousePosition = uIHandler.MousePosition;
		MouseButton mouseButtons = uIHandler.MouseButtons;
		Vector2 mouseScroll = uIHandler.MouseScroll;
		if (mousePosition != prevPos)
		{
			BrowserNative.zfb_mouseMove(browser.browserId, mousePosition.x, 1f - mousePosition.y);
		}
		if (mouseScroll.sqrMagnitude != 0f)
		{
			BrowserNative.zfb_mouseScroll(browser.browserId, (int)mouseScroll.x * uIHandler.InputSettings.scrollSpeed, (int)mouseScroll.y * uIHandler.InputSettings.scrollSpeed);
		}
		bool flag = (prevButtons & MouseButton.Left) != (mouseButtons & MouseButton.Left);
		bool flag2 = (mouseButtons & MouseButton.Left) == MouseButton.Left;
		bool flag3 = (prevButtons & MouseButton.Middle) != (mouseButtons & MouseButton.Middle);
		bool down = (mouseButtons & MouseButton.Middle) == MouseButton.Middle;
		bool flag4 = (prevButtons & MouseButton.Right) != (mouseButtons & MouseButton.Right);
		bool down2 = (mouseButtons & MouseButton.Right) == MouseButton.Right;
		if (flag)
		{
			if (flag2)
			{
				leftClickHistory.ButtonPress(mousePosition, uIHandler, browser.Size);
			}
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_LEFT, flag2, flag2 ? leftClickHistory.repeatCount : 0);
		}
		if (flag3)
		{
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_MIDDLE, down, 1);
		}
		if (flag4)
		{
			BrowserNative.zfb_mouseButton(browser.browserId, BrowserNative.MouseButton.MBT_RIGHT, down2, 1);
		}
		prevPos = mousePosition;
		prevButtons = mouseButtons;
	}

	private void HandleKeyInput()
	{
		List<Event> keyEvents = browser.UIHandler.KeyEvents;
		if (keyEvents.Count == 0)
		{
			return;
		}
		foreach (Event item in keyEvents)
		{
			int windowsKeyCode = KeyMappings.GetWindowsKeyCode(item);
			if (item.character == '\n')
			{
				item.character = '\r';
			}
			if (item.character != 0 && item.type == EventType.KeyDown)
			{
				BrowserNative.zfb_characterEvent(browser.browserId, item.character, windowsKeyCode);
			}
			else
			{
				BrowserNative.zfb_keyEvent(browser.browserId, item.type == EventType.KeyDown, windowsKeyCode);
			}
		}
	}
}
