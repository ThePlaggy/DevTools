using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZenFulcrum.EmbeddedBrowser;

public class StandaloneWebResources : WebResources
{
	public struct IndexEntry
	{
		public string name;

		public long offset;

		public int length;
	}

	private const string FileHeader = "zfbRes_v1";

	public const string DefaultPath = "Resources/browser_assets";

	protected string dataFile;

	protected Dictionary<string, IndexEntry> toc = new Dictionary<string, IndexEntry>();

	public StandaloneWebResources(string dataFile)
	{
		this.dataFile = dataFile;
	}

	public void LoadIndex()
	{
		using BinaryReader binaryReader = new BinaryReader(File.OpenRead(dataFile));
		string text = binaryReader.ReadString();
		if (text != "zfbRes_v1")
		{
			throw new Exception("Invalid web resource file");
		}
		int num = binaryReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			IndexEntry value = new IndexEntry
			{
				name = binaryReader.ReadString(),
				offset = binaryReader.ReadInt64(),
				length = binaryReader.ReadInt32()
			};
			toc[value.name] = value;
		}
	}

	public override byte[] GetData(string path)
	{
		if (!toc.TryGetValue(path, out var value))
		{
			return null;
		}
		byte[] result;
		using (FileStream fileStream = File.OpenRead(dataFile))
		{
			fileStream.Seek(value.offset, SeekOrigin.Begin);
			byte[] array = new byte[value.length];
			int num = fileStream.Read(array, 0, value.length);
			if (num != array.Length)
			{
				throw new Exception("Insufficient data for file");
			}
			result = array;
		}
		return result;
	}

	public void WriteData(Dictionary<string, byte[]> files)
	{
		Dictionary<string, IndexEntry> dictionary = new Dictionary<string, IndexEntry>();
		using FileStream fileStream = File.OpenWrite(dataFile);
		BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8);
		binaryWriter.Write("zfbRes_v1");
		binaryWriter.Write(files.Count);
		long position = fileStream.Position;
		foreach (KeyValuePair<string, byte[]> file in files)
		{
			binaryWriter.Write(file.Key);
			binaryWriter.Write(0L);
			binaryWriter.Write(0);
		}
		foreach (KeyValuePair<string, byte[]> file2 in files)
		{
			byte[] value = file2.Value;
			IndexEntry value2 = new IndexEntry
			{
				name = file2.Key,
				length = file2.Value.Length,
				offset = fileStream.Position
			};
			binaryWriter.Write(value);
			dictionary[file2.Key] = value2;
		}
		binaryWriter.Seek((int)position, SeekOrigin.Begin);
		foreach (KeyValuePair<string, byte[]> file3 in files)
		{
			IndexEntry indexEntry = dictionary[file3.Key];
			binaryWriter.Write(file3.Key);
			binaryWriter.Write(indexEntry.offset);
			binaryWriter.Write(indexEntry.length);
		}
	}
}
