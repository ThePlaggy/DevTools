using UnityEngine;

public static class TarotRefiller
{
	public static void RefillCards()
	{
		if (TarotCardsBehaviour.CantRefillNow)
		{
			GameManager.TimeSlinger.FireTimer(5f, RefillCards);
			Debug.Log("[TarotRefiller] Can't refill now");
		}
		else if (TarotCardsBehaviour.Owned)
		{
			if (TarotCardsBehaviour.CardsOnTable)
			{
				GameManager.TimeSlinger.FireTimer(3f, RefillCards);
				return;
			}
			TarotCardsBehaviour.CardsOnTable = true;
			Object.Destroy(TarotCardsBehaviour.Ins.gameObject);
			Object.Instantiate(CustomObjectLookUp.TarotCards).GetComponent<TarotCardsBehaviour>().SoftBuild();
			TarotCardsBehaviour.Ins.MoveMe(new Vector3(1.393f, 40.68f, 2.489f), new Vector3(0f, -20f, 180f), new Vector3(0.3f, 0.3f, 0.3f));
			Debug.Log("[TarotRefiller] Refilled!");
		}
	}
}
