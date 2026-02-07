using System;

[Serializable]
public class TextDocIconData
{
	public int ID;

	public string TextDocName;

	public string TextDocText;

	public Vect2 TextDocPOS;

	public TextDocIconData(int SetID, string SetName, string SetText, Vect2 SetPOS)
	{
		ID = SetID;
		TextDocName = SetName;
		TextDocText = SetText;
		TextDocPOS = SetPOS;
	}
}
