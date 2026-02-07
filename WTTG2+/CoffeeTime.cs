using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class CoffeeTime : MonoBehaviour
{
	private InteractionHook myInteractionHook;

	public bool isCoffee;

	private static AudioHubObject AHO;

	public static bool HaveCoffee;

	public static InteractionHook CoffeeIns;

	public static bool scary;

	public static void BuildTrigger()
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject.name = "CoffeePacks_Trigger";
		gameObject.layer = 9;
		gameObject.AddComponent<CoffeeTime>().isCoffee = true;
		gameObject.GetComponent<InteractionHook>().StateActive = PLAYER_STATE.ROAMING;
		gameObject.GetComponent<InteractionHook>().RequireLocationCheck = true;
		gameObject.GetComponent<InteractionHook>().LocationToCheck = PLAYER_LOCATION.LOBBY;
		gameObject.transform.position = new Vector3(1.4f, 1.08f, -14.7327f);
		gameObject.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
		gameObject2.name = "CoffeeMaker_Trigger";
		gameObject2.layer = 9;
		gameObject2.AddComponent<CoffeeTime>().isCoffee = false;
		gameObject2.GetComponent<InteractionHook>().StateActive = PLAYER_STATE.ROAMING;
		gameObject2.GetComponent<InteractionHook>().RequireLocationCheck = true;
		gameObject2.GetComponent<InteractionHook>().LocationToCheck = PLAYER_LOCATION.LOBBY;
		gameObject2.GetComponent<InteractionHook>().ForceLock = true;
		CoffeeIns = gameObject2.GetComponent<InteractionHook>();
		gameObject2.transform.position = new Vector3(1.88f, 1.08f, -14.6327f);
		gameObject2.transform.localScale = new Vector3(0.2318f, 0.8864f, 0.3227f);
		gameObject2.GetComponent<MeshRenderer>().enabled = false;
	}

	private void Awake()
	{
		myInteractionHook = GetComponent<InteractionHook>();
		myInteractionHook.LeftClickAction += Coffee;
		AHO = GameObject.Find("CoffeeMaker").AddComponent<AudioHubObject>();
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= Coffee;
		CoffeeIns = null;
		AHO = null;
	}

	private void Update()
	{
		if (scary && ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position.x >= 15f && ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position.x <= 20f && ControllerManager.Get<roamController>(GAME_CONTROLLER.ROAM).transform.position.y <= 15f)
		{
			scary = false;
			if (AHO != null)
			{
				AHO.PlaySound(CustomSoundLookUp.coffeetime);
			}
		}
	}

	private void Coffee()
	{
		if (isCoffee)
		{
			GameManager.AudioSlinger.PlaySound(LookUp.SoundLookUp.ItemPickUp2);
			HaveCoffee = true;
			myInteractionHook.ForceLock = true;
			GameObject.Find("CoffeePacks").SetActive(value: false);
			GameManager.TimeSlinger.FireTimer(0.5f, delegate
			{
				CoffeeIns.ForceLock = false;
			});
		}
		else if (HaveCoffee)
		{
			if (AHO != null)
			{
				AHO.PlaySound(CustomSoundLookUp.coffeetime);
			}
			DOSCoinsCurrencyManager.AddCurrency((Random.Range(0, 100) <= 1) ? 5f : 0.1f);
			myInteractionHook.ForceLock = true;
			GameManager.TimeSlinger.FireTimer(CustomSoundLookUp.coffeetime.AudioClip.length + 0.65f, delegate
			{
				myInteractionHook.ForceLock = false;
			});
		}
	}
}
