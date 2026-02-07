using UnityEngine;

public class BlueWhisperManager : MonoBehaviour
{
	public static BlueWhisperManager Ins;

	[SerializeField]
	private AudioSourceDefinition blueWhisperAudioSource;

	private BlueWhisperData myData;

	private int myID;

	public bool Owns => myData.Owns;

	private void Awake()
	{
		Ins = this;
		myID = base.transform.position.GetHashCode();
		GameManager.StageManager.Stage += stageMe;
		GameManager.StageManager.TheGameIsLive += gameLive;
	}

	private void OnDestroy()
	{
	}

	public void PickedUpBlueWhisper()
	{
		if (myData != null)
		{
			myData.Pending = false;
			myData.Owns = true;
			DataManager.Save(myData);
		}
	}

	public void ProcessSound(AudioFileDefinition TheSound)
	{
		if (myData.Owns)
		{
			AudioFileDefinition audioFileDefinition = new AudioFileDefinition(TheSound);
			audioFileDefinition.MyAudioSourceDefinition = blueWhisperAudioSource;
			audioFileDefinition.MyAudioHub = AUDIO_HUB.BLUE_WHISPER_HUB;
			audioFileDefinition.MyAudioLayer = AUDIO_LAYER.BLUE_WHISPER;
			audioFileDefinition.Volume = 0.2f;
			GameManager.AudioSlinger.PlaySound(audioFileDefinition);
		}
	}

	private void productWasPickedUp(ShadowMarketProductDefinition TheProduct)
	{
		HARDWARE_PRODUCTS productID = TheProduct.productID;
		if (productID == HARDWARE_PRODUCTS.BLUE_WHISPER && myData != null && !myData.Owns)
		{
			myData.Pending = true;
			DataManager.Save(myData);
			BlueWhisperBehaviour.Ins.SpawnMe();
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<BlueWhisperData>(myID);
		if (myData == null)
		{
			myData = new BlueWhisperData(myID);
			myData.Pending = false;
			myData.Owns = false;
		}
		GameManager.StageManager.Stage -= stageMe;
		GameManager.ManagerSlinger.ProductsManager.ShadowMarketProductWasActivated.Event += productWasPickedUp;
	}

	private void gameLive()
	{
		if (myData.Pending)
		{
			BlueWhisperBehaviour.Ins.SpawnMe();
		}
		GameManager.StageManager.TheGameIsLive -= gameLive;
	}
}
