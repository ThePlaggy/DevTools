using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMCustomStackPusher : MonoBehaviour
{
	public Slider matrixSizeSlider;

	public Slider stackPiecesSlider;

	public Slider deadPiecesSlider;

	public Slider timePerPieceSlider;

	public TMP_Text matrixSizeTXT;

	public TMP_Text stackPiecesTXT;

	public TMP_Text deadPiecesTXT;

	public TMP_Text timePerPieceTXT;

	public int matrixSize;

	public int stackPieces;

	public int deadPieces;

	public float timePerPiece;

	public bool randomExit;

	public Toggle randomExitToggle;

	private void OnEnable()
	{
		randomExitToggle.isOn = false;
		HMCustomHack.AdjustSlider(matrixSizeSlider, 3f, 21f, 7f);
		HMCustomHack.AdjustSlider(stackPiecesSlider, 1f, 150f, 5f);
		HMCustomHack.AdjustSlider(deadPiecesSlider, 0f, 75f, 3f);
		HMCustomHack.AdjustSlider(timePerPieceSlider, 0.25f, 2.5f, 0.75f);
		randomExitChanged(active: false);
		matrixSizeChanged();
		stackPiecesChanged();
		deadPiecesChanged();
		timePerPieceChanged();
	}

	public void matrixSizeChanged()
	{
		matrixSize = (int)matrixSizeSlider.value;
		matrixSizeTXT.text = matrixSize.ToString();
	}

	public void stackPiecesChanged()
	{
		stackPieces = (int)stackPiecesSlider.value;
		if (stackPiecesSlider.value >= 100f)
		{
			stackPiecesTXT.enableAutoSizing = true;
		}
		else
		{
			stackPiecesTXT.enableAutoSizing = false;
			stackPiecesTXT.fontSize = 36f;
		}
		stackPiecesTXT.text = stackPieces.ToString();
	}

	public void deadPiecesChanged()
	{
		deadPieces = (int)deadPiecesSlider.value;
		deadPiecesTXT.text = deadPieces.ToString();
	}

	public void timePerPieceChanged()
	{
		timePerPiece = timePerPieceSlider.value;
		timePerPiece *= 100f;
		timePerPiece = (int)timePerPiece;
		timePerPiece /= 100f;
		timePerPieceTXT.text = timePerPiece.ToString();
	}

	public void randomExitChanged(bool active)
	{
		randomExit = active;
	}
}
