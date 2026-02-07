using UnityEngine;

public class PlayerAudioBehaviour : MonoBehaviour
{
	[SerializeField]
	private int maxMicThreshold = 20;

	public CustomEvent<float> CurrentPlayersLoudLevel = new CustomEvent<float>(5);

	private bool hasMic;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}
}
