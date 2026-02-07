using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFDManager : MonoBehaviour
{
	public static AFDManager Ins;

	private int dynamicId;

	private Dictionary<AFD_TYPE, AUDIO_HUB> MyHubs = new Dictionary<AFD_TYPE, AUDIO_HUB>();

	private Dictionary<AFD_TYPE, AUDIO_LAYER> MyLayers = new Dictionary<AFD_TYPE, AUDIO_LAYER>();

	private Dictionary<AFD_TYPE, AudioSourceDefinition> MySources = new Dictionary<AFD_TYPE, AudioSourceDefinition>();

	public AFDManager()
	{
		dynamicId = 254090;
		AudioSourceDefinition audioSourceDefinition = ScriptableObject.CreateInstance<AudioSourceDefinition>();
		AudioSourceDefinition audioSourceDefinition2 = ScriptableObject.CreateInstance<AudioSourceDefinition>();
		AudioSourceDefinition audioSourceDefinition3 = ScriptableObject.CreateInstance<AudioSourceDefinition>();
		AudioSourceDefinition audioSourceDefinition4 = ScriptableObject.CreateInstance<AudioSourceDefinition>();
		audioSourceDefinition.id = 979419;
		audioSourceDefinition.SpatialBlend = 1f;
		audioSourceDefinition.ReverbZoneMix = 1f;
		audioSourceDefinition.MinDistance = 0.75f;
		audioSourceDefinition.MaxDistance = 5f;
		audioSourceDefinition2.id = 979407;
		audioSourceDefinition2.SpatialBlend = 1f;
		audioSourceDefinition2.ReverbZoneMix = 1f;
		audioSourceDefinition2.DopplerLevel = 1f;
		audioSourceDefinition2.MinDistance = 2f;
		audioSourceDefinition2.MaxDistance = 8f;
		audioSourceDefinition3.id = 979401;
		audioSourceDefinition3.SpatialBlend = 1f;
		audioSourceDefinition3.ReverbZoneMix = 1f;
		audioSourceDefinition3.DopplerLevel = 1f;
		audioSourceDefinition3.MinDistance = 2f;
		audioSourceDefinition3.MaxDistance = 8f;
		audioSourceDefinition4.id = 979405;
		audioSourceDefinition4.SpatialBlend = 0.814f;
		audioSourceDefinition4.ReverbZoneMix = 0.538f;
		audioSourceDefinition4.Spread = 334f;
		audioSourceDefinition4.MinDistance = 1.35f;
		audioSourceDefinition4.MaxDistance = 15.27f;
		MyHubs.Add(AFD_TYPE.PLAYER, AUDIO_HUB.PLAYER_HUB);
		MyHubs.Add(AFD_TYPE.ENEMY, AUDIO_HUB.UNIVERSAL);
		MyHubs.Add(AFD_TYPE.COMPUTER, AUDIO_HUB.COMPUTER_HUB);
		MyHubs.Add(AFD_TYPE.OUTSIDE, AUDIO_HUB.UNIVERSAL);
		MyLayers.Add(AFD_TYPE.PLAYER, AUDIO_LAYER.GAME_OVER);
		MyLayers.Add(AFD_TYPE.ENEMY, AUDIO_LAYER.ENEMY);
		MyLayers.Add(AFD_TYPE.COMPUTER, AUDIO_LAYER.ENVIRONMENT);
		MyLayers.Add(AFD_TYPE.OUTSIDE, AUDIO_LAYER.OUTSIDE);
		MySources.Add(AFD_TYPE.PLAYER, audioSourceDefinition);
		MySources.Add(AFD_TYPE.COMPUTER, audioSourceDefinition2);
		MySources.Add(AFD_TYPE.ENEMY, audioSourceDefinition3);
		MySources.Add(AFD_TYPE.OUTSIDE, audioSourceDefinition4);
	}

	private void Awake()
	{
		Ins = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnDestroy()
	{
		Ins = null;
	}

	public IEnumerator CreateAFDAsync(AssetBundle AB, string Clip, AFD_TYPE Type, float Volume, bool Loop, Action<AudioFileDefinition> callback)
	{
		AudioFileDefinition audioFileDefinition = ScriptableObject.CreateInstance<AudioFileDefinition>();
		dynamicId = (audioFileDefinition.id = dynamicId) + 1;
		audioFileDefinition.MyAudioHub = MyHubs[Type];
		audioFileDefinition.MyAudioLayer = MyLayers[Type];
		audioFileDefinition.MyAudioSourceDefinition = MySources[Type];
		audioFileDefinition.Volume = Volume;
		audioFileDefinition.Delay = false;
		audioFileDefinition.DelayAmount = 0f;
		audioFileDefinition.Loop = Loop;
		audioFileDefinition.LoopCount = -1;
		yield return LoadAudioClipAsync(audioFileDefinition, AB, Clip);
		callback?.Invoke(audioFileDefinition);
	}

	private static IEnumerator LoadAudioClipAsync(AudioFileDefinition audioFileDefinition, AssetBundle AB, string Clip)
	{
		AssetBundleRequest request = AB.LoadAssetAsync<AudioClip>(Clip);
		yield return request;
		audioFileDefinition.AudioClip = request.asset as AudioClip;
	}
}
