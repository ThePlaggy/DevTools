using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMCustomNodeHexer : MonoBehaviour
{
	public Slider matrixSizeSlider;

	public Slider tagPiecesSlider;

	public Slider timeBoostSlider;

	public TMP_Text matrixSizeTXT;

	public TMP_Text tagPiecesTXT;

	public TMP_Text timeBoostTXT;

	public int matrixSize;

	public int tagPieces;

	public float timeBoost;

	public float startTime;

	public Slider startTimeSlider;

	public TMP_Text startTimeTXT;

	private void OnEnable()
	{
		HMCustomHack.AdjustSlider(matrixSizeSlider, 3f, 15f, 5f);
		HMCustomHack.AdjustSlider(tagPiecesSlider, 1f, 50f, 3f);
		HMCustomHack.AdjustSlider(timeBoostSlider, 1f, 5f, 1.25f);
		HMCustomHack.AdjustSlider(startTimeSlider, 5f, 15f, 7.5f);
		MatrixSizeChanged();
		TagPiecesChanged();
		TimeBoostChanged();
		StartTimeChanged();
	}

	public void MatrixSizeChanged()
	{
		matrixSize = (int)matrixSizeSlider.value;
		matrixSizeTXT.text = matrixSize.ToString();
	}

	public void TagPiecesChanged()
	{
		tagPieces = (int)tagPiecesSlider.value;
		tagPiecesTXT.text = tagPieces.ToString();
	}

	public void TimeBoostChanged()
	{
		timeBoost = timeBoostSlider.value;
		timeBoost *= 100f;
		timeBoost = (int)timeBoost;
		timeBoost /= 100f;
		timeBoostTXT.text = timeBoost.ToString();
	}

	public void StartTimeChanged()
	{
		startTime = startTimeSlider.value;
		startTime *= 100f;
		startTime = (int)startTime;
		startTime /= 100f;
		startTimeTXT.text = startTime.ToString();
	}
}
