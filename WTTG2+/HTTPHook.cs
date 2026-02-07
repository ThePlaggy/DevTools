using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPHook : MonoBehaviour
{
	private Action<UnityWebRequest> OnResponse;

	private Action<Exception> OnError;

	public void CreateRequest(UnityWebRequest UWR, Action<Exception> Error, Action<UnityWebRequest> Response)
	{
		OnResponse = Response;
		OnError = Error;
		StartCoroutine(SendRequest(UWR));
	}

	private void HandleResponse(UnityWebRequest res)
	{
		OnResponse(res);
	}

	private IEnumerator SendRequest(UnityWebRequest UWR)
	{
		if (UWR.downloadHandler == null)
		{
			UWR.downloadHandler = new DownloadHandlerBuffer();
		}
		yield return UWR.SendWebRequest();
		if (UWR.isHttpError || UWR.isNetworkError)
		{
			OnError?.Invoke(new Exception(UWR.error));
			UnityEngine.Object.Destroy(base.gameObject);
			throw new Exception(UWR.error);
		}
		HandleResponse(UWR);
	}
}
