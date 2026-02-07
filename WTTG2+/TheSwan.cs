using System;
using UnityEngine;

public class TheSwan
{
	public static skyBreakBehavior mySkyBreak;

	public bool isActivatedBefore;

	public bool SwanError => !(TheSwanBehaviour.Ins == null) && (TheSwanBehaviour.Ins.systemFailure || TheSwanBehaviour.Ins.loading);

	public bool SwanFailure => !(TheSwanBehaviour.Ins == null) && TheSwanBehaviour.Ins.systemFailure;

	public void ActivateTheSwan()
	{
		if (!ComputerPowerHook.Ins.FullyPoweredOn)
		{
			Debug.Log("[TH3SW4N] Computer not active, re-scheduling");
			GameManager.TimeSlinger.FireTimer(10f, ActivateTheSwan);
		}
		else if (!isActivatedBefore)
		{
			isActivatedBefore = true;
			string[] array = new string[3] { "swan.txt", "dharma.txt", "hatch.txt" };
			int num = UnityEngine.Random.Range(0, array.Length);
			GameManager.ManagerSlinger.TextDocManager.CreateTextDoc(array[num], "THE CODE IS LOST: 4 8 15 16 23 42");
			TheSwanBehaviour.FailSafeActivate();
		}
	}

	public void TakeSwanDOSB4()
	{
		GameManager.AudioSlinger.PlaySound(CustomSoundLookUp.systemFailure);
		GameManager.BehaviourManager.NotesBehaviour.ClearNotes();
		if (!DifficultyManager.Nightmare && !DifficultyManager.LeetMode && EnemyManager.CultManager.keyDiscoveryCount < 8)
		{
			GameManager.TheCloud.ForceKeyDiscover();
		}
		double num = DOSCoinsCurrencyManager.CurrentCurrency;
		float num2 = UnityEngine.Random.Range(0.5f, 0.9f);
		if (DifficultyManager.Nightmare)
		{
			DOSCoinsCurrencyManager.RemoveCurrency(DOSCoinsCurrencyManager.CurrentCurrency);
			return;
		}
		DOSCoinsCurrencyManager.RemoveCurrency((float)Math.Round(num * (double)num2, 3));
		WifiInteractionManager.LockTheWiFi(-1);
		BreatherManager.SwanAdded += UnityEngine.Random.Range(4, 6);
		if (BotnetBehaviour.Ins != null)
		{
			BotnetBehaviour.Ins.LoseConnection();
		}
	}
}
