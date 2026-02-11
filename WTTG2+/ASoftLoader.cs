using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ASoftLoader
{
	public AssetBundle myProps;

	public string bundleName;

	public BUNDLE myBundle;

	public bool iAmReady;

	public AssetFile AF;

	private long startTimestamp;

	public ASoftLoader(string bundle, long fileSize)
	{
		myBundle = (Enum.IsDefined(typeof(BUNDLE), bundle) ? ((BUNDLE)Enum.Parse(typeof(BUNDLE), bundle)) : BUNDLE.OTHER);
		AF = new AssetFile(fileSize, bundle.ToLower());
		bundleName = bundle;
	}

    public IEnumerator LoadLocalAsset()
    {
        startTimestamp = TimeSlinger.CurrentTimestampMs;

        if (AF.ValidateExists() && AF.ValidateSize())
        {
            if (myBundle.ToString().ToUpper().StartsWith("WEBSITES"))
            {
                iAmReady = true;
                yield break;
            }

            if (AssetBundleManager.LoadedBundles.Contains(myBundle))
            {
                iAmReady = true;
                yield break;
            }

            iAmReady = false;
            Debug.Log($"[ASoftLoader] Loading Asset: {bundleName} (BUNDLE.{myBundle})");

            using (UnityWebRequest UWR = UnityWebRequestAssetBundle.GetAssetBundle(AF.fileLocation))
            {
                UWR.certificateHandler = new CertificateManager.IgnoreCertificateHandler();
                yield return UWR.SendWebRequest();

                if (UWR.isNetworkError)
                {
                    Debug.Log($"[ASoftLoader] Failed To Load Asset File {bundleName} (BUNDLE.{myBundle}) :(");
                    AssetsManager.AssetsFailed?.Invoke("Failed To Load Asset File " + bundleName);
                    throw new Exception(UWR.error);
                }

                iAmReady = true;
                myProps = DownloadHandlerAssetBundle.GetContent(UWR);
                AssetBundleManager.LoadedBundles.Add(myBundle);

                Debug.Log($"[ASoftLoader] {bundleName} (BUNDLE.{myBundle}) Has Been Loaded, Took: {TimeSlinger.CurrentTimestampMs - startTimestamp}ms");
            }
        }
        else
        {
            AssetsManager.AssetsFailed?.Invoke("Failed To Load Asset File " + bundleName);
        }
    }
}
