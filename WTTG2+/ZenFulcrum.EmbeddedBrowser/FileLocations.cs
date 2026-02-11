using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser
{

	public static class FileLocations
	{
		public class CEFDirs
		{
			public string binariesPath;

			public string localesPath;

			public string logFile;

			public string resourcesPath;

			public string subprocessFile;
		}

		public const string SlaveExecutable = "ZFGameBrowser";

		private static CEFDirs _dirs;

		public static CEFDirs Dirs
		{
			get
			{
				CEFDirs result;
				if ((result = _dirs) == null)
				{
					result = (_dirs = GetCEFDirs());
				}
				return result;
			}
		}

		private static CEFDirs GetCEFDirs()
		{
			string text = Application.dataPath + "/Plugins";
			return new CEFDirs
			{
				resourcesPath = text,
				binariesPath = Application.dataPath + "/../",
				localesPath = text + "/locales",
				subprocessFile = text + "/ZFGameBrowser.exe",
				logFile = Application.dataPath + "/output_log.txt"
			};
		}
	}
}