using Colorful;
using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class ComputerCameraManager : MonoBehaviour
{
	public static ComputerCameraManager Ins;

	private Sequence beginHackAniSeq;

	private Sequence hackedBlockSeq;

	private Sequence hackedSeq;

	private Camera mainCamera;

	private Camera myCamera;

	private Bloom postBloom;

	private BrightnessContrastGamma postBright;

	private AnalogTV postFXAnalogTV;

	private Glitch postFXGlitch;

	private Led postFXLED;

	private Sequence showLocationAniSeq;

	private Sequence triggerHackingTerminalDumpGlitchSeq;

	public RenderTexture FinalRenderTexture { get; private set; }

	private void Awake()
	{
		Ins = this;
		myCamera = GetComponent<Camera>();
		FinalRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
		FinalRenderTexture.wrapMode = TextureWrapMode.Clamp;
		FinalRenderTexture.filterMode = FilterMode.Trilinear;
		myCamera.orthographicSize = (float)Screen.height / 2f;
		base.transform.localPosition = new Vector3((float)Screen.width / 2f, 0f - (float)(Screen.height / 2), base.transform.localPosition.z);
		myCamera.targetTexture = FinalRenderTexture;
		postFXAnalogTV = GetComponent<AnalogTV>();
		postFXGlitch = GetComponent<Glitch>();
		postFXLED = GetComponent<Led>();
		postBright = GetComponent<BrightnessContrastGamma>();
		postBloom = GetComponent<Bloom>();
		beginHackAniSeq = DOTween.Sequence();
		beginHackAniSeq.Insert(0f, DOTween.To(() => postFXAnalogTV.Distortion, delegate(float x)
		{
			postFXAnalogTV.Distortion = x;
		}, -0.09f, 0.1f).SetEase(Ease.InCirc));
		beginHackAniSeq.Insert(0f, DOTween.To(() => postBright.Contrast, delegate(float x)
		{
			postBright.Contrast = x;
		}, -100f, 0.2f).SetEase(Ease.Linear));
		beginHackAniSeq.Insert(0f, DOTween.To(() => postBright.Gamma, delegate(float x)
		{
			postBright.Contrast = x;
		}, 9.9f, 0.2f).SetEase(Ease.Linear));
		beginHackAniSeq.Insert(0.25f, DOTween.To(() => postFXAnalogTV.Distortion, delegate(float x)
		{
			postFXAnalogTV.Distortion = x;
		}, 0f, 0.2f).SetEase(Ease.OutCirc));
		beginHackAniSeq.Insert(0.25f, DOTween.To(() => postBright.Contrast, delegate(float x)
		{
			postBright.Contrast = x;
		}, 0f, 0.15f).SetEase(Ease.Linear));
		beginHackAniSeq.Insert(0.25f, DOTween.To(() => postBright.Gamma, delegate(float x)
		{
			postBright.Contrast = x;
		}, 1f, 0.15f).SetEase(Ease.Linear));
		beginHackAniSeq.Insert(0.45f, DOTween.To(() => postFXGlitch.SettingsTearing.Speed, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Speed = x;
		}, 4f, 0.7f).SetEase(Ease.Linear));
		beginHackAniSeq.Insert(0.45f, DOTween.To(() => postFXGlitch.SettingsTearing.Intensity, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Intensity = x;
		}, 0.64f, 0.5f).SetEase(Ease.Linear));
		beginHackAniSeq.Pause();
		beginHackAniSeq.SetAutoKill(autoKillOnCompletion: false);
		showLocationAniSeq = DOTween.Sequence();
		showLocationAniSeq.Insert(0f, DOTween.To(() => postFXAnalogTV.Distortion, delegate(float x)
		{
			postFXAnalogTV.Distortion = x;
		}, -0.09f, 0.1f).SetEase(Ease.InCirc));
		showLocationAniSeq.Insert(0f, DOTween.To(() => postFXAnalogTV.Distortion, delegate(float x)
		{
			postFXAnalogTV.Distortion = x;
		}, 0f, 0.2f).SetEase(Ease.OutCirc));
		showLocationAniSeq.Insert(0.25f, DOTween.To(() => postFXGlitch.SettingsTearing.Speed, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Speed = x;
		}, 4f, 0.7f).SetEase(Ease.Linear));
		showLocationAniSeq.Insert(0.25f, DOTween.To(() => postFXGlitch.SettingsTearing.Intensity, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Intensity = x;
		}, 0.64f, 0.5f).SetEase(Ease.Linear));
		showLocationAniSeq.Pause();
		showLocationAniSeq.SetAutoKill(autoKillOnCompletion: false);
		triggerHackingTerminalDumpGlitchSeq = DOTween.Sequence();
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsInterferences.Speed, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.Speed = x;
		}, 10f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsInterferences.Density, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.Density = x;
		}, 3.1f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsInterferences.MaxDisplacement, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.MaxDisplacement = x;
		}, 1.32f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsTearing.Speed, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Speed = x;
		}, 30f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsTearing.Intensity, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Intensity = x;
		}, 0.145f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Insert(0f, DOTween.To(() => postFXGlitch.SettingsTearing.MaxDisplacement, delegate(float x)
		{
			postFXGlitch.SettingsTearing.MaxDisplacement = x;
		}, 0.028f, 0.25f).SetEase(Ease.Linear));
		triggerHackingTerminalDumpGlitchSeq.Pause();
		triggerHackingTerminalDumpGlitchSeq.SetAutoKill(autoKillOnCompletion: false);
		hackedBlockSeq = DOTween.Sequence();
		hackedBlockSeq.Insert(0f, DOTween.To(() => postFXAnalogTV.Scale, delegate(float x)
		{
			postFXAnalogTV.Scale = x;
		}, 1f, 0.35f).SetEase(Ease.InExpo));
		hackedBlockSeq.Insert(1.85f, DOTween.To(() => postBright.Brightness, delegate(float x)
		{
			postBright.Brightness = x;
		}, 100f, 0.4f).SetEase(Ease.Linear));
		hackedBlockSeq.Insert(1.85f, DOTween.To(() => postBright.Contrast, delegate(float x)
		{
			postBright.Contrast = x;
		}, -100f, 0.4f).SetEase(Ease.Linear));
		hackedBlockSeq.Insert(1.85f, DOTween.To(() => postBright.Gamma, delegate(float x)
		{
			postBright.Gamma = x;
		}, 9.9f, 0.4f).SetEase(Ease.Linear));
		hackedBlockSeq.Pause();
		hackedBlockSeq.SetAutoKill(autoKillOnCompletion: false);
		hackedSeq = DOTween.Sequence();
		hackedSeq.Insert(0.55f, DOTween.To(() => postBloom.bloomIntensity, delegate(float x)
		{
			postBloom.bloomIntensity = x;
		}, 4.4f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postBloom.bloomThreshold, delegate(float x)
		{
			postBloom.bloomThreshold = x;
		}, 0.5f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postBloom.bloomBlurIterations, delegate(int x)
		{
			postBloom.bloomBlurIterations = x;
		}, 2, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsInterferences.Speed, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.Speed = x;
		}, 4.5f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsInterferences.Density, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.Density = x;
		}, 4f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsInterferences.MaxDisplacement, delegate(float x)
		{
			postFXGlitch.SettingsInterferences.MaxDisplacement = x;
		}, 8f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsTearing.Speed, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Speed = x;
		}, 0.95f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsTearing.Intensity, delegate(float x)
		{
			postFXGlitch.SettingsTearing.Intensity = x;
		}, 0.02f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(0.55f, DOTween.To(() => postFXGlitch.SettingsTearing.MaxDisplacement, delegate(float x)
		{
			postFXGlitch.SettingsTearing.MaxDisplacement = x;
		}, 0.125f, 0.3f).SetEase(Ease.Linear));
		hackedSeq.Insert(2.5f, DOTween.To(() => postBright.Brightness, delegate(float x)
		{
			postBright.Brightness = x;
		}, 100f, 0.4f).SetEase(Ease.Linear));
		hackedSeq.Insert(2.5f, DOTween.To(() => postBright.Contrast, delegate(float x)
		{
			postBright.Contrast = x;
		}, -100f, 0.4f).SetEase(Ease.Linear));
		hackedSeq.Insert(2.5f, DOTween.To(() => postBright.Gamma, delegate(float x)
		{
			postBright.Gamma = x;
		}, 9.9f, 0.4f).SetEase(Ease.Linear));
		hackedSeq.Pause();
		hackedSeq.SetAutoKill(autoKillOnCompletion: false);
		CameraManager.Get(CAMERA_ID.MAIN, out mainCamera);
	}

	public void BecomeMaster()
	{
		myCamera.targetTexture = null;
		mainCamera.enabled = false;
	}

	public void BecomeSlave()
	{
		myCamera.targetTexture = FinalRenderTexture;
		mainCamera.enabled = true;
	}

	public void BeginHackAni()
	{
		postFXAnalogTV.enabled = true;
		postFXGlitch.enabled = true;
		postBright.enabled = true;
		postFXGlitch.SettingsTearing.MaxDisplacement = 0.215f;
		beginHackAniSeq.Restart();
	}

	public void TriggerShowEndLocation()
	{
		postFXAnalogTV.enabled = true;
		postFXGlitch.enabled = true;
		postBright.enabled = true;
		postFXGlitch.SettingsTearing.MaxDisplacement = 0.215f;
		showLocationAniSeq.Restart();
	}

	public void TriggerHackingTerminalDumpGlitch()
	{
		postFXGlitch.enabled = true;
		postFXGlitch.Mode = Glitch.GlitchingMode.Complete;
		triggerHackingTerminalDumpGlitchSeq.Restart();
	}

	public void TriggerHackingTerminalSkullEFXs()
	{
		postFXLED.enabled = true;
		postFXGlitch.enabled = true;
		postFXGlitch.Mode = Glitch.GlitchingMode.Interferences;
		postFXGlitch.SettingsInterferences.Speed = 10f;
		postFXGlitch.SettingsInterferences.Density = 30f;
		postFXGlitch.SettingsInterferences.MaxDisplacement = 0.215f;
	}

	public void TriggerHackBlockedEFXs()
	{
		postBright.enabled = true;
		postFXAnalogTV.enabled = true;
		postFXGlitch.enabled = true;
		postBloom.enabled = true;
		postFXAnalogTV.NoiseIntensity = 0.03f;
		postFXAnalogTV.ScanlinesIntensity = 0.59f;
		postFXAnalogTV.ScanlinesCount = 772;
		postFXAnalogTV.Distortion = -0.75f;
		postFXAnalogTV.CubicDistortion = -0.75f;
		postFXAnalogTV.Scale = 0.2f;
		postFXGlitch.Mode = Glitch.GlitchingMode.Complete;
		postFXGlitch.SettingsInterferences.Speed = 4.5f;
		postFXGlitch.SettingsInterferences.Density = 4f;
		postFXGlitch.SettingsInterferences.MaxDisplacement = 8f;
		postFXGlitch.SettingsTearing.Speed = 0.95f;
		postFXGlitch.SettingsTearing.Intensity = 0.02f;
		postFXGlitch.SettingsTearing.MaxDisplacement = 0.125f;
		postBloom.bloomIntensity = 4.4f;
		postBloom.bloomThreshold = 0.5f;
		postBloom.bloomBlurIterations = 2;
		hackedBlockSeq.Restart();
	}

	public void TriggerHackedEFXs()
	{
		postBright.enabled = true;
		postFXLED.enabled = true;
		postFXGlitch.enabled = true;
		postBloom.enabled = true;
		postFXGlitch.Mode = Glitch.GlitchingMode.Interferences;
		postFXGlitch.SettingsInterferences.Speed = 10f;
		postFXGlitch.SettingsInterferences.Density = 30f;
		postFXGlitch.SettingsInterferences.MaxDisplacement = 0.215f;
		hackedSeq.Restart();
		GameManager.TimeSlinger.FireTimer(0.55f, delegate
		{
			postFXLED.enabled = false;
			postFXGlitch.Mode = Glitch.GlitchingMode.Complete;
		});
	}

	public void ClearPostFXs()
	{
		postFXAnalogTV.enabled = false;
		postFXGlitch.enabled = false;
		postFXLED.enabled = false;
		postBright.enabled = false;
		postBloom.enabled = false;
		postFXGlitch.Mode = Glitch.GlitchingMode.Tearing;
		postFXGlitch.SettingsInterferences.Speed = 0f;
		postFXGlitch.SettingsInterferences.Density = 0f;
		postFXGlitch.SettingsInterferences.MaxDisplacement = 0f;
		postFXGlitch.SettingsTearing.Speed = 0f;
		postFXGlitch.SettingsTearing.Intensity = 0f;
		postFXGlitch.SettingsTearing.MaxDisplacement = 0f;
		postFXAnalogTV.NoiseIntensity = 0f;
		postFXAnalogTV.ScanlinesIntensity = 0f;
		postFXAnalogTV.ScanlinesCount = 0;
		postFXAnalogTV.Distortion = 0f;
		postFXAnalogTV.CubicDistortion = 0f;
		postFXAnalogTV.Scale = 1f;
		postBright.Brightness = 0f;
		postBright.Contrast = 0f;
		postBright.Gamma = 1f;
		postBloom.bloomIntensity = 0f;
		postBloom.bloomThreshold = 0f;
		postBloom.bloomBlurIterations = 0;
	}
}
