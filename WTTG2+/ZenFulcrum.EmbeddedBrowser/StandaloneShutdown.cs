using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

internal class StandaloneShutdown : MonoBehaviour
{
	public void OnApplicationQuit()
	{
		BrowserNative.UnloadNative();
	}

	public static void Create()
	{
		GameObject gameObject = new GameObject("ZFB Shutdown");
		gameObject.AddComponent<StandaloneShutdown>();
		Object.DontDestroyOnLoad(gameObject);
	}
}
