using System.Collections.Generic;
using UnityEngine;

public class TextDocManager : MonoBehaviour
{
	public static bool CustomIcon;

	public static int imageCorresponding;

	[SerializeField]
	private int TEXT_DOC_POOL_COUNT = 10;

	[SerializeField]
	private GameObject TextDocIconObject;

	[SerializeField]
	private GameObject TextDocObject;

	private Dictionary<int, TextDocIconObject> currentTextDocIcons = new Dictionary<int, TextDocIconObject>(10);

	private Dictionary<int, TextDocObject> currentTextDocs = new Dictionary<int, TextDocObject>(10);

	private TextDocManagerData myData;

	private int myID;

	private PooledStack<TextDocIconObject> textDocIconPool;

	private PooledStack<TextDocObject> textDocPool;

	private List<TextDocIconObject> files = new List<TextDocIconObject>();

	private void Awake()
	{
		myID = base.transform.position.GetHashCode();
		GameManager.ManagerSlinger.TextDocManager = this;
		GameManager.StageManager.Stage += stageMe;
		textDocIconPool = new PooledStack<TextDocIconObject>(delegate
		{
			TextDocIconObject component = Object.Instantiate(TextDocIconObject, LookUp.DesktopUI.TEXT_DOC_ICONS_PARENT).GetComponent<TextDocIconObject>();
			component.SoftBuild();
			component.UpdateMyData.Event += updateTextDocIconData;
			component.OpenEvents.Event += OpenTextDoc;
			return component;
		}, TEXT_DOC_POOL_COUNT);
		textDocPool = new PooledStack<TextDocObject>(delegate
		{
			TextDocObject component = Object.Instantiate(TextDocObject, LookUp.DesktopUI.WINDOW_HOLDER).GetComponent<TextDocObject>();
			component.SoftBuild();
			component.gameObject.SetActive(value: false);
			return component;
		}, TEXT_DOC_POOL_COUNT);
	}

	private void OnDestroy()
	{
		clearTextDocs();
	}

	public void DeleteFiles()
	{
		for (int i = 0; i < files.Count; i++)
		{
			files[i].DeleteMe();
		}
		Debug.LogFormat("Deleted {0} files from player's desktop", files.Count);
		files.Clear();
	}

