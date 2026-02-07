using UnityEngine;

[RequireComponent(typeof(AudioHubObject))]
public class AudioPodObject : MonoBehaviour
{
	[SerializeField]
	private AudioFileDefinition[] gameIsStagingSFXS = new AudioFileDefinition[0];

	[SerializeField]
	private AudioFileDefinition[] gameIsLiveSFXS = new AudioFileDefinition[0];

	[SerializeField]
	private bool magicFire;

	[SerializeField]
	private float magicFireWindowMin = 5f;

	[SerializeField]
	private float magicFireWindowMax = 30f;

	[SerializeField]
	private AudioFileDefinition[] magicFireSFXS = new AudioFileDefinition[0];

	private float freezeAddTime;

	private float freezeTimeStamp;

	private bool gameIsPaused;

	private bool isUniversal;

	private bool magicFireActive;

	private float magicFireTimeStamp;

	private float magicFireWindow;

	private AudioHubObject myAudioHubObject;

	private void Awake()
	{
		myAudioHubObject = GetComponent<AudioHubObject>();
		if (myAudioHubObject.MyAudioHub == AUDIO_HUB.UNIVERSAL)
		{
			isUniversal = true;
		}
		GameManager.StageManager.TheGameIsStageing += gameIsStaging;
		GameManager.StageManager.TheGameIsLive += gameIsLive;
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
				myAudioHubObject.PlaySound(magicFireSFXS[Random.Range(0, magicFireSFXS.Length)]);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(magicFireSFXS[Random.Range(0, magicFireSFXS.Length)]);
			}
			generateMagicFire();
		}
	}

	private void OnDestroy()
	{
		GameManager.PauseManager.GamePaused -= playerPausedGame;
		GameManager.PauseManager.GameUnPaused -= playerUnPausedGame;
	}

	private void gameIsStaging()
	{
		for (int i = 0; i < gameIsStagingSFXS.Length; i++)
		{
			if (isUniversal)
			{
				myAudioHubObject.PlaySound(gameIsStagingSFXS[i]);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(gameIsStagingSFXS[i]);
			}
		}
		GameManager.StageManager.TheGameIsStageing -= gameIsStaging;
	}

	private void gameIsLive()
	{
		for (int i = 0; i < gameIsLiveSFXS.Length; i++)
		{
			if (isUniversal)
			{
				myAudioHubObject.PlaySound(gameIsLiveSFXS[i]);
			}
			else
			{
				GameManager.AudioSlinger.PlaySound(gameIsLiveSFXS[i]);
			}
		}
		if (magicFire)
		{
			generateMagicFire();
		}
		GameManager.StageManager.TheGameIsLive -= gameIsLive;
	}

	private void generateMagicFire()
	{
		magicFireWindow = Random.Range(magicFireWindowMin, magicFireWindowMax);
		magicFireTimeStamp = Time.time;
		magicFireActive = true;
	}

	private void playerPausedGame()
	{
		if (magicFire)
		{
			freezeTimeStamp = Time.time;
			gameIsPaused = true;
		}
	}

	private void playerUnPausedGame()
	{
		if (magicFire)
		{
			freezeAddTime += Time.time - freezeTimeStamp;
			gameIsPaused = false;
		}
	}
}
