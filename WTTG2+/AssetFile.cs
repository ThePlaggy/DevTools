using System.IO;
using UnityEngine;

public class AssetFile
{
	public string fileLocation;

	public string fileName;

	public string filePath;

	public long size;

	public AssetFile(long _size, string _fileName)
	{
		size = _size;
		fileName = _fileName;
		fileLocation = "file:///" + Application.dataPath + "/Resources/" + fileName;
		filePath = GetFullLongPath(Application.dataPath + "/Resources/" + fileName);
	}

	public bool ValidateSize()
	{
		return ValidateExists() && new FileInfo(filePath).Length == size;
	}

	public bool ValidateExists()
	{
		return File.Exists(filePath);
	}

	private string GetFullLongPath(string path)
	{
		if (!path.StartsWith("\\\\?\\"))
		{
			return "\\\\?\\" + Path.GetFullPath(path);
		}
		return path;
	}
}
