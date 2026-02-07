using System.Collections.Generic;
using UnityEngine;

public class EndingResponseManager : MonoBehaviour
{
	private const float RESPONSE_SPACING = 45f;

	private const float RESPONSE_PRESENT_DELAY_TIME = 1f;

	[SerializeField]
	private GameObject endingResponseObject;

	[SerializeField]
	private RectTransform endingResponseObjectHolder;

	[SerializeField]
	private EndController endController;

	[SerializeField]
	private AdamBehaviour adamBehaviour;

	private List<EndingResponseObject> currentEndingResponses = new List<EndingResponseObject>(2);

	private EndingStepDefinition currentEndingStep;

	private PooledStack<EndingResponseObject> endingResponseObjectPool;

	private void Awake()
	{
		endingResponseObjectPool = new PooledStack<EndingResponseObject>(delegate
		{
			EndingResponseObject component = Object.Instantiate(endingResponseObject, endingResponseObjectHolder).GetComponent<EndingResponseObject>();
			component.SoftBuild();
			return component;
		}, 2);
	}

	public void ProcessPlayerReponse(EndingStepDefinition TheStep)
	{
		currentEndingStep = TheStep;
		float num = 0f;
		float num2 = 45f * (float)(currentEndingStep.ResponseOptions.Count - 1);
		for (int i = 0; i < currentEndingStep.ResponseOptions.Count; i++)
		{
			EndingResponseObject endingResponseObject = endingResponseObjectPool.Pop();
			endingResponseObject.Build(currentEndingStep.ResponseOptions[i], i, num2, num);
			currentEndingResponses.Add(endingResponseObject);
			num += 1f;
			num2 -= 45f;
		}
		num = 1f * (float)currentEndingStep.ResponseOptions.Count;
		GameManager.TimeSlinger.FireTimer(num, delegate
		{
			endController.PlayerSelectedOptionOne.Event += playerChoseOptionOne;
			endController.PlayerSelectedOptionTwo.Event += playerChoseOptionTwo;
			endController.AllowPlayerResponseSelection = true;
		});
	}

	private void playerChoseOptionOne()
	{
		endController.PlayerSelectedOptionOne.Event -= playerChoseOptionOne;
		endController.PlayerSelectedOptionTwo.Event -= playerChoseOptionTwo;
		currentEndingResponses[0].Dismiss(1.5f);
		currentEndingResponses[1].Dismiss(0f);
		clearCurrentResponses();
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			adamBehaviour.CallAniTrigger(currentEndingStep.ResponseOptions[0].ResponseAnimationTrigger);
			currentEndingStep = null;
		});
	}

	private void playerChoseOptionTwo()
	{
		endController.PlayerSelectedOptionOne.Event -= playerChoseOptionOne;
		endController.PlayerSelectedOptionTwo.Event -= playerChoseOptionTwo;
		currentEndingResponses[0].Dismiss(0f);
		currentEndingResponses[1].Dismiss(1.5f);
		clearCurrentResponses();
		GameManager.TimeSlinger.FireTimer(2f, delegate
		{
			adamBehaviour.CallAniTrigger(currentEndingStep.ResponseOptions[1].ResponseAnimationTrigger);
			currentEndingStep = null;
		});
	}

	private void clearCurrentResponses()
	{
		for (int i = 0; i < currentEndingResponses.Count; i++)
		{
			endingResponseObjectPool.Push(currentEndingResponses[i]);
		}
		currentEndingResponses.Clear();
	}
}
