using System;
using UnityEngine;

public class KidnapperTitleManager : MonoBehaviour
{
	[SerializeField]
	private Animator myAnimator;

	[NonSerialized]
	public bool isEnding;

	private void Awake()
	{
		base.transform.position = new Vector3(-0.02f, 0.41f, 0.9f);
		base.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		base.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
	}

	private void Start()
	{
		if (isEnding)
		{
			base.transform.position = new Vector3(-2.1205f, 0.225f, 9.5472f);
			base.transform.rotation = Quaternion.Euler(0f, 140f, 0f);
			base.transform.localScale = Vector3.one;
		}
		else
		{
			TimeSlinger.FireTimer(TriggerAnim, 9f);
		}
	}

	private void TriggerAnim()
	{
		Stage(UnityEngine.Random.Range(0, 2) + 1);
		TimeSlinger.FireTimer(TriggerAnim, UnityEngine.Random.Range(5f, 12f));
	}

	private void Stage(int stage)
	{
		myAnimator.SetInteger("stageID", stage);
		Invoke("Reset", 1f);
	}

	private void Reset()
	{
		myAnimator.SetInteger("stageID", 0);
	}
}
