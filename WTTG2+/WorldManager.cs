using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
	public delegate void WorldManagerActions();

	[SerializeField]
	private bool debugMode;

	[SerializeField]
	private int[] worldIDs;

	[SerializeField]
	private VPNCurrencyDefinition defaultVPNCurrency;

	private List<LocationVolume> activeLocationVolumes = new List<LocationVolume>(20);

	private List<LocationVolume> currentLocationVolumes = new List<LocationVolume>(20);

	private int worldIDIndex;

	public static bool DevTeleportation;

	public List<VPNVolume> CurrentVPNVolumes { get; } = new List<VPNVolume>(70);

	public event WorldManagerActions WorldLoaded;

	private void Awake()
	{
		GameManager.WorldManager = this;
		worldIDIndex = 0;
	}

	private void Update()
	{
		checkPlayerLocation();
	}

	public void StageGame()
	{
		if (!debugMode)
		{
			GameManager.TimeSlinger.FireTimer(1.5f, loadInWorld, worldIDs[worldIDIndex]);
		}
	}

	public void loadInWorld(int worldID)
	{
		StartCoroutine(loadWorld(worldID));
	}

	public void AddLocationVolume(LocationVolume SetVolume)
	{
		currentLocationVolumes.Add(SetVolume);
	}

	public void AddActiveLocationVolume(LocationVolume SetVolume)
	{
		if (!activeLocationVolumes.Contains(SetVolume))
		{
			activeLocationVolumes.Add(SetVolume);
		}
	}

	public void RemoveActiveLocationVolume(LocationVolume SetVolume)
	{
		activeLocationVolumes.Remove(SetVolume);
	}

	public void AddVPNVolume(VPNVolume SetVolume)
	{
		CurrentVPNVolumes.Add(SetVolume);
	}

	public void SetVPNValues(RemoteVPNObject TheRemoteVPN)
	{
		bool flag = false;
		for (int i = 0; i < CurrentVPNVolumes.Count; i++)
		{
			if (CurrentVPNVolumes[i].VPNInRange(TheRemoteVPN))
			{
				flag = true;
				i = CurrentVPNVolumes.Count;
			}
		}
		if (!flag)
		{
			TheRemoteVPN.SetCurrency(defaultVPNCurrency);
		}
	}

	public float GetVPNValues(Transform ThePosition)
	{
		float TimeValue = 2500f;
		for (int i = 0; i < CurrentVPNVolumes.Count; i++)
		{
			if (CurrentVPNVolumes[i].VPNRangeCheck(ThePosition, out TimeValue))
			{
				i = CurrentVPNVolumes.Count;
			}
		}
		return TimeValue;
	}

	private void checkPlayerLocation()
	{
		if (activeLocationVolumes.Count > 0)
		{
			StateManager.PlayerLocation = activeLocationVolumes[activeLocationVolumes.Count - 1].Location;
			if (DevTeleportation && StateManager.PlayerLocation == PLAYER_LOCATION.MAIN_ROON)
			{
				DevTeleportation = false;
				GameManager.AudioSlinger.UnMuffleAudioLayer(AUDIO_LAYER.COMPUTER_SFX);
				GameManager.AudioSlinger.UnMuffleAudioLayer(AUDIO_LAYER.HACKING_SFX);
				GameManager.AudioSlinger.UnMuffleAudioLayer(AUDIO_LAYER.WEBSITE);
			}
		}
		else
		{
			StateManager.PlayerLocation = PLAYER_LOCATION.UNKNOWN;
		}
	}

	private IEnumerator loadWorld(int worldID)
	{
		AsyncOperation result = SceneManager.LoadSceneAsync(worldID, LoadSceneMode.Additive);
		while (!result.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		worldIDIndex++;
		if (DifficultyManager.HackerMode)
		{
			if (worldIDIndex != 2)
			{
				StageGame();
			}
			else if (this.WorldLoaded != null)
			{
				this.WorldLoaded();
			}
		}
		else if (worldIDIndex < worldIDs.Length)
		{
			StageGame();
		}
		else if (this.WorldLoaded != null)
		{
			this.WorldLoaded();
		}
	}
}
