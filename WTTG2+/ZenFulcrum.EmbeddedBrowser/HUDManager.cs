using System;
using System.Collections;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class HUDManager : MonoBehaviour
{
	public GUIBrowserUI hud;

	private bool haveMouse;

	public static HUDManager Instance { get; private set; }

	public Browser HUDBrowser { get; private set; }

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		HUDBrowser = hud.GetComponent<Browser>();
		HUDBrowser.RegisterFunction("unpause", delegate
		{
			Unpause();
		});
		HUDBrowser.RegisterFunction("browserMode", delegate
		{
			LoadBrowseLevel(force: true);
		});
		HUDBrowser.RegisterFunction("quit", delegate
		{
			Application.Quit();
		});
		Unpause();
		PlayerInventory.Instance.coinCollected += delegate(int count)
		{
			HUDBrowser.CallFunction("setCoinCount", count);
		};
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (haveMouse)
			{
				Pause();
			}
			else
			{
				Unpause();
			}
		}
	}

	private IEnumerator Rehide()
	{
		while (Application.isShowingSplashScreen)
		{
			yield return null;
		}
		Cursor.visible = false;
		yield return new WaitForSeconds(0.2f);
		Cursor.visible = true;
		yield return new WaitForSeconds(0.2f);
		Cursor.visible = false;
	}

	public void Unpause()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		EnableUserControls(enableIt: true);
		Time.timeScale = 1f;
		haveMouse = true;
		HUDBrowser.CallFunction("setPaused", false);
	}

	public void Pause()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		haveMouse = false;
		Time.timeScale = 0f;
		EnableUserControls(enableIt: false);
		HUDBrowser.CallFunction("setPaused", true);
	}

	public void Say(string html, float dwellTime)
	{
		HUDBrowser.CallFunction("say", html, dwellTime);
	}

	protected void EnableUserControls(bool enableIt)
	{
		FPSCursorRenderer.Instance.EnableInput = enableIt;
		MouseLook[] componentsInChildren = GetComponentsInChildren<MouseLook>();
		foreach (MouseLook mouseLook in componentsInChildren)
		{
			mouseLook.enabled = enableIt;
		}
		MouseLook[] componentsInChildren2 = GetComponentsInChildren<MouseLook>();
		foreach (MouseLook mouseLook2 in componentsInChildren2)
		{
			mouseLook2.enabled = enableIt;
		}
		Behaviour behaviour = (Behaviour)GetComponentInChildren(Type.GetType("FPSInputController, Assembly-UnityScript"));
		behaviour.enabled = enableIt;
		hud.enableInput = !enableIt;
	}

	public void LoadBrowseLevel(bool force = false)
	{
		StartCoroutine(LoadLevel(force));
	}

	private IEnumerator LoadLevel(bool force = false)
	{
		if (!force)
		{
			yield return new WaitUntil(() => SayWordsOnTouch.ActiveSpeakers == 0);
		}
		Pause();
		Application.LoadLevel("SimpleBrowser");
	}
}
