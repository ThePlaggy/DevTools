using System;
using System.IO;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

internal class EditorWebResources : WebResources
{
	protected string basePath;

	public EditorWebResources()
	{
		basePath = Path.GetDirectoryName(Application.dataPath) + "/BrowserAssets";
	}

	public override byte[] GetData(string path)
	{
		try
		{
			return File.ReadAllBytes(basePath + path);
		}
		catch (Exception ex)
		{
			if (!(ex is FileNotFoundException) && !(ex is DirectoryNotFoundException))
			{
				throw;
			}
			return null;
		}
	}
}
