using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public static class BrowserNative
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ChangeFunc(int browserId, ChangeType changeType, string arg1);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ConsoleFunc(int browserId, string message, string source, int line);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void DisplayDialogFunc(int browserId, DialogType dialogType, string dialogText, string initialPromptText, string sourceURL);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ForwardJSCallFunc(int browserId, int callbackId, string data, int size);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void GetCookieFunc(NativeCookie cookie);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void GetRequestDataFunc(int reqId, IntPtr data, int size);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int GetRequestHeadersFunc(string url, IntPtr mimeType, out int size, out int responseCode);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void MessageFunc(string message);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate NewWindowAction NewWindowFunc(int browserId, IntPtr newURL, bool userInvoked, int possibleBrowserId, ref ZFBSettings possibleSettings);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ReadyFunc(int browserId);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void ShowContextMenuFunc(int browserId, string menuJSON, int x, int y, ContextMenuOrigin origin);

	public enum ChangeType
	{
		CHT_CURSOR,
		CHT_BROWSER_CLOSE,
		CHT_FETCH_FINISHED,
		CHT_FETCH_FAILED,
		CHT_LOAD_FINISHED,
		CHT_CERT_ERROR,
		CHT_SAD_TAB
	}

	[Flags]
	public enum ContextMenuOrigin
	{
		Editable = 2,
		Image = 4,
		Selection = 8,
		Other = 1
	}

	public enum CookieAction
	{
		Delete,
		Create
	}

	public enum CursorType
	{
		Pointer,
		Cross,
		Hand,
		IBeam,
		Wait,
		Help,
		EastResize,
		NorthResize,
		NorthEastResize,
		NorthWestResize,
		SouthResize,
		SouthEastResize,
		SouthWestResize,
		WestResize,
		NorthSouthResize,
		EastWestResize,
		NorthEastSouthWestResize,
		NorthWestSouthEastResize,
		ColumnResize,
		RowResize,
		MiddlePanning,
		EastPanning,
		NorthPanning,
		NorthEastPanning,
		NorthWestPanning,
		SouthPanning,
		SouthEastPanning,
		SouthWestPanning,
		WestPanning,
		Move,
		VerticalText,
		Cell,
		ContextMenu,
		Alias,
		Progress,
		NoDrop,
		Copy,
		None,
		NotAllowed,
		ZoomIn,
		ZoomOut,
		Grab,
		Grabbing,
		Custom
	}

	public enum DialogType
	{
		DLT_HIDE,
		DLT_ALERT,
		DLT_CONFIRM,
		DLT_PROMPT,
		DLT_PAGE_UNLOAD,
		DLT_PAGE_RELOAD,
		DLT_GET_AUTH
	}

	public enum FrameCommand
	{
		Undo,
		Redo,
		Cut,
		Copy,
		Paste,
		Delete,
		SelectAll,
		ViewSource
	}

	public enum LoadChange
	{
		LC_STOP = 1,
		LC_RELOAD,
		LC_FORCE_RELOAD
	}

	public enum MouseButton
	{
		MBT_LEFT,
		MBT_MIDDLE,
		MBT_RIGHT
	}

	public enum NewWindowAction
	{
		NWA_IGNORE = 1,
		NWA_REDIRECT,
		NWA_NEW_BROWSER,
		NWA_NEW_WINDOW
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ZFBInitialSettings
	{
		public string cefPath;

		public string localePath;

		public string subprocessFile;

		public string userAgent;

		public string logFile;

		public int debugPort;

		public int multiThreadedMessageLoop;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ZFBSettings
	{
		public int bgR;

		public int bgG;

		public int bgB;

		public int bgA;

		public int offscreen;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct RenderData
	{
		public IntPtr pixels;

		public int w;

		public int h;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class NativeCookie
	{
		public string creation;

		public string domain;

		public string expires;

		public byte httpOnly;

		public string lastAccess;

		public string name;

		public string path;

		public byte secure;

		public string value;
	}

	public const int DebugPort = 9849;

	public static List<string> commandLineSwitches = new List<string> { "--enable-system-flash" };

	private static Dictionary<int, WebResources.Response> requests = new Dictionary<int, WebResources.Response>();

	private static int nextRequestId = 1;

	public static List<StandaloneWebResources> resourceURLs = new List<StandaloneWebResources>();

	public static bool NativeLoaded { get; private set; }

	public static string LocalUrlPrefix => "http://game.local/";

	private static void LogCallback(string message)
	{
		Debug.Log("ZFWeb: " + message);
	}

	public static void LoadNative()
	{
		if (NativeLoaded)
		{
			return;
		}
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_root"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_vanilla"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_wttg1"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_nasko222"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_kotzwurst"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_fierce"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_otrex"));
		resourceURLs.Add(new StandaloneWebResources(Application.dataPath + "/Resources/websites_others"));
		for (int i = 0; i < resourceURLs.Count; i++)
		{
			resourceURLs[i].LoadIndex();
		}
		int debugPort = (Debug.isDebugBuild ? 9849 : 0);
		FileLocations.CEFDirs dirs = FileLocations.Dirs;
		string fullName = Directory.GetParent(Application.dataPath).FullName;
		Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + fullName);
		StandaloneShutdown.Create();
		zfb_destroyAllBrowsers();
		GetRequestHeadersFunc headerFunc = HeaderCallback;
		GetRequestDataFunc dataFunc = DataCallback;
		zfb_localRequestFuncs(headerFunc, dataFunc);
		zfb_setCallbacksEnabled(enabled: true);
		ZFBInitialSettings settings = new ZFBInitialSettings
		{
			cefPath = dirs.resourcesPath,
			localePath = dirs.localesPath,
			subprocessFile = dirs.subprocessFile,
			userAgent = UserAgent.GetUserAgent(),
			logFile = dirs.logFile,
			debugPort = debugPort,
			multiThreadedMessageLoop = 1
		};
		foreach (string commandLineSwitch in commandLineSwitches)
		{
			zfb_addCLISwitch(commandLineSwitch);
		}
		if (!zfb_init(settings))
		{
			throw new Exception("Failed to initialize browser system.");
		}
		NativeLoaded = true;
		AppDomain.CurrentDomain.DomainUnload += delegate
		{
			zfb_destroyAllBrowsers();
			zfb_setCallbacksEnabled(enabled: false);
		};
	}

	private static void FixProcessPermissions(FileLocations.CEFDirs dirs)
	{
		uint attributes = (uint)File.GetAttributes(dirs.subprocessFile);
		attributes |= 0x80000000u;
		File.SetAttributes(dirs.subprocessFile, (FileAttributes)attributes);
	}

	private static int HeaderCallback(string url, IntPtr mimeTypeDest, out int size, out int responseCode)
	{
		if (url.SafeStartsWith(LocalUrlPrefix))
		{
			url = "/" + url.Substring(LocalUrlPrefix.Length);
		}
		WebResources.Response value = new WebResources.Response
		{
			data = Encoding.UTF8.GetBytes("No WebResources handler!"),
			mimeType = "text/plain",
			responseCode = 500
		};
		for (int i = 0; i < resourceURLs.Count; i++)
		{
			value = resourceURLs[i][url];
			if (value.responseCode != 404)
			{
				i = 9999;
			}
		}
		if (value.responseCode == 404)
		{
			Debug.LogWarning("WebResources: File not found fetching " + WWW.UnEscapeURL(url));
		}
		byte[] array = Encoding.UTF8.GetBytes(value.mimeType);
		if (array.Length > 99)
		{
			Debug.LogWarning("mime type is too long " + value.mimeType);
			array = new byte[0];
		}
		Marshal.Copy(array, 0, mimeTypeDest, array.Length);
		Marshal.WriteByte(mimeTypeDest, array.Length, 0);
		responseCode = value.responseCode;
		size = value.data.Length;
		int num;
		lock (requests)
		{
			num = nextRequestId++;
			requests[num] = value;
		}
		return num;
	}

	private static void DataCallback(int reqId, IntPtr data, int size)
	{
		WebResources.Response value;
		lock (requests)
		{
			if (!requests.TryGetValue(reqId, out value))
			{
				value = new WebResources.Response
				{
					data = Encoding.UTF8.GetBytes("No response for request!"),
					mimeType = "text/plain",
					responseCode = 500
				};
			}
			requests.Remove(reqId);
		}
		if (size != 0)
		{
			Marshal.Copy(value.data, 0, data, size);
		}
	}

	public static void UnloadNative()
	{
		if (NativeLoaded)
		{
			Debug.Log("Stop CEF");
			zfb_setCallbacksEnabled(enabled: false);
			zfb_shutdown();
			NativeLoaded = false;
		}
	}

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_noop();

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_free(IntPtr memory);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_memcpy(IntPtr dst, IntPtr src, int size);

	[DllImport("ZFEmbedWeb")]
	public static extern IntPtr zfb_getVersion();

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setDebugFunc(MessageFunc debugFunc);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_localRequestFuncs(GetRequestHeadersFunc headerFunc, GetRequestDataFunc dataFunc);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setCallbacksEnabled(bool enabled);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_destroyAllBrowsers();

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_addCLISwitch(string value);

	[DllImport("ZFEmbedWeb")]
	public static extern bool zfb_init(ZFBInitialSettings settings);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_shutdown();

	[DllImport("ZFEmbedWeb")]
	public static extern int zfb_createBrowser(ZFBSettings settings);

	[DllImport("ZFEmbedWeb")]
	public static extern int zfb_numBrowsers();

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_destoryBrowser(int id);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_tick();

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setReadyCallback(int id, ReadyFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_resize(int id, int w, int h);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setOverlay(int browserId, int overlayBrowserId);

	[DllImport("ZFEmbedWeb")]
	public static extern RenderData zfb_getImage(int id, bool forceDirty);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_goToURL(int id, string url, bool force);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_goToHTML(int id, string html, string url);

	[DllImport("ZFEmbedWeb")]
	public static extern IntPtr zfb_getURL(int id);

	[DllImport("ZFEmbedWeb")]
	public static extern bool zfb_canNav(int id, int direction);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_doNav(int id, int direction);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setZoom(int id, double zoom);

	[DllImport("ZFEmbedWeb")]
	public static extern bool zfb_isLoading(int id);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_changeLoading(int id, LoadChange what);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_showDevTools(int id, bool show);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_setFocused(int id, bool focused);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_mouseMove(int id, float x, float y);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_mouseButton(int id, MouseButton button, bool down, int clickCount);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_mouseScroll(int id, int deltaX, int deltaY);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_keyEvent(int id, bool down, int windowsKeyCode);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_characterEvent(int id, int character, int windowsKeyCode);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerConsoleCallback(int id, ConsoleFunc callback);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_evalJS(int id, string script, string scriptURL);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerJSCallback(int id, ForwardJSCallFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerChangeCallback(int id, ChangeFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern CursorType zfb_getMouseCursor(int id, out int width, out int height);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_getMouseCustomCursor(int id, IntPtr buffer, int width, int height, out int hotX, out int hotY);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerDialogCallback(int id, DisplayDialogFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_sendDialogResults(int id, bool affirmed, string text1, string text2);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerPopupCallback(int id, NewWindowFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_registerContextMenuCallback(int id, ShowContextMenuFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_sendContextMenuResults(int id, int commandId);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_sendCommandToFocusedFrame(int id, FrameCommand command);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_getCookies(int id, GetCookieFunc cb);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_editCookie(int id, NativeCookie cookie, CookieAction action);

	[DllImport("ZFEmbedWeb")]
	public static extern void zfb_clearCookies(int id);
}
