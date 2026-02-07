using UnityEngine;
using UnityEngine.UI;

public class SourceViewerBehaviour : WindowBehaviour
{
	private const float START_SET_Y = -5f;

	private const float NOTE_OBJ_SET_X = 5f;

	private const float NOTE_OBJ_BOT_SPACING = 5f;

	private const float NOTES_CONTENT_BOT_SPACE = 10f;

	private string currentHTML;

	private GameObject notesObject;

	private Vector2 notesStartPOS = new Vector2(5f, -5f);

	private Vector2 sourceObjectHolderSize = Vector2.zero;

	protected new void Awake()
	{
		base.Awake();
		notesObject = LookUp.DesktopUI.SOURCE_NOTES_OBJECT;
		currentHTML = string.Empty;
		GameManager.BehaviourManager.SourceViewerBehaviour = this;
		if (Themes.selected <= THEME.TWR)
		{
			Scrollbar component = LookUp.DesktopUI.SOURCE_WINDOW_CONTENT.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>();
			ColorBlock colors = component.colors;
			colors.normalColor = new Color(0.326f, 0.706f, 1f);
			colors.highlightedColor = new Color(0.306f, 0.581f, 0.9f);
			colors.pressedColor = new Color(0.126f, 0.411f, 0.7f);
			colors.disabledColor = new Color(0f, 0.3f, 0.5f);
			component.colors = colors;
		}
	}

	public void ViewSourceCode(string SetHTML, bool DoSetHTML = true)
	{
		if (DoSetHTML)
		{
			currentHTML = SetHTML;
		}
		if (!Window.activeSelf)
		{
			Launch();
			return;
		}
		Window.GetComponent<BringWindowToFrontBehaviour>().forceTap();
		buildHTML();
	}

	protected override void OnLaunch()
	{
		if (!Window.activeSelf)
		{
			buildHTML();
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
		buildHTML();
	}

	protected override void OnUnMax()
	{
		buildHTML();
	}

	protected override void OnResized()
	{
		buildHTML();
	}

	private void buildHTML()
	{
		notesObject.GetComponent<NoteObject>().BuildMe(currentHTML, SOFTWARE_PRODUCTS.SOURCE_VIEWER);
		notesObject.GetComponent<RectTransform>().anchoredPosition = notesStartPOS;
		resizeContentHolder();
	}

	private void resizeContentHolder()
	{
		sourceObjectHolderSize.x = LookUp.DesktopUI.SOURCE_WINDOW_OBJECT_HOLDER.sizeDelta.x;
		sourceObjectHolderSize.y = -5f + notesObject.GetComponent<RectTransform>().sizeDelta.y;
		LookUp.DesktopUI.SOURCE_WINDOW_OBJECT_HOLDER.sizeDelta = sourceObjectHolderSize;
	}
}
