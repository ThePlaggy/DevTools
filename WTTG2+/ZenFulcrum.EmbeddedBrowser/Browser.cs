using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class Browser : MonoBehaviour
{
	public delegate void JSCallback(JSONNode args);

	[Flags]
	public enum NewWindowAction
	{
		Ignore = 1,
		Redirect = 2,
		NewBrowser = 3,
		NewWindow = 4
	}

	protected delegate void JSResultFunc(JSONNode value, bool isError);

	private static int lastUpdateFrame;

	protected static Dictionary<int, List<object>> allThingsToRemember = new Dictionary<int, List<object>>();

	[Tooltip("Initial URL to load.\n\nTo change at runtime use browser.Url to load a page.")]
	[SerializeField]
	private string _url = string.Empty;

	[Tooltip("Initial size.\n\nTo change at runtime use browser.Resize.")]
	[SerializeField]
	private int _width = 512;

	[Tooltip("Initial size.\n\nTo change at runtime use browser.Resize.")]
	[SerializeField]
	private int _height = 512;

	[Tooltip("Generate mipmaps?\r\n\r\nGenerating mipmaps tends to be somewhat expensive, especially when updating a large texture every frame. Instead of\r\ngenerating mipmaps, try using one of the \"emulate mipmap\" shader variants.\r\n\r\nTo change at runtime modify this value and call browser.Resize.")]
	public bool generateMipmap;

	[Tooltip("Initial background color to use for pages.\r\nYou may pick any opaque color. As a special case, if alpha == 0 the entire background will be rendered transparent.\r\n(Be sure to use a transparent-capable shader to see it.)\r\n\r\nThis value cannot be changed after the first InputUpdate() tick, create a new browser if you need a different option.")]
	public Color32 backgroundColor = new Color32(0, 0, 0, 0);

	[Tooltip("Initial browser \"zoom level\". Negative numbers are smaller, zero is normal, positive numbers are larger.\r\nThe size roughly doubles/halves for every four units added/removed.\r\nNote that zoom level is shared by all pages on the some domain.\r\nAlso note that this zoom level may be persisted across runs.\r\n\r\nTo change at runtime use browser.Zoom.")]
	[SerializeField]
	private float _zoom;

	[Tooltip("Allow right-clicking to show a context menu on what parts of the page?\r\n\r\nMay be changed at any time.\r\n")]
	[FlagsField]
	public BrowserNative.ContextMenuOrigin allowContextMenuOn = BrowserNative.ContextMenuOrigin.Editable;

	[Tooltip("What should we do when a user/the page tries to open a new window?\r\n\r\nFor \"New Browser\" to work, you need to assign NewWindowHandler to a handler of your creation.\r\n")]
	public NewWindowAction newWindowAction = NewWindowAction.Redirect;

	protected IBrowserUI _uiHandler;

	protected internal int browserId;

	protected bool browserIdRequested;

	private BrowserInput browserInput;

	protected DialogHandler dialogHandler;

	protected bool forceNextRender = true;

	private bool loadPending;

	private int nextCallbackId = 1;

	protected List<Action> onloadActions = new List<Action>();

	private Browser overlay;

	private Action pageReplacer;

	private float pageReplacerPriority;

	private Color32[] pixelData;

	protected Dictionary<int, JSResultFunc> registeredCallbacks = new Dictionary<int, JSResultFunc>();

	private bool skipNextLoad;

	protected Texture2D texture;

	protected bool textureIsOurs;

	protected List<Action> thingsToDo = new List<Action>();

	protected List<Action> thingsToDoClone = new List<Action>();

	protected List<object> thingsToRemember = new List<object>();

	protected bool uiHandlerAssigned;

	private int unsafeBrowserId;

	public static string LocalUrlPrefix => BrowserNative.LocalUrlPrefix;

	public IBrowserUI UIHandler
	{
		get
		{
			return _uiHandler;
		}
		set
		{
			uiHandlerAssigned = true;
			_uiHandler = value;
		}
	}

	public INewWindowHandler NewWindowHandler { get; set; }

	public bool EnableRendering { get; set; }

	public bool EnableInput { get; set; }

	public CookieManager CookieManager { get; private set; }

	public Texture2D Texture => texture;

	public bool IsReady => browserId != 0;

	public string Url
	{
		get
		{
			if (!IsReady)
			{
				return string.Empty;
			}
			CheckSanity();
			IntPtr intPtr = BrowserNative.zfb_getURL(browserId);
			string result = Marshal.PtrToStringAnsi(intPtr);
			BrowserNative.zfb_free(intPtr);
			return result;
		}
		set
		{
			LoadURL(value, force: true);
		}
	}

	public bool CanGoBack
	{
		get
		{
			if (!IsReady)
			{
				return false;
			}
			CheckSanity();
			return BrowserNative.zfb_canNav(browserId, -1);
		}
	}

	public bool CanGoForward
	{
		get
		{
			if (!IsReady)
			{
				return false;
			}
			CheckSanity();
			return BrowserNative.zfb_canNav(browserId, 1);
		}
	}

	public bool IsLoadingRaw => IsReady && BrowserNative.zfb_isLoading(browserId);

	public bool IsLoaded
	{
		get
		{
			if (!IsReady || loadPending || BrowserNative.zfb_isLoading(browserId))
			{
				return false;
			}
			string url = Url;
			return !string.IsNullOrEmpty(url) && !(url == "about:blank");
		}
	}

	public Vector2 Size => new Vector2(_width, _height);

	public float Zoom
	{
		get
		{
			return _zoom;
		}
		set
		{
			if (!DeferUnready(delegate
			{
				Zoom = value;
			}))
			{
				BrowserNative.zfb_setZoom(browserId, value);
				_zoom = value;
			}
		}
	}

	public event Action<string, string> onConsoleMessage = delegate
	{
	};

	public event Action<Texture2D> afterResize = delegate
	{
	};

	protected event BrowserNative.ReadyFunc onNativeReady;

	public event Action<JSONNode> onLoad = delegate
	{
	};

	public event Action<JSONNode> onFetch = delegate
	{
	};

	public event Action<JSONNode> onFetchError = delegate
	{
	};

	public event Action<JSONNode> onCertError = delegate
	{
	};

	public event Action onSadTab = delegate
	{
	};

	protected void Awake()
	{
		EnableRendering = true;
		EnableInput = true;
		CookieManager = new CookieManager(this);
		browserInput = new BrowserInput(this);
		onNativeReady += delegate
		{
			if (!uiHandlerAssigned)
			{
				MeshCollider component = GetComponent<MeshCollider>();
				if ((bool)component)
				{
					UIHandler = ClickMeshBrowserUI.Create(component);
				}
			}
			Resize(_width, _height);
			Zoom = _zoom;
			if (!string.IsNullOrEmpty(_url))
			{
				Url = _url;
			}
		};
		onConsoleMessage += delegate(string message, string source)
		{
			Debug.Log(source + ": " + message, this);
		};
		onFetchError += delegate(JSONNode err)
		{
			if (!(err["error"] == "ERR_ABORTED"))
			{
				QueuePageReplacer(delegate
				{
					LoadHTML(Resources.Load<TextAsset>("Browser/Errors").text, Url);
					CallFunction("setErrorInfo", err);
				}, -1000f);
			}
		};
		onCertError += delegate(JSONNode err)
		{
			QueuePageReplacer(delegate
			{
				LoadHTML(Resources.Load<TextAsset>("Browser/Errors").text, Url);
				CallFunction("setErrorInfo", err);
			}, -900f);
		};
		onSadTab += delegate
		{
			QueuePageReplacer(delegate
			{
				LoadHTML(Resources.Load<TextAsset>("Browser/Errors").text, Url);
				CallFunction("showCrash");
			}, -1000f);
		};
	}

	protected void Update()
	{
		ProcessCallbacks();
		if (browserId == 0)
		{
			RequestNativeBrowser();
			return;
		}
		CheckSanity();
		HandleInput();
	}

	protected void LateUpdate()
	{
		if (lastUpdateFrame != Time.frameCount)
		{
			BrowserNative.zfb_tick();
			lastUpdateFrame = Time.frameCount;
		}
		ProcessCallbacks();
		if (pageReplacer != null)
		{
			pageReplacer();
			pageReplacer = null;
		}
		if (browserId != 0 && EnableRendering)
		{
			Render();
		}
	}

	protected void OnDisable()
	{
	}

	protected void OnDestroy()
	{
		if (browserId != 0)
		{
			if ((bool)dialogHandler)
			{
				UnityEngine.Object.DestroyImmediate(dialogHandler.gameObject);
			}
			dialogHandler = null;
			BrowserNative.zfb_destoryBrowser(browserId);
			if (textureIsOurs)
			{
				UnityEngine.Object.Destroy(texture);
			}
			browserId = 0;
			texture = null;
		}
	}

	protected void OnApplicationQuit()
	{
		OnDestroy();
		if (BrowserNative.zfb_numBrowsers() == 0)
		{
			for (int i = 0; i < 10; i++)
			{
				BrowserNative.zfb_tick();
				Thread.Sleep(10);
			}
			BrowserNative.UnloadNative();
		}
	}

	public void WhenReady(Action callback)
	{
		if (IsReady)
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(callback);
				return;
			}
		}
		BrowserNative.ReadyFunc func = null;
		func = delegate
		{
			callback();
			onNativeReady -= func;
		};
		onNativeReady += func;
	}

	public void RunOnMainThread(Action callback)
	{
		lock (thingsToDo)
		{
			thingsToDo.Add(callback);
		}
	}

	public void WhenLoaded(Action callback)
	{
		onloadActions.Add(callback);
	}

	private void RequestNativeBrowser(int newBrowserId = 0)
	{
		if (browserId != 0 || browserIdRequested)
		{
			return;
		}
		browserIdRequested = true;
		try
		{
			BrowserNative.LoadNative();
		}
		catch
		{
			base.gameObject.SetActive(value: false);
			throw;
		}
		int newId;
		if (newBrowserId == 0)
		{
			newId = BrowserNative.zfb_createBrowser(new BrowserNative.ZFBSettings
			{
				bgR = backgroundColor.r,
				bgG = backgroundColor.g,
				bgB = backgroundColor.b,
				bgA = backgroundColor.a,
				offscreen = 1
			});
		}
		else
		{
			newId = newBrowserId;
		}
		unsafeBrowserId = newId;
		lock (allThingsToRemember)
		{
			allThingsToRemember[newId] = thingsToRemember;
		}
		BrowserNative.ForwardJSCallFunc forwardJSCallFunc = delegate(int bId, int id, string data, int size)
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(delegate
				{
					if (!registeredCallbacks.TryGetValue(id, out var value))
					{
						Debug.LogWarning("Got a JS callback for event " + id + ", but no such event is registered.");
						return;
					}
					bool isError = false;
					if (data.StartsWith("fail-"))
					{
						isError = true;
						data = data.Substring(5);
					}
					JSONNode value2;
					try
					{
						value2 = JSONNode.Parse(data);
					}
					catch (SerializationException)
					{
						Debug.LogWarning("Invalid JSON sent from browser: " + data);
						return;
					}
					try
					{
						value(value2, isError);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				});
			}
		};
		thingsToRemember.Add(forwardJSCallFunc);
		BrowserNative.zfb_registerJSCallback(newId, forwardJSCallFunc);
		BrowserNative.ChangeFunc changeFunc = delegate(int id, BrowserNative.ChangeType type, string arg1)
		{
			if (type == BrowserNative.ChangeType.CHT_BROWSER_CLOSE)
			{
				if ((bool)this)
				{
					lock (thingsToDo)
					{
						thingsToDo.Add(delegate
						{
							UnityEngine.Object.Destroy(base.gameObject);
						});
					}
				}
				lock (allThingsToRemember)
				{
					allThingsToRemember.Remove(unsafeBrowserId);
				}
				browserId = 0;
			}
			else if ((bool)this)
			{
				lock (thingsToDo)
				{
					thingsToDo.Add(delegate
					{
						OnItemChange(type, arg1);
					});
				}
			}
		};
		thingsToRemember.Add(changeFunc);
		BrowserNative.zfb_registerChangeCallback(newId, changeFunc);
		BrowserNative.DisplayDialogFunc displayDialogFunc = delegate(int id, BrowserNative.DialogType type, string text, string promptText, string url)
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(delegate
				{
					CreateDialogHandler();
					dialogHandler.HandleDialog(type, text, promptText);
				});
			}
		};
		thingsToRemember.Add(displayDialogFunc);
		BrowserNative.zfb_registerDialogCallback(newId, displayDialogFunc);
		BrowserNative.ShowContextMenuFunc showContextMenuFunc = delegate(int id, string json, int x, int y, BrowserNative.ContextMenuOrigin origin)
		{
			if (json != null && (allowContextMenuOn & origin) == 0)
			{
				BrowserNative.zfb_sendContextMenuResults(browserId, -1);
				return;
			}
			lock (thingsToDo)
			{
				thingsToDo.Add(delegate
				{
					if (json != null)
					{
						CreateDialogHandler();
					}
					if (dialogHandler != null)
					{
						dialogHandler.HandleContextMenu(json, x, y);
					}
				});
			}
		};
		thingsToRemember.Add(showContextMenuFunc);
		BrowserNative.zfb_registerContextMenuCallback(newId, showContextMenuFunc);
		BrowserNative.NewWindowFunc newWindowFunc = delegate(int id, IntPtr urlPtr, bool userInvoked, int possibleId, ref BrowserNative.ZFBSettings possibleSettings)
		{
			if (userInvoked)
			{
				switch (newWindowAction)
				{
				case NewWindowAction.Redirect:
					return BrowserNative.NewWindowAction.NWA_REDIRECT;
				case NewWindowAction.NewBrowser:
					if (NewWindowHandler != null)
					{
						possibleSettings.bgR = backgroundColor.r;
						possibleSettings.bgG = backgroundColor.g;
						possibleSettings.bgB = backgroundColor.b;
						possibleSettings.bgA = backgroundColor.a;
						lock (thingsToDo)
						{
							thingsToDo.Add(delegate
							{
								NewWindowHandler.CreateBrowser(this).RequestNativeBrowser(possibleId);
							});
							return BrowserNative.NewWindowAction.NWA_NEW_BROWSER;
						}
					}
					Debug.LogError("Missing NewWindowHandler, can't open new window", this);
					return BrowserNative.NewWindowAction.NWA_IGNORE;
				case NewWindowAction.NewWindow:
					return BrowserNative.NewWindowAction.NWA_NEW_WINDOW;
				default:
					return BrowserNative.NewWindowAction.NWA_IGNORE;
				}
			}
			return BrowserNative.NewWindowAction.NWA_IGNORE;
		};
		thingsToRemember.Add(newWindowFunc);
		BrowserNative.zfb_registerPopupCallback(newId, newWindowFunc);
		BrowserNative.ConsoleFunc consoleFunc = delegate(int id, string message, string source, int line)
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(delegate
				{
					this.onConsoleMessage(message, source + ":" + line);
				});
			}
		};
		thingsToRemember.Add(consoleFunc);
		BrowserNative.zfb_registerConsoleCallback(newId, consoleFunc);
		BrowserNative.ReadyFunc readyFunc = delegate
		{
			lock (thingsToDo)
			{
				thingsToDo.Add(delegate
				{
					browserId = newId;
					this.onNativeReady(browserId);
				});
			}
		};
		thingsToRemember.Add(readyFunc);
		BrowserNative.zfb_setReadyCallback(newId, readyFunc);
	}

	protected void OnItemChange(BrowserNative.ChangeType type, string arg1)
	{
		switch (type)
		{
		case BrowserNative.ChangeType.CHT_CURSOR:
			UpdateCursor();
			break;
		case BrowserNative.ChangeType.CHT_FETCH_FINISHED:
			this.onFetch(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_FETCH_FAILED:
			this.onFetchError(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_LOAD_FINISHED:
			if (skipNextLoad)
			{
				skipNextLoad = false;
				break;
			}
			loadPending = false;
			if (onloadActions.Count != 0)
			{
				foreach (Action onloadAction in onloadActions)
				{
					onloadAction();
				}
				onloadActions.Clear();
			}
			this.onLoad(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_CERT_ERROR:
			this.onCertError(JSONNode.Parse(arg1));
			break;
		case BrowserNative.ChangeType.CHT_SAD_TAB:
			this.onSadTab();
			break;
		case BrowserNative.ChangeType.CHT_BROWSER_CLOSE:
			break;
		}
	}

	protected void CreateDialogHandler()
	{
		if (!(dialogHandler != null))
		{
			dialogHandler = DialogHandler.Create(this, delegate(bool affirm, string text1, string text2)
			{
				CheckSanity();
				BrowserNative.zfb_sendDialogResults(browserId, affirm, text1, text2);
			}, delegate(int commandId)
			{
				CheckSanity();
				BrowserNative.zfb_sendContextMenuResults(browserId, commandId);
			});
		}
	}

	protected void CheckSanity()
	{
		if (browserId == 0)
		{
			throw new InvalidOperationException("No native browser allocated");
		}
	}

	internal bool DeferUnready(Action ifNotReady)
	{
		if (browserId == 0)
		{
			WhenReady(ifNotReady);
			return true;
		}
		CheckSanity();
		return false;
	}

	public void LoadURL(string url, bool force)
	{
		if (!DeferUnready(delegate
		{
			LoadURL(url, force);
		}))
		{
			if (url.StartsWith("localGame://"))
			{
				url = LocalUrlPrefix + url.Substring("localGame://".Length);
			}
			if (string.IsNullOrEmpty(url))
			{
				throw new ArgumentException("URL must be non-empty", "value");
			}
			loadPending = true;
			BrowserNative.zfb_goToURL(browserId, url, force);
		}
	}

	public void LoadHTML(string html, string url = null)
	{
		if (!DeferUnready(delegate
		{
			LoadHTML(html, url);
		}))
		{
			loadPending = true;
			if (string.IsNullOrEmpty(url))
			{
				url = LocalUrlPrefix + "custom";
			}
			if (string.IsNullOrEmpty(Url))
			{
				Url = "about:blank";
				skipNextLoad = true;
			}
			BrowserNative.zfb_goToHTML(browserId, html, url);
		}
	}

	public void SendFrameCommand(BrowserNative.FrameCommand command)
	{
		if (!DeferUnready(delegate
		{
			SendFrameCommand(command);
		}))
		{
			BrowserNative.zfb_sendCommandToFocusedFrame(browserId, command);
		}
	}

	public void QueuePageReplacer(Action replacePage, float priority)
	{
		if (pageReplacer == null || !((double)priority < (double)pageReplacerPriority))
		{
			pageReplacer = replacePage;
			pageReplacerPriority = priority;
		}
	}

	public void GoBack()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_doNav(browserId, -1);
		}
	}

	public void GoForward()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_doNav(browserId, 1);
		}
	}

	public void Stop()
	{
		if (IsReady)
		{
			CheckSanity();
			BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_STOP);
		}
	}

	public void Reload(bool force = false)
	{
		if (IsReady)
		{
			CheckSanity();
			if (force)
			{
				BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_FORCE_RELOAD);
			}
			else
			{
				BrowserNative.zfb_changeLoading(browserId, BrowserNative.LoadChange.LC_RELOAD);
			}
		}
	}

	public void ShowDevTools(bool show = true)
	{
		if (!DeferUnready(delegate
		{
			ShowDevTools(show);
		}))
		{
			BrowserNative.zfb_showDevTools(browserId, show);
		}
	}

	protected void _Resize(Texture2D newTexture, bool newTextureIsOurs)
	{
		int width = newTexture.width;
		int height = newTexture.height;
		if (textureIsOurs && (bool)texture && newTexture != texture)
		{
			UnityEngine.Object.Destroy(texture);
		}
		_width = width;
		_height = height;
		if (IsReady)
		{
			BrowserNative.zfb_resize(browserId, width, height);
		}
		else
		{
			WhenReady(delegate
			{
				BrowserNative.zfb_resize(browserId, width, height);
			});
		}
		texture = newTexture;
		textureIsOurs = newTextureIsOurs;
		Renderer component = GetComponent<Renderer>();
		if ((bool)component)
		{
			component.material.mainTexture = texture;
		}
		this.afterResize(texture);
		if ((bool)overlay)
		{
			overlay.Resize(Texture);
		}
	}

	public void Resize(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, generateMipmap);
		if (generateMipmap)
		{
			texture2D.filterMode = FilterMode.Trilinear;
		}
		texture2D.wrapMode = TextureWrapMode.Clamp;
		_Resize(texture2D, newTextureIsOurs: true);
	}

	public void Resize(Texture2D newTexture)
	{
		_Resize(newTexture, newTextureIsOurs: false);
	}

	public IPromise<JSONNode> EvalJS(string script, string scriptURL = "scripted command")
	{
		Promise<JSONNode> promise = new Promise<JSONNode>();
		int id = nextCallbackId++;
		string asJSON = new JSONNode(script).AsJSON;
		string resultJS = "try {_zfb_event(" + id + ", JSON.stringify(eval(" + asJSON + " )) || 'null');} catch(ex) {_zfb_event(" + id + ", 'fail-' + (JSON.stringify(ex.stack) || 'null'));}";
		registeredCallbacks.Add(id, delegate(JSONNode val, bool isError)
		{
			registeredCallbacks.Remove(id);
			if (isError)
			{
				promise.Reject(new JSException(val));
			}
			else
			{
				promise.Resolve(val);
			}
		});
		if (!IsLoaded)
		{
			WhenLoaded(delegate
			{
				_EvalJS(resultJS, scriptURL);
			});
		}
		else
		{
			_EvalJS(resultJS, scriptURL);
		}
		return promise;
	}

	protected void _EvalJS(string script, string scriptURL)
	{
		BrowserNative.zfb_evalJS(browserId, script, scriptURL);
	}

	public IPromise<JSONNode> CallFunction(string name, params JSONNode[] arguments)
	{
		string text = name + "(";
		string text2 = string.Empty;
		foreach (JSONNode jSONNode in arguments)
		{
			text = text + text2 + jSONNode.AsJSON;
			text2 = ", ";
		}
		text += ");";
		return EvalJS(text);
	}

	public void RegisterFunction(string name, JSCallback callback)
	{
		int key = nextCallbackId++;
		registeredCallbacks.Add(key, delegate(JSONNode value, bool error)
		{
			callback(value);
		});
		EvalJS(name + " = function() { _zfb_event(" + key + ", JSON.stringify(Array.prototype.slice.call(arguments))); };");
	}

	protected void ProcessCallbacks()
	{
		while (thingsToDo.Count != 0)
		{
			lock (thingsToDo)
			{
				thingsToDoClone.AddRange(thingsToDo);
				thingsToDo.Clear();
			}
			foreach (Action item in thingsToDoClone)
			{
				item();
			}
			thingsToDoClone.Clear();
		}
	}

	protected void Render()
	{
		CheckSanity();
		BrowserNative.RenderData renderData = BrowserNative.zfb_getImage(browserId, forceNextRender);
		forceNextRender = false;
		if (!(renderData.pixels == IntPtr.Zero) && renderData.w == texture.width && renderData.h == texture.height)
		{
			if (pixelData == null || pixelData.Length != renderData.w * renderData.h)
			{
				pixelData = new Color32[renderData.w * renderData.h];
			}
			GCHandle gCHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
			BrowserNative.zfb_memcpy(gCHandle.AddrOfPinnedObject(), renderData.pixels, pixelData.Length * 4);
			gCHandle.Free();
			texture.SetPixels32(pixelData);
			texture.Apply(updateMipmaps: true);
		}
	}

	public void SetOverlay(Browser overlayBrowser)
	{
		if (!DeferUnready(delegate
		{
			SetOverlay(overlayBrowser);
		}) && (!overlayBrowser || !overlayBrowser.DeferUnready(delegate
		{
			SetOverlay(overlayBrowser);
		})))
		{
			BrowserNative.zfb_setOverlay(browserId, overlayBrowser ? overlayBrowser.browserId : 0);
			overlay = overlayBrowser;
			if ((bool)overlay && (!overlay.Texture || overlay.Texture.width != Texture.width || overlay.Texture.height != Texture.height))
			{
				overlay.Resize(Texture);
			}
		}
	}

	protected void HandleInput()
	{
		if (_uiHandler != null && EnableInput)
		{
			CheckSanity();
			browserInput.HandleInput();
		}
	}

	public void UpdateCursor()
	{
		if (UIHandler != null && !DeferUnready(UpdateCursor))
		{
			int width;
			int height;
			BrowserNative.CursorType cursorType = BrowserNative.zfb_getMouseCursor(browserId, out width, out height);
			if (cursorType != BrowserNative.CursorType.Custom)
			{
				UIHandler.BrowserCursor.SetActiveCursor(cursorType);
				return;
			}
			if (width == 0 && height == 0)
			{
				UIHandler.BrowserCursor.SetActiveCursor(BrowserNative.CursorType.None);
				return;
			}
			Color32[] array = new Color32[width * height];
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			BrowserNative.zfb_getMouseCustomCursor(browserId, gCHandle.AddrOfPinnedObject(), width, height, out var hotX, out var hotY);
			gCHandle.Free();
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, mipChain: false);
			texture2D.SetPixels32(array);
			UIHandler.BrowserCursor.SetCustomCursor(texture2D, new Vector2(hotX, hotY));
			UnityEngine.Object.DestroyImmediate(texture2D);
		}
	}
}
