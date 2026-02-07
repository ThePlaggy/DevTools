using System;

[Serializable]
public class WebPageDefinition : Definition
{
	public string PageName;

	public string FileName;

	public string PageHTML;

	public bool HasMusic;

	public AudioFileDefinition AudioFile;

	public KEY_DISCOVERY_MODES KeyDiscoverMode;

	public bool IsTapped;

	public int HashIndex;

	public string HashValue;

	public GameEvent PageEvent;

	public bool isWTTG1Website;

	public void InvokePageEvent()
	{
		if (PageEvent != null)
		{
			PageEvent.Raise();
		}
	}
}
