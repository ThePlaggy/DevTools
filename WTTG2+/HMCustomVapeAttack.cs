using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMCustomVapeAttack : MonoBehaviour
{
	public Slider matrixSizeSlider;

	public Slider timePerBlockSlider;

	public Slider freeCountPerSlider;

	public Slider groupSizeSlider;

	public Slider deadNoteSizeSlider;

	public TMP_Text matrixSizeTXT;

	public TMP_Text timePerBlockTXT;

	public TMP_Text freeCountPerTXT;

	public TMP_Text groupSizeTXT;

	public TMP_Text deadNoteSizeTXT;

	public int matrixSize;

	public float timePerBlock;

	public float freeCountPer;

	public int groupSize;

	public int deadNoteSize;

	private void OnEnable()
	{
		HMCustomHack.AdjustSlider(timePerBlockSlider, 0.5f, 1f, 1f);
		HMCustomHack.AdjustSlider(matrixSizeSlider, 3f, 12f, 4f);
		HMCustomHack.AdjustSlider(freeCountPerSlider, 0.5f, 1f, 0.65f);
		HMCustomHack.AdjustSlider(groupSizeSlider, 1f, 5f, 2f);
		HMCustomHack.AdjustSlider(deadNoteSizeSlider, 0f, 25f, 2f);
		matrixSizeChanged();
		timePerBlockChanged();
		freeCountPerChanged();
		groupSizeChanged();
		deadNoteChanged();
	}

	public void matrixSizeChanged()
	{
		matrixSize = (int)matrixSizeSlider.value;
		matrixSizeTXT.text = matrixSize.ToString();
	}

	public void timePerBlockChanged()
	{
		timePerBlock = timePerBlockSlider.value;
		timePerBlock *= 100f;
		timePerBlock = (int)timePerBlock;
		timePerBlock /= 100f;
		timePerBlockTXT.text = timePerBlock.ToString();
	}

	public void freeCountPerChanged()
	{
		freeCountPer = freeCountPerSlider.value;
		freeCountPer *= 100f;
		freeCountPer = (int)freeCountPer;
		freeCountPer /= 100f;
		freeCountPerTXT.text = freeCountPer.ToString();
	}

	public void groupSizeChanged()
	{
		groupSize = (int)groupSizeSlider.value;
		groupSizeTXT.text = groupSize.ToString();
	}

	public void deadNoteChanged()
	{
		deadNoteSize = (int)deadNoteSizeSlider.value;
		deadNoteSizeTXT.text = deadNoteSize.ToString();
	}
}
