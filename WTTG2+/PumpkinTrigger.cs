using UnityEngine;

[RequireComponent(typeof(InteractionHook))]
public class PumpkinTrigger : MonoBehaviour
{
	[HideInInspector]
	public InteractionHook myInteractionHook;

	[HideInInspector]
	public HalloweenEvent HE;

	[HideInInspector]
	public bool TriggeredMe;

	[HideInInspector]
	public int myPumpkinID;

	private void Awake()
	{
		TriggeredMe = false;
		base.gameObject.layer = 9;
		myInteractionHook = base.gameObject.GetComponent<InteractionHook>();
		myInteractionHook.StateActive = PLAYER_STATE.ROAMING;
		myInteractionHook.LeftClickAction += boo;
		GetComponent<BoxCollider>().size = new Vector3(3f, 3f, 3f);
	}

	public void PutHE(HalloweenEvent HE, int dynamicId = 0, bool PCPumpkin = false)
	{
		this.HE = HE;
		this.HE.pumpkinLights.Add(base.gameObject.transform.Find("PumpkinLight").GetComponent<Light>());
		myPumpkinID = dynamicId;
		if (PCPumpkin)
		{
			GetComponent<BoxCollider>().size = new Vector3(5f, 5f, 5f);
			myInteractionHook.StateActive = PLAYER_STATE.DESK;
		}
	}

	private void OnDestroy()
	{
		myInteractionHook.LeftClickAction -= boo;
	}

	private void boo()
	{
		if (!TriggeredMe)
		{
			HE.OnPumpkinClicked(this);
		}
	}
}