	public void CreateTextDoc(string Name, string Text)
	{
		int hashCode = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode))
		{
			float x = Random.Range(15f, (float)Screen.width * 0.9f);
			float y = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData = new TextDocIconData(hashCode, Name, Text, new Vect2(x, y));
			TextDocIconObject textDocIconObject = textDocIconPool.Pop();
			textDocIconObject.Build(textDocIconData);
			currentTextDocIcons.Add(hashCode, textDocIconObject);
			files.Add(textDocIconObject);
			myData.CurrentDocs.Add(textDocIconData);
			DataManager.Save(myData);
			return;
		}
		string setName = Name;
		Name = Name + "(" + Random.Range(0, 100000) + ")";
		int hashCode2 = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode2))
		{
			float x2 = Random.Range(15f, (float)Screen.width * 0.9f);
			float y2 = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData2 = new TextDocIconData(hashCode2, setName, Text, new Vect2(x2, y2));
			TextDocIconObject textDocIconObject2 = textDocIconPool.Pop();
			textDocIconObject2.Build(textDocIconData2);
			currentTextDocIcons.Add(hashCode2, textDocIconObject2);
			files.Add(textDocIconObject2);
			myData.CurrentDocs.Add(textDocIconData2);
			DataManager.Save(myData);
		}
	}

	public void OpenTextDoc(TextDocIconData DocData)
	{
		if (currentTextDocs.ContainsKey(DocData.ID))
		{
			currentTextDocs[DocData.ID].gameObject.SetActive(value: true);
			currentTextDocs[DocData.ID].BumpMe();
		}
		else
		{
			buildTextDoc(DocData);
		}
	}

	private void updateTextDocIconData()
	{
		DataManager.Save(myData);
	}

	private void buildTextDoc(TextDocIconData DocData)
	{
		TextDocObject textDocObject = textDocPool.Pop();
		textDocObject.gameObject.SetActive(value: true);
		textDocObject.Build(DocData.TextDocName, DocData.TextDocText);
		currentTextDocs.Add(DocData.ID, textDocObject);
	}

	private void clearTextDocs()
	{
		foreach (KeyValuePair<int, TextDocIconObject> currentTextDocIcon in currentTextDocIcons)
		{
			textDocIconPool.Push(currentTextDocIcon.Value);
		}
		foreach (TextDocIconObject item in textDocIconPool)
		{
			item.UpdateMyData.Event -= updateTextDocIconData;
			item.OpenEvents.Event -= OpenTextDoc;
		}
		currentTextDocIcons.Clear();
		textDocIconPool.Clear();
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myData = DataManager.Load<TextDocManagerData>(myID);
		if (myData == null)
		{
			myData = new TextDocManagerData(myID);
			myData.CurrentDocs = new List<TextDocIconData>();
		}
		for (int i = 0; i < myData.CurrentDocs.Count; i++)
		{
			TextDocIconData textDocIconData = myData.CurrentDocs[i];
			TextDocIconObject textDocIconObject = textDocIconPool.Pop();
			int hashCode = textDocIconData.TextDocName.GetHashCode();
			textDocIconObject.Build(textDocIconData);
			currentTextDocIcons.Add(hashCode, textDocIconObject);
			files.Add(textDocIconObject);
		}
		DataManager.Save(myData);
	}

	public void CreateGameExecutable(string Name, string Preset)
	{
		int hashCode = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode))
		{
			float x = Random.Range(15f, (float)Screen.width * 0.9f);
			float y = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData = new TextDocIconData(hashCode, Name, "|[.ados]|" + Preset, new Vect2(x, y));
			TextDocIconObject textDocIconObject = textDocIconPool.Pop();
			textDocIconObject.Build(textDocIconData);
			textDocIconObject.SetDevToolsIcon(CustomSpriteLookUp.adosicon);
			currentTextDocIcons.Add(hashCode, textDocIconObject);
			myData.CurrentDocs.Add(textDocIconData);
			DataManager.Save(myData);
			return;
		}
		string setName = Name;
		Name = Name + "(" + Random.Range(0, 100000) + ")";
		int hashCode2 = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode2))
		{
			float x2 = Random.Range(15f, (float)Screen.width * 0.9f);
			float y2 = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData2 = new TextDocIconData(hashCode2, setName, "|[.ados]|" + Preset, new Vect2(x2, y2));
			TextDocIconObject textDocIconObject2 = textDocIconPool.Pop();
			textDocIconObject2.Build(textDocIconData2);
			textDocIconObject2.SetDevToolsIcon(CustomSpriteLookUp.adosicon);
			currentTextDocIcons.Add(hashCode2, textDocIconObject2);
			myData.CurrentDocs.Add(textDocIconData2);
			DataManager.Save(myData);
		}
	}

	public void CreateGamingVPN()
	{
		string text = "Install.exe";
		int hashCode = text.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode))
		{
			float x = Random.Range(15f, (float)Screen.width * 0.9f);
			float y = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData = new TextDocIconData(hashCode, text, "|[.vpn]|", new Vect2(x, y));
			TextDocIconObject textDocIconObject = textDocIconPool.Pop();
			textDocIconObject.Build(textDocIconData);
			textDocIconObject.SetDevToolsIcon(CustomSpriteLookUp.adosicon);
			currentTextDocIcons.Add(hashCode, textDocIconObject);
			myData.CurrentDocs.Add(textDocIconData);
			DataManager.Save(myData);
			return;
		}
		string setName = text;
		text = text + "(" + Random.Range(0, 100000) + ")";
		int hashCode2 = text.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode2))
		{
			float x2 = Random.Range(15f, (float)Screen.width * 0.9f);
			float y2 = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData2 = new TextDocIconData(hashCode2, setName, "|[.vpn]|", new Vect2(x2, y2));
			TextDocIconObject textDocIconObject2 = textDocIconPool.Pop();
			textDocIconObject2.Build(textDocIconData2);
			textDocIconObject2.SetDevToolsIcon(CustomSpriteLookUp.adosicon);
			currentTextDocIcons.Add(hashCode2, textDocIconObject2);
			myData.CurrentDocs.Add(textDocIconData2);
			DataManager.Save(myData);
		}
	}

	public void CreatePNG(string Name)
	{
		int hashCode = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode))
		{
			float x = Random.Range(15f, (float)Screen.width * 0.9f);
			float y = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData = new TextDocIconData(hashCode, Name, "|[.png]|" + imageCorresponding, new Vect2(x, y));
			imageCorresponding++;
			TextDocIconObject textDocIconObject = textDocIconPool.Pop();
			textDocIconObject.Build(textDocIconData);
			textDocIconObject.SetPNGIcon();
			currentTextDocIcons.Add(hashCode, textDocIconObject);
			files.Add(textDocIconObject);
			myData.CurrentDocs.Add(textDocIconData);
			DataManager.Save(myData);
			return;
		}
		string setName = Name;
		Name = Name + "(" + Random.Range(0, 100000) + ")";
		int hashCode2 = Name.GetHashCode();
		if (!currentTextDocIcons.ContainsKey(hashCode2))
		{
			float x2 = Random.Range(15f, (float)Screen.width * 0.9f);
			float y2 = 0f - Random.Range(56f, (float)Screen.height - 40f - 15f);
			TextDocIconData textDocIconData2 = new TextDocIconData(hashCode2, setName, "|[.png]|" + imageCorresponding, new Vect2(x2, y2));
			imageCorresponding++;
			TextDocIconObject textDocIconObject2 = textDocIconPool.Pop();
			textDocIconObject2.Build(textDocIconData2);
			textDocIconObject2.SetPNGIcon();
			currentTextDocIcons.Add(hashCode2, textDocIconObject2);
			files.Add(textDocIconObject2);
			myData.CurrentDocs.Add(textDocIconData2);
			DataManager.Save(myData);
		}
	}
}
