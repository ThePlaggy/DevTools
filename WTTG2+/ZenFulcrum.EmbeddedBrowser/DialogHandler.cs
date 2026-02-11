using System;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	[RequireComponent(typeof(Browser))]
	public class DialogHandler : MonoBehaviour
	{
		public delegate void DialogCallback(bool affirm, string text1, string text2);

		public delegate void MenuCallback(int commandId);

		protected static string dialogPage;

		protected MenuCallback contextCallback;

		protected Browser dialogBrowser;

		protected DialogCallback dialogCallback;

		protected Browser parentBrowser;

		public static DialogHandler Create(Browser parent, DialogCallback dialogCallback, MenuCallback contextCallback)
		{
			if (dialogPage == null)
			{
				dialogPage = Resources.Load<TextAsset>("Browser/Dialogs").text;
			}
			GameObject gameObject = new GameObject("Browser Dialog for " + parent.name);
			DialogHandler handler = gameObject.AddComponent<DialogHandler>();
			handler.parentBrowser = parent;
			handler.dialogCallback = dialogCallback;
			Browser browser = (handler.dialogBrowser = handler.GetComponent<Browser>());
			browser.UIHandler = parent.UIHandler;
			browser.EnableRendering = false;
			browser.EnableInput = false;
			browser.allowContextMenuOn = BrowserNative.ContextMenuOrigin.Editable;
			browser.Resize(parent.Texture);
			browser.LoadHTML(dialogPage, "about:dialog");
			browser.UIHandler = parent.UIHandler;
			browser.RegisterFunction("reportDialogResult", delegate (JSONNode args)
			{
				dialogCallback(args[0], args[1], args[3]);
				handler.Hide();
			});
			browser.RegisterFunction("reportContextMenuResult", delegate (JSONNode args)
			{
				contextCallback(args[0]);
				handler.Hide();
			});
			return handler;
		}

		public void HandleDialog(BrowserNative.DialogType type, string text, string promptDefault = null)
		{
			if (type == BrowserNative.DialogType.DLT_HIDE)
			{
				Hide();
				return;
			}
			Show();
			switch (type)
			{
				case BrowserNative.DialogType.DLT_ALERT:
					dialogBrowser.CallFunction("showAlert", text);
					break;
				case BrowserNative.DialogType.DLT_CONFIRM:
					dialogBrowser.CallFunction("showConfirm", text);
					break;
				case BrowserNative.DialogType.DLT_PROMPT:
					dialogBrowser.CallFunction("showPrompt", text, promptDefault);
					break;
				case BrowserNative.DialogType.DLT_PAGE_UNLOAD:
					dialogBrowser.CallFunction("showConfirmNav", text);
					break;
				case BrowserNative.DialogType.DLT_PAGE_RELOAD:
					dialogBrowser.CallFunction("showConfirmReload", text);
					break;
				case BrowserNative.DialogType.DLT_GET_AUTH:
					dialogBrowser.CallFunction("showAuthPrompt", text);
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		public void Show()
		{
			parentBrowser.SetOverlay(dialogBrowser);
			parentBrowser.EnableInput = false;
			dialogBrowser.EnableInput = true;
			dialogBrowser.UpdateCursor();
		}

		public void Hide()
		{
			parentBrowser.SetOverlay(null);
			parentBrowser.EnableInput = true;
			dialogBrowser.EnableInput = false;
			parentBrowser.UpdateCursor();
			if (dialogBrowser.IsLoaded)
			{
				dialogBrowser.CallFunction("reset");
			}
		}

		public void HandleContextMenu(string menuJSON, int x, int y)
		{
			if (menuJSON == null)
			{
				Hide();
				return;
			}
			Show();
			dialogBrowser.CallFunction("showContextMenu", menuJSON, x, y);
		}
	}
}