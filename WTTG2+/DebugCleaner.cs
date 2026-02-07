using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DebugCleaner
{
	public static readonly string logPath = Application.persistentDataPath + "/output.log";

	public static void CleanUnloadingAssetsWarning()
	{
		List<string> list = File.ReadAllLines(logPath).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].StartsWith("Unloading") || list[i].StartsWith("UnloadTime") || list[i].StartsWith("Total:"))
			{
				list.RemoveAt(i);
			}
		}
		File.WriteAllLines(logPath, list.ToArray());
	}

	public static void CleanCorruptedFilesWarning()
	{
		List<string> list = File.ReadAllLines(logPath).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].StartsWith("The file ") || list[i].StartsWith("[Position out of bounds!]"))
			{
				list.RemoveAt(i);
			}
		}
		File.WriteAllLines(logPath, list.ToArray());
	}

	public static void CleanScriptSerializationWarning()
	{
		List<string> list = File.ReadAllLines(logPath).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].StartsWith("A script behaviour") || list[i].StartsWith("Did you"))
			{
				list.RemoveAt(i);
			}
		}
		File.WriteAllLines(logPath, list.ToArray());
	}
}
