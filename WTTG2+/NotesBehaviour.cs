using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotesBehaviour : WindowBehaviour
{
	private const float START_SET_Y = -5f;

	private const float NOTE_OBJ_SET_X = 5f;

	private const float NOTE_OBJ_BOT_SPACING = 5f;

	private const float NOTES_CONTENT_BOT_SPACE = 10f;

	private List<string> currentNotes = new List<string>();

	private NotesData noteData;

	private int noteID;

	private GameObject notesObject;

	private Vector2 notesObjectHolderSize = Vector2.zero;

	private Vector2 notesStartPOS = new Vector2(5f, -5f);

	protected new void Awake()
	{
		noteID = 123;
		base.Awake();
		GameManager.BehaviourManager.NotesBehaviour = this;
		GameManager.StageManager.Stage += stageMe;
		notesObject = LookUp.DesktopUI.NOTES_NOTES_OBJECT;
		if (Themes.selected <= THEME.TWR)
		{
			Scrollbar component = LookUp.DesktopUI.NOTES_WINDOW_CONTENT.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
			ColorBlock colors = component.colors;
			colors.normalColor = new Color(0.326f, 0.706f, 1f);
			colors.highlightedColor = new Color(0.306f, 0.581f, 0.9f);
			colors.pressedColor = new Color(0.126f, 0.411f, 0.7f);
			colors.disabledColor = new Color(0f, 0.3f, 0.5f);
			component.colors = colors;
		}
	}

	public void AddNote(string NoteToAdd)
	{
		if (NoteToAdd != string.Empty)
		{
			NoteToAdd = NoteToAdd.Trim();
			if (noteData != null)
			{
				noteData.Notes.Add(NoteToAdd);
				DataManager.Save(noteData);
			}
			currentNotes.Add(NoteToAdd);
			buildNotes();
		}
	}

	public void ClearNotes()
	{
		if (noteData != null)
		{
			noteData.Notes.Clear();
			DataManager.Save(noteData);
		}
		currentNotes.Clear();
		buildNotes();
		GameManager.ManagerSlinger.TextDocManager.DeleteFiles();
	}

	protected override void OnLaunch()
	{
		if (!Window.activeSelf)
		{
			buildNotes();
		}
	}

	protected override void OnClose()
	{
	}

	protected override void OnMin()
	{
	}

	protected override void OnUnMin()
	{
	}

	protected override void OnMax()
	{
		buildNotes();
	}

	protected override void OnUnMax()
	{
		buildNotes();
	}

	protected override void OnResized()
	{
		buildNotes();
	}

	private void buildNotes()
	{
		string text = string.Empty;
		for (int i = 0; i < currentNotes.Count; i++)
		{
			text = text + currentNotes[i] + "\n\r\n\r";
		}
		notesObject.GetComponent<NoteObject>().BuildMe(text, SOFTWARE_PRODUCTS.NOTES);
		notesObject.GetComponent<RectTransform>().anchoredPosition = notesStartPOS;
		resizeContentHolder();
	}

	private void resizeContentHolder()
	{
		notesObjectHolderSize.x = LookUp.DesktopUI.NOTES_WINDOW_OBJECT_HOLDER.sizeDelta.x;
		notesObjectHolderSize.y = -5f + notesObject.GetComponent<RectTransform>().sizeDelta.y + 5f;
		LookUp.DesktopUI.NOTES_WINDOW_OBJECT_HOLDER.sizeDelta = notesObjectHolderSize;
		LookUp.DesktopUI.NOTES_WINDOW_CONTENT.normalizedPosition = Vector2.zero;
	}

	private void stageMe()
	{
		noteData = DataManager.Load<NotesData>(noteID);
		if (noteData == null)
		{
			noteData = new NotesData(noteID);
			noteData.Notes = new List<string>(50);
		}
		currentNotes = new List<string>(noteData.Notes);
		buildNotes();
		GameManager.StageManager.Stage -= stageMe;
	}

	public string GetNotes()
	{
		string text = "Notes: \n";
		for (int i = 0; i < currentNotes.Count; i++)
		{
			text = text + currentNotes[i] + "\n";
		}
		return text;
	}
}
