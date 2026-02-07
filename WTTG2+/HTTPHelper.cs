using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPHelper<T> : IDisposable
{
	public CustomEvent<T> OnResponse = new CustomEvent<T>();

	public CustomEvent<Exception> OnError = new CustomEvent<Exception>();

	public const string API = "https://wttg2plus.ampersoft.cz/api";

	private static GameObject hookObject;

	private HTTPHook myHook;

	public bool DontDispose { get; set; }

	public HTTP_METHOD Method { get; set; }

	public string URL { get; private set; }

	public WWWForm Content { get; private set; } = new WWWForm();

	public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

	public static GameObject HookObject
	{
		get
		{
			if (hookObject == null)
			{
				hookObject = new GameObject("HTTPHook");
			}
			return hookObject;
		}
	}

	public HTTPHelper(string URL = null, HTTP_METHOD Method = HTTP_METHOD.GET)
	{
		if (URL != null)
		{
			SetURL(URL);
		}
		SetMethod(Method);
	}

	public void SetMethod(HTTP_METHOD Method)
	{
		this.Method = Method;
	}

	public void SetURL(string URL, bool isApi = true)
	{
		if (isApi)
		{
			URL = "https://wttg2plus.ampersoft.cz/api" + URL;
		}
		this.URL = URL;
	}

	public void AddPostField(string key, string value)
	{
		Content.AddField(key, JsonUtility.ToJson(value));
	}

	public void ClearPostFields()
	{
		Content = new WWWForm();
	}

	public void AddHeader(string key, string value)
	{
		Headers.Add(key, value);
	}

	public void Send(Action<T> Response = null, Action<Exception> Error = null)
	{
		if (Response == null)
		{
			Response = delegate(T T)
			{
				OnResponse.Execute(T);
			};
		}
		if (Error == null)
		{
			Error = delegate(Exception Exception)
			{
				OnError.Execute(Exception);
			};
		}
		SendRequest(Response, Error);
	}

	public void Dispose()
	{
		if (DontDispose)
		{
			UnityEngine.Object.Destroy(myHook);
		}
	}

	private void SendRequest(Action<T> Response, Action<Exception> Error)
	{
		if (myHook == null)
		{
			myHook = HookObject.AddComponent<HTTPHook>();
		}
		UnityWebRequest UWR = new UnityWebRequest
		{
			certificateHandler = new CertificateManager.IgnoreCertificateHandler(),
			method = Method.ToString(),
			uri = new Uri(URL)
		};
		if (Method == HTTP_METHOD.POST)
		{
			byte[] array = null;
			if (Content != null)
			{
				array = Content.data;
				if (array.Length == 0)
				{
					array = null;
				}
			}
			UWR.uploadHandler = new UploadHandlerRaw(array);
			UWR.downloadHandler = new DownloadHandlerBuffer();
			if (array != null)
			{
				UWR.SetRequestHeader("Content-Type", "application/json");
				Dictionary<string, string> headers = Content.headers;
				foreach (KeyValuePair<string, string> item in headers)
				{
					UWR.SetRequestHeader(item.Key, item.Value);
				}
			}
		}
		foreach (KeyValuePair<string, string> header in Headers)
		{
			UWR.SetRequestHeader(header.Key, header.Value);
		}
		myHook.CreateRequest(UWR, Error, delegate(UnityWebRequest UnityWebRequest)
		{
			if ((int)UnityWebRequest.responseCode > 300)
			{
				Debug.Log(URL + ", " + typeof(T).Name);
				throw new InvalidOperationException(UnityWebRequest.error + "\r\n" + UnityWebRequest.downloadHandler.text);
			}
			try
			{
				T val = JsonUtility.FromJson<T>(UnityWebRequest.downloadHandler?.text);
				if (val == null)
				{
					throw new Exception("Invalid " + typeof(T).Name + " Deserialization :((");
				}
				ResponseModel obj = val as ResponseModel;
				if (obj != null && !obj.Passed)
				{
					Debug.Log("Request for " + typeof(T).Name + " didn't passed ?");
				}
				try
				{
					Response?.Invoke(val);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				finally
				{
					Dispose();
					UWR.Dispose();
				}
			}
			catch (Exception message2)
			{
				Debug.Log(message2);
				throw new Exception("Invalid " + typeof(T).Name + " Deserialization :((");
			}
		});
	}
}
