using System;

[Serializable]
public struct BookmarkData
{
	public string MyTitle;

	public string MyURL;

	public BookmarkData(string SetTitle, string SetURL)
	{
		MyTitle = SetTitle;
		MyURL = SetURL;
	}
}
