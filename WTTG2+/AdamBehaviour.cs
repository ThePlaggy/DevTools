using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AdamBehaviour : MonoBehaviour
{
	public static AdamBehaviour Ins;

	[SerializeField]
	private AudioFileDefinition thanksSFX;

	[SerializeField]
	private AudioFileDefinition letHerGo;

	private Animator myAC;

	private void Awake()
	{
		Ins = this;
		myAC = GetComponent<Animator>();
	}

	public void ProcessEndingPrompt(EndingPromptDefinition ThePrompt)
	{
		if (ThePrompt.HasAnimationAudio)
		{
			GameManager.AudioSlinger.PlaySound(ThePrompt.AnimationAudioFile);
		}
	}

	public void ProcessEndingStep(EndingStepDefinition TheStep)
	{
		if (EndingManager.Ins != null)
		{
			EndingManager.Ins.ManualProcessEndingStep(TheStep);
		}
	}

	public void CallAniTrigger(string SetTrigger)
	{
		myAC.SetTrigger(SetTrigger);
	}

	public void PlayerChoiceLife()
	{
		EndingManager.Ins.PlayerChoiceLife();
	}

	public void PlayerChoiceDeath()
	{
		EndingManager.Ins.PlayerChoiceDeath();
	}

	public void ThanksForPlaying()
	{
		GameManager.AudioSlinger.PlaySound(thanksSFX);
	}

	public void LetHerGo()
	{
		GameManager.AudioSlinger.PlaySound(letHerGo);
	}
}
