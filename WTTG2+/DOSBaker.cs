using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DOSBaker : MonoBehaviour
{
	public Texture2D[] textsToPack;

	public Rect[] theRects;

	public List<GameObject> meshObjects;

	public List<MeshFilter> meshFilters;

	public GameObject fooObject;

	public bool hasPrepErrors;

	public bool meshesArePrepped;

	public string prepErrors = string.Empty;

	public string objectsToCombineNames = string.Empty;

	public void prepTheMeshes()
	{
		hasPrepErrors = false;
		prepErrors = string.Empty;
		objectsToCombineNames = string.Empty;
		if (meshObjects.Count > 0)
		{
			if (meshFilters == null)
			{
				meshFilters = new List<MeshFilter>();
			}
			for (int i = 0; i < meshObjects.Count; i++)
			{
				if (meshObjects[i] != null)
				{
					if (meshObjects[i].GetComponent<MeshFilter>() != null)
					{
						meshFilters.Add(meshObjects[i].GetComponent<MeshFilter>());
						string text = objectsToCombineNames;
						objectsToCombineNames = text + "\nObject #" + i + " - " + meshObjects[i].name;
					}
					else
					{
						hasPrepErrors = true;
						string text2 = prepErrors;
						prepErrors = text2 + "\nObject #" + i + " - " + meshObjects[i].name + ": Has no MeshFilter Component!";
					}
				}
				else
				{
					hasPrepErrors = true;
					prepErrors = prepErrors + "\nObject #" + i + ": Is NULL!";
				}
			}
		}
		else
		{
			prepErrors += "\nThere are no mesh objects";
			hasPrepErrors = true;
		}
		if (!hasPrepErrors)
		{
			meshesArePrepped = true;
		}
	}

	public void resetPrep()
	{
		meshFilters = new List<MeshFilter>();
		hasPrepErrors = false;
		meshesArePrepped = false;
		prepErrors = string.Empty;
		objectsToCombineNames = string.Empty;
	}

	public void bakeTheMeshes()
	{
		if (meshesArePrepped && meshFilters.Count > 0)
		{
			GameObject gameObject = new GameObject("MeshBaked");
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			CombineInstance[] array = new CombineInstance[meshFilters.Count];
			for (int i = 0; i < meshFilters.Count; i++)
			{
				array[i].mesh = meshFilters[i].sharedMesh;
				array[i].transform = meshFilters[i].transform.localToWorldMatrix;
				meshFilters[i].gameObject.SetActive(value: false);
			}
			gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
			gameObject.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(array, mergeSubMeshes: true);
		}
	}

	public void fooBar()
	{
		Debug.Log("FOOBAR");
		Texture2D texture2D = new Texture2D(2048, 2048);
		theRects = texture2D.PackTextures(textsToPack, 2, 2048);
		Debug.Log(theRects);
		Debug.Log(texture2D);
		byte[] bytes = texture2D.EncodeToJPG();
		File.WriteAllBytes(Application.dataPath + "/foobar.jpg", bytes);
		Debug.Log(Application.dataPath);
	}
}
