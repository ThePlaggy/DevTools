using System;
using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class EasterEggTrigger : MonoBehaviour
{
	[SerializeField]
	private Texture2D[] textureArray;

	[NonSerialized]
	public MeshRenderer mesh;

	[NonSerialized]
	public InteractionHook myInteractionHook;

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = base.gameObject.AddComponent<InteractionHook>();
		myInteractionHook.StateActive = PLAYER_STATE.ROAMING;
		myInteractionHook.LeftClickAction += pickegg;
	}

	private void Start()
	{
		EasterEggManager.EasterEggsLeft = 50;
		Material material = new Material(Shader.Find("Standard"));
		material.mainTexture = textureArray[UnityEngine.Random.Range(0, textureArray.Length)];
		GetComponent<MeshRenderer>().material = material;
		EasterEggManager.Ins.AddEgg(this);
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= pickegg;
	}

	private void pickegg()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
		EasterEggManager.EasterEggsLeft--;
		EasterEggManager.Ins.eggCounter.text = 50 - EasterEggManager.EasterEggsLeft + "/50";
		if (EasterEggManager.EasterEggsLeft <= 0)
		{
			EasterEggManager.EventCompleted = true;
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.tadaaa);
			EventRewardManager.EasterReward();
		}
	}
}
