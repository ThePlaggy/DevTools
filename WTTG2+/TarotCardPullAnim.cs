using DG.Tweening;
using UnityEngine;

public class TarotCardPullAnim : MonoBehaviour
{
	public static int currentCard;

	public static int currentCardTex = -1;

	public static float foolTimer = 1f;

	public static TarotCardPullAnim Ins;

	public static bool ManipulatedCard;

	public static int ManipulatedID;

	public static bool WheelOfFortuneActive;

	public static int MAX_TAROT_CARDS;

	public static bool ManipulatedFortune;

	public static bool[] AlreadyGotTarot;

	public GameObject[] cards;

	public Texture2D[] cardsTex;

	public Texture2D theFool;

	public Material GlowMat;

	[HideInInspector]
	public AudioFileDefinition TheFoolSFX;

	[HideInInspector]
	public AudioFileDefinition PullSFX;

	[HideInInspector]
	public AudioFileDefinition DisappearSFX;

	public Texture2D wheelOfFortune;

	public Texture2D theLiar;

	private readonly int[] CommonCards = new int[7] { 0, 1, 2, 3, 4, 5, 6 };

	private readonly int[] UncommonCards = new int[8] { 7, 8, 9, 10, 11, 12, 13, 23 };

	private readonly int[] RareCards = new int[9] { 14, 15, 16, 17, 18, 19, 22, 24, 29 };

	private readonly int[] VeryRareCards = new int[6] { 20, 21, 25, 26, 27, 28 };

	private void Awake()
	{
		Ins = this;
		GlowMat = Object.Instantiate(GlowMat);
		DisappearSFX = CustomSoundLookUp.disappear;
		TheFoolSFX = CustomSoundLookUp.fool;
		PullSFX = CustomSoundLookUp.pull;
		currentCard = 0;
		currentCardTex = -1;
		foolTimer = 1f;
	}

	private void TheFool()
	{
		switch (Random.Range(0, 3))
		{
		case 0:
			TheFoolSFX = CustomSoundLookUp.fool;
			break;
		case 1:
			TheFoolSFX = CustomSoundLookUp.fool2;
			break;
		case 2:
			TheFoolSFX = CustomSoundLookUp.fool3;
			break;
		}
		TarotCardsBehaviour.Ins.myAudioHub.PlaySoundWithWildPitch(TheFoolSFX, 0.5f, 1.5f);
		cards[currentCard].GetComponent<Renderer>().material = GlowMat;
		ChangeTexBF();
		for (float num = 0f; num < 0.3f; num += 0.01f)
		{
			GameManager.TimeSlinger.FireTimer(num, GlowIn);
		}
		GameManager.TimeSlinger.FireTimer(0.16f, ChangeTexF);
		for (float num2 = 0.3f; num2 < 0.62f; num2 += 0.01f)
		{
			GameManager.TimeSlinger.FireTimer(num2, GlowOut);
		}
		GameManager.TimeSlinger.FireTimer(0.48f, ChangeTexAF);
		foolTimer = 1f;
	}

