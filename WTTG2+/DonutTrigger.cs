using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class DonutTrigger : MonoBehaviour
{
	private MeshRenderer mesh;

	private InteractionHook myInteractionHook;

	public static bool Eaten;

	public static void BuildTrigger()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "DonutTrigger";
		gameObject.layer = 9;
		gameObject.AddComponent<DonutTrigger>();
		gameObject.GetComponent<InteractionHook>().StateActive = PLAYER_STATE.ROAMING;
		gameObject.GetComponent<InteractionHook>().RequireLocationCheck = true;
		gameObject.GetComponent<InteractionHook>().LocationToCheck = PLAYER_LOCATION.LOBBY;
		gameObject.transform.position = new Vector3(3.0736f, 1.09f, -14.5964f);
		gameObject.transform.localScale = new Vector3(0.5f, 0.3f, 0.3f);
		gameObject.GetComponent<MeshRenderer>().enabled = false;
	}

	private void Awake()
	{
		mesh = GetComponent<MeshRenderer>();
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += EatDonut;
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= EatDonut;
	}

	private void EatDonut()
	{
		if (Eaten)
		{
			return;
		}
		myInteractionHook.ForceLock = true;
		mesh.enabled = false;
		Eaten = true;
		GameObject.Find("DoughnutBox").SetActive(value: false);
		if (TarotManager.CurSpeed == playerSpeedMode.WEAK)
		{
			TarotManager.CurSpeed = playerSpeedMode.NORMAL;
		}
		else if (TarotManager.CurSpeed != playerSpeedMode.QUICK)
		{
			TarotManager.CurSpeed = playerSpeedMode.QUICK;
			GameManager.TimeSlinger.FireTimer(10f, delegate
			{
				TarotManager.CurSpeed = playerSpeedMode.NORMAL;
			});
		}
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.omnom);
	}
}
