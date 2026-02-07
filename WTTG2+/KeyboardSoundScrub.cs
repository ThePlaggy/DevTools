using UnityEngine;
using UnityEngine.UI;

public class KeyboardSoundScrub : MonoBehaviour
{
	private InputField myTextInput;

	private void Awake()
	{
		myTextInput = GetComponent<InputField>();
	}

	private void Update()
	{
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER && myTextInput.isFocused)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				processSpaceReturnSFX();
			}
			else if (Input.GetKeyDown(KeyCode.Return))
			{
				processSpaceReturnSFX();
			}
			else if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
			{
				processKeySFX();
			}
		}
		if (StateManager.PlayerState == PLAYER_STATE.COMPUTER && KAttack.IsInAttack && !Input.GetKeyDown(KeyCode.Return) && Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
		{
			GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.KeyReturn);
			if (EnemyStateManager.HasEnemyState(ENEMY_STATE.EXECUTIONER))
			{
				EXESoundPopper.PopSound(1);
			}
		}
	}

	private void processKeySFX()
	{
		int num = Random.Range(1, LookUp.SoundLookUp.KeyboardSounds.Length);
		AudioFileDefinition audioFileDefinition = LookUp.SoundLookUp.KeyboardSounds[num];
		GameManager.AudioSlinger.PlaySound(audioFileDefinition);
		LookUp.SoundLookUp.KeyboardSounds[num] = LookUp.SoundLookUp.KeyboardSounds[num];
		LookUp.SoundLookUp.KeyboardSounds[0] = audioFileDefinition;
	}

	private void processSpaceReturnSFX()
	{
		GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.KeyboardSpaceReturnSound);
	}
}
