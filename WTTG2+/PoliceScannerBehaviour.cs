using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
[RequireComponent(typeof(InteractionHook))]
public class PoliceScannerBehaviour : MonoBehaviour
{
	public static PoliceScannerBehaviour Ins;

	public AudioFileDefinition onSFX;

	public AudioFileDefinition offSFX;

	[SerializeField]
	private AudioFileDefinition[] policeBanterSFXS;

	[SerializeField]
	private AudioFileDefinition[] warningSFXS;

	[SerializeField]
	private float fireWindowMin = 10f;

	[SerializeField]
	private float fireWindowMax = 20f;

	[SerializeField]
	private Material emsMat;

	private List<AudioFileDefinition> currentPlayingClips = new List<AudioFileDefinition>(3);

	[NonSerialized]
	private AudioFileDefinition currentBanterClip;

	private float fireTimeStamp;

	private float fireTimeWindow;

	private float freezeAddTime;

	private float freezeTimeStamp;

	private bool gameIsPaused;

	private AudioHubObject myAudioHub;

	private InteractionHook myInteractionHook;

	private MeshRenderer myMeshRenderer;

	private int playingPolice;

	private bool exed;

	private AudioHubObject secondAHO;

	public bool IsOn { get; private set; }

	public bool ownPoliceScanner { get; private set; }

	public string ScannerDebug => ((fireTimeWindow - (Time.time - freezeAddTime - fireTimeStamp) > 0f) ? ((int)(fireTimeWindow - (Time.time - freezeAddTime - fireTimeStamp))).ToString() : 0.ToString()) + " -> " + playingPolice;

	private void Update()
	{
		if (!gameIsPaused && IsOn && Time.time - freezeAddTime - fireTimeStamp >= fireTimeWindow)
		{
			generateFireWindow();
			triggerPoliceBanter();
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= leftClickAction;
		GameManager.PauseManager.GamePaused -= playerHitPause;
		GameManager.PauseManager.GameUnPaused -= playerHitUnPause;
		currentBanterClip = null;
	}

	public void SoftBuild()
	{
		Ins = this;
		myMeshRenderer = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myAudioHub = GetComponent<AudioHubObject>();
		secondAHO = base.gameObject.AddComponent<AudioHubObject>();
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.Euler(Vector3.zero);
		myMeshRenderer.enabled = false;
		myInteractionHook.LeftClickAction += leftClickAction;
		GameManager.PauseManager.GamePaused += playerHitPause;
		GameManager.PauseManager.GameUnPaused += playerHitUnPause;
		exed = false;
	}

	public void MoveMe(Vector3 SetPOS, Vector3 SetROT)
	{
		ownPoliceScanner = true;
		myMeshRenderer.enabled = true;
		IsOn = true;
		base.transform.position = SetPOS;
		base.transform.rotation = Quaternion.Euler(SetROT);
		IsOn = true;
		emsMat.EnableKeyword("_EMISSION");
		EnemyManager.PoliceManager.FireWarning += TriggerWarning;
		generateFireWindow();
	}

	public void TriggerWarning()
	{
		if (IsOn)
		{
			generateFireWindow();
			currentBanterClip = warningSFXS[UnityEngine.Random.Range(0, warningSFXS.Length)];
			ReadyPlayAudio();
		}
	}

	private void playerHitPause()
	{
		freezeTimeStamp = Time.time;
		gameIsPaused = true;
	}

	private void playerHitUnPause()
	{
		freezeAddTime += Time.time - freezeTimeStamp;
		gameIsPaused = false;
	}

	private void leftClickAction()
	{
		toggleScanner();
	}

	private void toggleScanner()
	{
		if (IsOn)
		{
			IsOn = false;
			emsMat.DisableKeyword("_EMISSION");
			secondAHO.PlaySound(offSFX);
			myAudioHub.MuteHub();
		}
		else
		{
			myAudioHub.UnMuteHub();
			secondAHO.PlaySound(onSFX);
			IsOn = true;
			emsMat.EnableKeyword("_EMISSION");
		}
	}

	private void generateFireWindow()
	{
		fireTimeWindow = UnityEngine.Random.Range(fireWindowMin, fireWindowMax);
		fireTimeStamp = Time.time;
	}

	public void generateSmallFireWindow()
	{
		fireTimeWindow = UnityEngine.Random.Range(5f, 10f);
		fireTimeStamp = Time.time;
	}

	private void triggerPoliceBanter()
	{
		currentBanterClip = policeBanterSFXS[UnityEngine.Random.Range(0, policeBanterSFXS.Length)];
		ReadyPlayAudio();
	}

	private void ReadyPlayAudio()
	{
		playingPolice++;
		myAudioHub.PlaySound(currentBanterClip);
		GameManager.TimeSlinger.FireTimer(currentBanterClip.AudioClip.length + 0.65f, delegate
		{
			playingPolice--;
		});
	}
}
