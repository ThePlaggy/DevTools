using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMCustomDOSBlocker : MonoBehaviour
{
	public Slider matrixSizeSlider;

	public Slider actionBlockSizeSlider;

	public Slider gameTimeModifierSlider;

	public Slider hotTimeSlider;

	public Toggle trollNodesActiveToggle;

	public TMP_Text matrixSizeTXT;

	public TMP_Text actionBlockSizeTXT;

	public TMP_Text gameTimeModifierTXT;

	public TMP_Text hotTimeTXT;

	public int matrixSize;

	public int actionBlockSize;

	public float gameTimeModifier;

	public float hotTime;

	public bool trollNodesActive;

	private void OnEnable()
	{
		trollNodesActiveToggle.isOn = false;
		HMCustomHack.AdjustSlider(matrixSizeSlider, 3f, 12f, 6f);
		HMCustomHack.AdjustSlider(actionBlockSizeSlider, 1f, 10f, 3f);
		HMCustomHack.AdjustSlider(gameTimeModifierSlider, 0.5f, 5f, 3f);
		HMCustomHack.AdjustSlider(hotTimeSlider, 0.15f, 1f, 0.75f);
		trollNodesActiveChanged(active: false);
		matrixSizeChanged();
		gameTimeModifierChanged();
		actionBlockSizeChanged();
		hotTimeChanged();
	}

	public void matrixSizeChanged()
	{
		matrixSize = (int)matrixSizeSlider.value;
		matrixSizeTXT.text = matrixSize.ToString();
	}

	public void actionBlockSizeChanged()
	{
		actionBlockSize = (int)actionBlockSizeSlider.value;
		actionBlockSizeTXT.text = actionBlockSize.ToString();
	}

	public void gameTimeModifierChanged()
	{
		gameTimeModifier = gameTimeModifierSlider.value;
		gameTimeModifier *= 100f;
		gameTimeModifier = (int)gameTimeModifier;
		gameTimeModifier /= 100f;
		gameTimeModifierTXT.text = gameTimeModifier.ToString();
	}

	public void hotTimeChanged()
	{
		hotTime = hotTimeSlider.value;
		hotTime *= 100f;
		hotTime = (int)hotTime;
		hotTime /= 100f;
		hotTimeTXT.text = hotTime.ToString();
	}

	public void trollNodesActiveChanged(bool active)
	{
		trollNodesActive = active;
	}
}
