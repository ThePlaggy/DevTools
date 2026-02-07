using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class SplashManager : MonoBehaviour
{
	private static bool BootstrapperInit;

	[SerializeField]
	private Camera myCamera;

	[SerializeField]
	private RectTransform rsLeftRT;

	[SerializeField]
	private RectTransform rsRightRT;

	[SerializeField]
	private CanvasGroup rsLogoCG;

	[SerializeField]
	private CanvasGroup rsLeftCG;

	[SerializeField]
	private CanvasGroup rsRightCG;

	[SerializeField]
	private CanvasGroup rsTextCG;

	[SerializeField]
	private CanvasGroup rsWebsiteCG;

	private Bloom camBloom;

	private Vector2 defaultLogoLeftPOS;

	private Vector2 defaultLogoRightPOS;

	private AudioSource myAS;

	private bool startActive;

	private float startTimeStamp;

	private void Awake()
	{
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
		myAS = GetComponent<AudioSource>();
		camBloom = myCamera.GetComponent<Bloom>();
		myCamera.orthographicSize = (float)Screen.height / 2f;
		myCamera.transform.position = new Vector3((float)Screen.width / 2f, 0f - (float)(Screen.height / 2), myCamera.transform.position.z);
		defaultLogoLeftPOS = rsLeftRT.anchoredPosition;
		defaultLogoRightPOS = rsRightRT.anchoredPosition;
		rsLeftRT.anchoredPosition = new Vector2(0f - ((float)Screen.width / 2f + rsLeftRT.sizeDelta.x / 2f), rsLeftRT.anchoredPosition.y);
		rsRightRT.anchoredPosition = new Vector2((float)Screen.width / 2f + rsRightRT.sizeDelta.x / 2f, rsRightRT.anchoredPosition.y);
		if (!BootstrapperInit)
		{
			BootstrapperInit = true;
			new GameObject("ASoftAPI").AddComponent<Bootstrapper>();
		}
	}

	private void Start()
	{
		if (Screen.height < 720)
		{
			Screen.SetResolution(1280, 720, fullscreen: false);
		}
		startTimeStamp = Time.time;
		startActive = true;
		Object.DontDestroyOnLoad(new GameObject("StatisticsManager").AddComponent<StatisticsManager>().gameObject);
	}

	private void Update()
	{
		if (startActive && Time.time - startTimeStamp >= 0.5f)
		{
			startActive = false;
			presentRSLogo();
		}
	}

	private void presentRSLogo()
	{
		myAS.Play();
		Sequence sequence = DOTween.Sequence().OnComplete(loadTitleScreen);
		sequence.Insert(0f, DOTween.To(() => rsLeftRT.anchoredPosition, delegate(Vector2 x)
		{
			rsLeftRT.anchoredPosition = x;
		}, defaultLogoLeftPOS, 0.6f).SetEase(Ease.InSine));
		sequence.Insert(0f, DOTween.To(() => rsRightRT.anchoredPosition, delegate(Vector2 x)
		{
			rsRightRT.anchoredPosition = x;
		}, defaultLogoRightPOS, 0.6f).SetEase(Ease.InSine));
		sequence.Insert(0f, DOTween.To(() => rsLeftCG.alpha, delegate(float x)
		{
			rsLeftCG.alpha = x;
		}, 0.5f, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(0f, DOTween.To(() => rsRightCG.alpha, delegate(float x)
		{
			rsRightCG.alpha = x;
		}, 0.5f, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(1.4f, DOTween.To(() => rsLeftCG.alpha, delegate(float x)
		{
			rsLeftCG.alpha = x;
		}, 0f, 1f).SetEase(Ease.Linear));
		sequence.Insert(1.4f, DOTween.To(() => rsRightCG.alpha, delegate(float x)
		{
			rsRightCG.alpha = x;
		}, 0f, 1f).SetEase(Ease.Linear));
		sequence.Insert(1.4f, DOTween.To(() => rsLogoCG.alpha, delegate(float x)
		{
			rsLogoCG.alpha = x;
		}, 0.6f, 1f).SetEase(Ease.Linear));
		sequence.Insert(1.6f, DOTween.To(() => rsTextCG.alpha, delegate(float x)
		{
			rsTextCG.alpha = x;
		}, 0.6f, 1f).SetEase(Ease.Linear));
		sequence.Insert(3.25f, DOTween.To(() => rsWebsiteCG.alpha, delegate(float x)
		{
			rsWebsiteCG.alpha = x;
		}, 0.6f, 1f).SetEase(Ease.Linear));
		sequence.Insert(4.75f, DOTween.To(() => rsLogoCG.alpha, delegate(float x)
		{
			rsLogoCG.alpha = x;
		}, 1f, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(4.75f, DOTween.To(() => rsTextCG.alpha, delegate(float x)
		{
			rsTextCG.alpha = x;
		}, 1f, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(4.75f, DOTween.To(() => rsWebsiteCG.alpha, delegate(float x)
		{
			rsWebsiteCG.alpha = x;
		}, 1f, 0.75f).SetEase(Ease.Linear));
		sequence.Insert(4.75f, DOTween.To(() => camBloom.bloomIntensity, delegate(float x)
		{
			camBloom.bloomIntensity = x;
		}, 0.25f, 2.5f).SetEase(Ease.Linear));
		sequence.Insert(7.25f, DOTween.To(() => rsLogoCG.alpha, delegate(float x)
		{
			rsLogoCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(7.25f, DOTween.To(() => rsTextCG.alpha, delegate(float x)
		{
			rsTextCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(7.25f, DOTween.To(() => rsWebsiteCG.alpha, delegate(float x)
		{
			rsWebsiteCG.alpha = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		sequence.Insert(7.25f, DOTween.To(() => camBloom.bloomIntensity, delegate(float x)
		{
			camBloom.bloomIntensity = x;
		}, 0f, 0.25f).SetEase(Ease.Linear));
		sequence.Play();
	}

	private void loadTitleScreen()
	{
		SceneManager.LoadScene(1);
	}
}
