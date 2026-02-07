using UnityEngine;

public class TutorialStepper : MonoBehaviour
{
	[SerializeField]
	protected TutorialStepDefinition[] steps = new TutorialStepDefinition[0];

	public CustomEvent TutoralHasEndedEvents = new CustomEvent(5);
}
