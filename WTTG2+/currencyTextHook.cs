using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class currencyTextHook : MonoBehaviour
{
	public static Text TextIns;

	private CurrencyData myData;

	private int myID;

	private Text myText;

	private TextRollerObject myTextRoller;

	private void Awake()
	{
		myID = 1738;
		myText = GetComponent<Text>();
		myTextRoller = base.gameObject.AddComponent<TextRollerObject>();
		DOSCoinsCurrencyManager.CurrencyWasAdded.Event += currencyWasAdded;
		DOSCoinsCurrencyManager.CurrencyWasRemoved.Event += currencyWasRemoved;
		DOSCoinsCurrencyManager.SetCurrencyTextRoller(myTextRoller);
		GameManager.StageManager.Stage += stageMe;
	}

	private void OnDestroy()
	{
		DOSCoinsCurrencyManager.CurrencyWasAdded.Event -= currencyWasAdded;
		DOSCoinsCurrencyManager.CurrencyWasRemoved.Event -= currencyWasRemoved;
	}

	private void currencyWasAdded(float AddAMT)
	{
		if (myData != null)
		{
			myData.CurrentCurrency += AddAMT;
			DataManager.Save(myData);
		}
	}

	private void currencyWasRemoved(float RemoveAMT)
	{
		if (myData != null)
		{
			myData.CurrentCurrency -= RemoveAMT;
			if (myData.CurrentCurrency <= 0f)
			{
				myData.CurrentCurrency = 0f;
			}
			DataManager.Save(myData);
		}
	}

	private void stageMe()
	{
		myData = DataManager.Load<CurrencyData>(myID);
		if (myData == null)
		{
			myData = new CurrencyData(myID);
			if (DifficultyManager.LeetMode)
			{
				myData.CurrentCurrency = 1f;
			}
			else
			{
				myData.CurrentCurrency = 10f;
			}
		}
		DOSCoinsCurrencyManager.SetCurrency(DifficultyManager.CasualMode ? 100f : (DifficultyManager.Nightmare ? 0f : myData.CurrentCurrency));
		myText.text = DOSCoinsCurrencyManager.CurrentCurrency.ToString();
		GameManager.StageManager.Stage -= stageMe;
	}
}
