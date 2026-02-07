using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMMusicSlider : MonoBehaviour
{
	public Slider slider;

	public TMP_Text text;

	public float volume;

	private void Start()
	{
		ValueChanged(slider.value);
	}

	public void ValueChanged(float value)
	{
		volume = value / 100f;
		text.text = value.ToString();
		if (HackerModeManager.Ins != null)
		{
			HackerModeManager.Ins.SetMusicVolume(volume);
		}
	}
}
