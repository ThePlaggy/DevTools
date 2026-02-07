using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class FireworksTrigger : MonoBehaviour
{
	public GameObject[] rockets;

	private MeshRenderer mesh;

	private InteractionHook myInteractionHook;

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += launchFirework;
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= launchFirework;
	}

	private void launchFirework()
	{
		GameObject gameObject = Object.Instantiate(rockets[Random.Range(0, 5)]);
		gameObject.transform.position = base.transform.position;
	}

	private void stageMe()
	{
		GameManager.StageManager.TheGameIsLive -= stageMe;
	}
}
