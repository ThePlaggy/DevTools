using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

public class DocHook : MonoBehaviour
{
	private const string BASE_DOC_URL = "localGame://";

	[SerializeField]
	public DocDefinition documentData;

	[SerializeField]
	private Browser docBrowser;

	private string docURL;

	private void Awake()
	{
		switch (documentData.Title)
		{
		case "memD3FR4G3R":
			documentData.Title = "Hacks";
			documentData.DocFolder = "DocHackers";
			break;
		case "stackPUSHER":
			documentData.Title = "Viruses";
			documentData.DocFolder = "DocVirusTypes";
			break;
		case "ZoneWall":
			documentData.Title = "Tanner";
			documentData.DocFolder = "DocTanner";
			break;
		case "N0D3H3X3R":
			documentData.Title = "Kidnapper";
			documentData.DocFolder = "DocKidnapper";
			break;
		}
		Text[] array = Object.FindObjectsOfType<Text>();
		foreach (Text text in array)
		{
			switch (text.text)
			{
			case "memD3FR4G3R":
				text.text = " Hacks";
				break;
			case "stackPUSHER":
				text.text = " Viruses";
				break;
			case "ZONEWALL":
				text.text = " Tanner";
				break;
			case "N0D3H3X3R":
				text.text = " Kidnapper";
				break;
			}
		}
		docURL = "localGame://" + documentData.DocFolder + "/index.html";
		docBrowser.onLoad += pageLoaded;
		SteamSlinger.Ins.AddTutDoc(documentData.GetHashCode());
	}

	public void OpenDoc()
	{
		docBrowser.LoadURL(docURL, force: false);
		SteamSlinger.Ins.ReadTutDoc(documentData.GetHashCode());
	}

	private void pageLoaded(JSONNode obj)
	{
		docBrowser.RegisterFunction("LinkHover", delegate
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: true);
		});
		docBrowser.RegisterFunction("LinkOut", delegate
		{
			GameManager.ManagerSlinger.CursorManager.PointerCursorState(active: false);
		});
	}
}
