using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(BoxCollider))]
public class InteractionHook : MonoBehaviour
{
	public delegate void ReceiveActions();

	public delegate void ReciveActionsFloat(float SetValue);

	public PLAYER_STATE StateActive;

	public bool AllStates;

	public bool RequireLocationCheck;

	public PLAYER_LOCATION LocationToCheck;

	public bool RequireLocationCheckForRightClick;

	public PLAYER_LOCATION LocationCheckForRightClick;

	public bool AllowMultiStates;

	public PLAYER_STATE[] MultiStatesActive = new PLAYER_STATE[0];

	public bool UseAxis;

	private bool activeCrossHairShown;

	private bool forceLock;

	private bool isReceving;

	private Action rescindCache;

	public bool ForceLock
	{
		get
		{
			return forceLock;
		}
		set
		{
			forceLock = value;
			MyBoxCollider.enabled = !forceLock;
		}
	}

	public BoxCollider MyBoxCollider { get; private set; }

	public event ReceiveActions RecvAction;

	public event ReciveActionsFloat LeftAxisAction;

	public event ReceiveActions LeftClickAction;

	public event ReceiveActions RightClickAction;

	public event ReceiveActions RecindAction;

	private void Awake()
	{
		MyBoxCollider = GetComponent<BoxCollider>();
		rescindCache = rescind;
	}

	private void Update()
	{
		bool flag = true;
		bool flag2 = true;
		if (!forceLock)
		{
			if (AllowMultiStates)
			{
				bool flag3 = false;
				for (int i = 0; i < MultiStatesActive.Length; i++)
				{
					if (StateManager.PlayerState == MultiStatesActive[i])
					{
						flag3 = true;
						i = MultiStatesActive.Length;
					}
				}
				MyBoxCollider.enabled = flag3;
			}
			else
			{
				MyBoxCollider.enabled = StateManager.PlayerState == StateActive;
			}
			if (AllStates)
			{
				MyBoxCollider.enabled = true;
			}
			if (isReceving)
			{
				if (RequireLocationCheck && StateManager.PlayerLocation != LocationToCheck)
				{
					flag2 = false;
				}
				if (flag2)
				{
					if (AllowMultiStates)
					{
						flag2 = false;
						for (int j = 0; j < MultiStatesActive.Length; j++)
						{
							if (StateManager.PlayerState == MultiStatesActive[j])
							{
								flag2 = true;
								j = MultiStatesActive.Length;
							}
						}
					}
					else if (StateManager.PlayerState == StateActive)
					{
						flag = false;
					}
					else
					{
						flag2 = false;
					}
				}
				if (AllStates)
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (!activeCrossHairShown)
					{
						activeCrossHairShown = true;
						GameManager.BehaviourManager.CrossHairBehaviour.ShowActiveCrossHair();
					}
					if (UseAxis)
					{
						if (this.LeftAxisAction != null)
						{
							this.LeftAxisAction(CrossPlatformInputManager.GetAxis("LeftClickWeighted"));
						}
					}
					else if (CrossPlatformInputManager.GetButtonDown("LeftClick"))
					{
						if (this.LeftClickAction != null)
						{
							this.LeftClickAction();
						}
					}
					else if (CrossPlatformInputManager.GetButtonDown("RightClick"))
					{
						if (RequireLocationCheckForRightClick && StateManager.PlayerLocation != LocationCheckForRightClick)
						{
							flag2 = false;
						}
						if (flag2 && this.RightClickAction != null)
						{
							this.RightClickAction();
						}
					}
				}
			}
		}
		if (flag && activeCrossHairShown)
		{
			activeCrossHairShown = false;
			GameManager.BehaviourManager.CrossHairBehaviour.HideActiveCrossHair();
		}
	}

	public void Receive()
	{
		if (isReceving)
		{
			return;
		}
		if (AllowMultiStates)
		{
			for (int i = 0; i < MultiStatesActive.Length; i++)
			{
				if (StateManager.PlayerState == MultiStatesActive[i])
				{
					isReceving = true;
					i = MultiStatesActive.Length;
				}
			}
		}
		else if (StateManager.PlayerState == StateActive)
		{
			isReceving = true;
		}
		if (isReceving && RequireLocationCheck)
		{
			if (StateManager.PlayerLocation == LocationToCheck)
			{
				isReceving = true;
			}
			else
			{
				isReceving = false;
			}
		}
		if (isReceving)
		{
			GameManager.InteractionManager.Rescind.Event += rescindCache;
			if (this.RecvAction != null)
			{
				this.RecvAction();
			}
		}
	}

	private void rescind()
	{
		isReceving = false;
		GameManager.InteractionManager.Rescind.Event -= rescindCache;
		if (this.RecindAction != null)
		{
			this.RecindAction();
		}
		if (this.LeftAxisAction != null)
		{
			this.LeftAxisAction(0f);
		}
	}
}