	private void GlowIn()
	{
		cards[currentCard].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0f, 0.4f, 0f, 1f) * foolTimer / 10f);
		foolTimer += 1f;
	}

	private void GlowOut()
	{
		cards[currentCard].GetComponent<Renderer>().material.SetColor("_EmissionColor", new Color(0f, 0.4f, 0f, 1f) * foolTimer / 10f);
		foolTimer -= 1f;
	}

	public void DoPull()
	{
		WheelOfFortuneActive = Random.Range(0, 100) <= 7 || ManipulatedFortune;
		TarotCardsBehaviour.Ins.myAudioHub.PlaySound(PullSFX);
		GameManager.TimeSlinger.FireTimer(0.16f, ChangeTex);
		int num = Random.Range(0, 103);
		if (num < 50)
		{
			currentCardTex = CommonCards[Random.Range(0, CommonCards.Length)];
		}
		else if (num < 85)
		{
			currentCardTex = UncommonCards[Random.Range(0, UncommonCards.Length)];
		}
		else if (num < 100)
		{
			currentCardTex = RareCards[Random.Range(0, RareCards.Length)];
		}
		else
		{
			currentCardTex = VeryRareCards[Random.Range(0, VeryRareCards.Length)];
			if (AlreadyGotTarot[currentCardTex])
			{
				currentCardTex = RareCards[Random.Range(0, RareCards.Length)];
			}
		}
		if (ManipulatedCard)
		{
			currentCardTex = ManipulatedID;
		}
		if (currentCardTex > MAX_TAROT_CARDS - 1)
		{
			Debug.Log("Tarot Cards - Something went wrong, MAX_TAROT_CARDS went out of range: --> " + currentCardTex);
			currentCardTex = Random.Range(0, MAX_TAROT_CARDS);
		}
		cards[currentCard].transform.DOLocalMoveX(-0.6f, 0.4f);
		cards[currentCard].transform.DOLocalMoveZ(-0.3f, 0.4f);
		cards[currentCard].transform.DOLocalMoveY(-0.4f, 0.2f).OnComplete(delegate
		{
			cards[currentCard].transform.DOLocalMoveY(0.4f, 0.4f);
			cards[currentCard].transform.DOLocalMoveX(-0.8f, 0.4f);
		});
		cards[currentCard].transform.DOLocalRotate(new Vector3(0f, -20f, -180f), 0.4f);
		if (Random.Range(0, 100) < 20 && !ManipulatedCard)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 2 && TarotManager.TimeController == 60)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 3 && TarotManager.TimeController == 5)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 8 && (StateManager.BeingHacked || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState()))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 14 && EnemyStateManager.IsInEnemyStateOrLocked())
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 19 && (PowerStateManager.IsLocked() || StateManager.BeingHacked || KAttack.IsInAttack || EnvironmentManager.PowerState == POWER_STATE.OFF || TarotManager.HermitActive))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 16 && EnvironmentManager.PowerState == POWER_STATE.OFF)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 10 && DifficultyManager.CasualMode)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 23 && DifficultyManager.CasualMode)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 17 && MainCameraHook.Ins.GetMyARFEnabled)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 11 && TarotManager.CurSpeed == playerSpeedMode.QUICK)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 12 && TarotManager.CurSpeed == playerSpeedMode.WEAK)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 15 && (DifficultyManager.LeetMode || DifficultyManager.Nightmare || DifficultyManager.CasualMode))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 7 && DOSCoinsCurrencyManager.CurrentCurrency <= 0f)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 22 && (DifficultyManager.CasualMode || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyStateOrLocked() || StateManager.BeingHacked))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 27 && DifficultyManager.CasualMode)
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 26 && EnemyStateManager.IsInEnemyStateOrLocked())
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 25 && (StateManager.BeingHacked || GameManager.HackerManager.theSwan.SwanFailure || !ComputerPowerHook.Ins.FullyPoweredOn || EnvironmentManager.PowerState == POWER_STATE.OFF || EnemyStateManager.IsInEnemyState()))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else if (currentCardTex == 28 && (DifficultyManager.CasualMode || EnemyStateManager.IsInEnemyState() || TarotVengeance._tarotVengeanceActivePool.Count <= 0))
		{
			GameManager.TimeSlinger.FireTimer(1f, TheFool);
		}
		else
		{
			AlreadyGotTarot[currentCardTex] = true;
			GameManager.TimeSlinger.FireTimer(1.25f, TarotManager.Ins.PullCardAtLoc);
			if (WheelOfFortuneActive)
			{
				GameManager.TimeSlinger.FireTimer(1.05f, StartWheelin, currentCard);
			}
		}
		GameManager.TimeSlinger.FireTimer(1.75f, Popoff);
		ManipulatedCard = false;
		ManipulatedID = 0;
	}

	private void ChangeTex()
	{
		if (WheelOfFortuneActive)
		{
			cards[currentCard].GetComponent<Renderer>().material.SetTexture("_MainTex", wheelOfFortune);
			cards[currentCard].GetComponent<Renderer>().material.SetTexture("_EmissionMap", wheelOfFortune);
		}
		else
		{
			cards[currentCard].GetComponent<Renderer>().material.SetTexture("_MainTex", cardsTex[currentCardTex]);
			cards[currentCard].GetComponent<Renderer>().material.SetTexture("_EmissionMap", cardsTex[currentCardTex]);
		}
	}

	private void ChangeTexBF()
	{
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_MainTex", WheelOfFortuneActive ? wheelOfFortune : cardsTex[currentCardTex]);
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_EmissionMap", null);
	}

	private void ChangeTexF()
	{
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_MainTex", theFool);
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_EmissionMap", null);
	}

	private void ChangeTexAF()
	{
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_MainTex", theFool);
		cards[currentCard].GetComponent<Renderer>().material.SetTexture("_EmissionMap", theFool);
	}

	private void Popoff()
	{
		TarotCardsBehaviour.Ins.myAudioHub.PlaySound(DisappearSFX);
		cards[currentCard].transform.DOScale(new Vector3(0f, 0f, 0f), 0.2f);
		currentCard++;
		currentCardTex = -1;
	}

	public static void DisableVerif()
	{
		if (!DifficultyManager.HackerMode)
		{
			Debug.Log("Disabling tarot card verification check: " + AlreadyGotTarot.Length);
			for (int i = 0; i < AlreadyGotTarot.Length; i++)
			{
				AlreadyGotTarot[i] = false;
			}
		}
	}

	private void StartWheelin(int card)
	{
		cards[card].transform.DORotate(new Vector3(0f, 1337f + cards[card].transform.rotation.y, 0f), 0.75f, RotateMode.FastBeyond360).SetEase(Ease.Linear);
	}
}
