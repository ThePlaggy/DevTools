using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class SpawnToDeadDropTrigger : MonoBehaviour
{
	public static SpawnToDeadDropTrigger Ins;

	public Vector3 DeadDropPOS = Vector3.zero;

	public Vector3 DeadDropROT = Vector3.zero;

	private InteractionHook myInteractionHook;

	public CustomEvent PlayerSpawningToDeadDropEvent = new CustomEvent(2);

	private void Awake()
	{
		Ins = this;
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += spawnPlayerToDeadDrop;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= spawnPlayerToDeadDrop;
	}

	private void spawnPlayerToDeadDrop()
	{
		if (!GameManager.PauseManager.Paused)
		{
			PlayerSpawningToDeadDropEvent.Execute();
			EnemyManager.BreatherManager.PlayerSpawnedToDeadDrop();
			ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).SpawnMeTo(DeadDropPOS, DeadDropROT, 1f);
		}
	}
}
