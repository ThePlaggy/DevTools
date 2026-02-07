using UnityEngine;

public class AudioLayerMuffleTrigger : MonoBehaviour
{
	[SerializeField]
	private AUDIO_LAYER[] muffleLayers = new AUDIO_LAYER[0];

	[SerializeField]
	private float muffleTime;

	[SerializeField]
	private float muffleAmount;

	[SerializeField]
	private PLAYER_LOCATION validLocationForMuffle;

	[SerializeField]
	private PLAYER_LOCATION validLocationForUnMuffle;

	public float MuffleAmount
	{
		get
		{
			return muffleAmount;
		}
		set
		{
			muffleAmount = value;
		}
	}

	public void MuffleLayer()
	{
		if (StateManager.PlayerLocation == validLocationForMuffle)
		{
			for (int i = 0; i < muffleLayers.Length; i++)
			{
				GameManager.AudioSlinger.MuffleAudioLayer(muffleLayers[i], muffleAmount, muffleTime);
			}
		}
		GameManager.TimeSlinger.FireTimer(muffleTime + 0.5f, delegate
		{
			if (StateManager.PlayerLocation != validLocationForMuffle)
			{
				for (int j = 0; j < muffleLayers.Length; j++)
				{
					GameManager.AudioSlinger.UnMuffleAudioLayer(muffleLayers[j]);
				}
			}
		});
	}

	public void UnMuffleLayer()
	{
		if (StateManager.PlayerLocation == validLocationForUnMuffle)
		{
			for (int i = 0; i < muffleLayers.Length; i++)
			{
				GameManager.AudioSlinger.UnMuffleAudioLayer(muffleLayers[i], muffleTime);
			}
		}
	}
}
