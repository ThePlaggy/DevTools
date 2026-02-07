using UnityEngine;

public class VirusManager : MonoBehaviour
{
	[SerializeField]
	private float virusTick = 30f;

	[SerializeField]
	private float dosCoinDeductPerVirus = 0.5f;

	[SerializeField]
	private int chanceOfGettingVirus = 75;

	[SerializeField]
	private int chanceOfLosingDOSCoin = 55;

	[SerializeField]
	private int chanceOfShutDown = 25;

	private bool hasVirus;

	private float virusTickTimeStamp;

	private VirusManagerData myData;

	public int getVirusCount { get; private set; }

	private void Awake()
	{
		GameManager.StageManager.Stage += stageMe;
	}

	private void stageMe()
	{
		GameManager.StageManager.Stage -= stageMe;
		myData = DataManager.Load<VirusManagerData>(608806);
		if (myData == null)
		{
			myData = new VirusManagerData(608806);
			myData.HasVirus = false;
			myData.VirusCount = 0;
		}
		hasVirus = myData.HasVirus;
		getVirusCount = myData.VirusCount;
		DataManager.Save(myData);
	}

	private void Update()
	{
		if (ComputerPowerHook.Ins.PowerOn)
		{
			if (hasVirus && Time.time - virusTickTimeStamp >= virusTick)
			{
				virusTickTimeStamp = Time.time;
				triggerVirusEffects();
			}
		}
		else if (hasVirus)
		{
			virusTickTimeStamp = Time.time;
		}
	}

	public void AddVirus()
	{
		if (Random.Range(0, 100) <= chanceOfGettingVirus)
		{
			if (getVirusCount <= 0)
			{
				virusTickTimeStamp = Time.time;
				hasVirus = true;
			}
			getVirusCount++;
			myData.HasVirus = true;
			myData.VirusCount = getVirusCount;
		}
	}

	public void ForceVirus()
	{
		if (DifficultyManager.CasualMode)
		{
			AddVirus();
			return;
		}
		if (getVirusCount <= 0)
		{
			virusTickTimeStamp = Time.time;
			hasVirus = true;
		}
		getVirusCount++;
		myData.HasVirus = true;
		myData.VirusCount = getVirusCount;
	}

	public void ClearVirus()
	{
		hasVirus = false;
		getVirusCount = 0;
		myData.HasVirus = false;
		myData.VirusCount = 0;
		DataManager.Save(myData);
	}

	private void triggerVirusEffects()
	{
		if (Random.Range(0, 100) <= chanceOfLosingDOSCoin)
		{
			DOSCoinsCurrencyManager.RemoveCurrency((float)getVirusCount * dosCoinDeductPerVirus);
		}
		if (Random.Range(0, 100) <= chanceOfShutDown && !StateManager.BeingHacked)
		{
			ComputerPowerHook.Ins.ShutDownComputer();
		}
	}
}
