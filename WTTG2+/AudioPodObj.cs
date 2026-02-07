using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
public class AudioPodObj : MonoBehaviour
{
	public List<AudioFileDefinition> MagicFireSFXS = new List<AudioFileDefinition>();

	public bool MagicFire;

	public float MagicFireWindowMin = 5f;

	public float MagicFireWindowMax = 30f;

	public AudioHubObject MyAudioHubObject;

	private float freezeAddTime;

	private float freezeTimeStamp;

	private bool gameIsPaused;

	private bool isUniversal;

	private bool magicFireActive;

	private float magicFireTimeStamp;

	private float magicFireWindow;

	private void Awake()
	{
		MyAudioHubObject = GetComponent<AudioHubObject>();
		if (MyAudioHubObject.MyAudioHub == AUDIO_HUB.UNIVERSAL)
		{
			isUniversal = true;
		}
		GameManager.PauseManager.GamePaused += playerPausedGame;
		GameManager.PauseManager.GameUnPaused += playerUnPausedGame;
	}

	private void Update()
	{
		if (magicFireActive && !gameIsPaused && Time.time - freezeAddTime - magicFireTimeStamp >= magicFireWindow)
		{
			magicFireActive = false;
			freezeTimeStamp = 0f;
			freezeAddTime = 0f;
			if (isUniversal)
			{
				MyAudioHubObject.PlaySound(MagicFireSFXS[Random.Range(0, MagicFireSFXS.Count)]);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(MagicFireSFXS[Random.Range(0, MagicFireSFXS.Count)]);
			}
			GenerateMagicFire();
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= playerPausedGame;
		GameManager.PauseManager.GameUnPaused -= playerUnPausedGame;
	}

	public void GenerateMagicFire()
	{
		magicFireWindow = Random.Range(MagicFireWindowMin, MagicFireWindowMax);
		magicFireTimeStamp = Time.time;
		magicFireActive = true;
	}

	private void playerPausedGame()
	{
		if (MagicFire)
		{
			freezeTimeStamp = Time.time;
			gameIsPaused = true;
		}
	}

	private void playerUnPausedGame()
	{
		if (MagicFire)
		{
			freezeAddTime += Time.time - freezeTimeStamp;
			gameIsPaused = false;
		}
	}
}
