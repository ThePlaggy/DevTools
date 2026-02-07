using UnityEngine;

public static class DOSCoinsCurrencyManager
{
	public static CustomEvent<float> CurrencyWasAdded = new CustomEvent<float>(2);

	public static CustomEvent<float> CurrencyWasRemoved = new CustomEvent<float>(2);

	private static TextRollerObject _currencyTextRoller;

	private static float _currentCurrenyAMT = 100f;

	private static float _pendingCurrencyAMT;

	private static float _pendingCurrencyTimeStamp;

	public static float CurrentCurrency => _currentCurrenyAMT;

	public static void SetCurrency(float SetAMT)
	{
		if (!DifficultyManager.HackerMode)
		{
			_currentCurrenyAMT = SetAMT;
			updateCurrencyText(_currentCurrenyAMT);
		}
	}

	public static void AddCurrency(float SetAMT)
	{
		if (!DifficultyManager.HackerMode)
		{
			updateCurrencyText(_currentCurrenyAMT + SetAMT);
			_currentCurrenyAMT += SetAMT;
			CurrencyWasAdded.Execute(SetAMT);
		}
	}

	public static void AddPendingCurrency(float SetAMT)
	{
		if (!DifficultyManager.HackerMode)
		{
			_pendingCurrencyAMT += SetAMT;
		}
	}

	public static void RemoveCurrency(float SetAMT, bool DosDrainerFast = false)
	{
		if (DifficultyManager.HackerMode)
		{
			return;
		}
		if (SetAMT <= 0f)
		{
			Debug.LogFormat("[CurrencyManager] Tried to remove negative currency of {0}", SetAMT.ToString());
			return;
		}
		if (_currentCurrenyAMT <= 0f)
		{
			Debug.LogFormat("[CurrencyManager] Cannot remove {0} because the current currency is below zero.", SetAMT.ToString());
			return;
		}
		float num = _currentCurrenyAMT - SetAMT;
		if (num <= 0f)
		{
			num = 0f;
		}
		updateCurrencyText(num, DosDrainerFast);
		_currentCurrenyAMT = num;
		CurrencyWasRemoved.Execute(SetAMT);
	}

	public static void RemoveCurrencyBypassNegative(float SetAMT)
	{
		float num = _currentCurrenyAMT - SetAMT;
		updateCurrencyText(num);
		_currentCurrenyAMT = num;
		CurrencyWasRemoved.Execute(SetAMT);
	}

	public static bool PurchaseItem(ZeroDayProductDefinition ItemToPurchase)
	{
		if (DifficultyManager.HackerMode)
		{
			return false;
		}
		if (_currentCurrenyAMT >= ItemToPurchase.productPrice)
		{
			RemoveCurrency(ItemToPurchase.productPrice);
			TannerDOSPopper.PopDOS((int)ItemToPurchase.productPrice);
			return true;
		}
		return false;
	}

	public static bool PurchaseItem(ShadowMarketProductDefinition ItemToPurchase)
	{
		if (DifficultyManager.HackerMode)
		{
			return false;
		}
		if (_currentCurrenyAMT >= ItemToPurchase.productPrice)
		{
			RemoveCurrency(ItemToPurchase.productPrice);
			TannerDOSPopper.PopDOS((int)ItemToPurchase.productPrice);
			return true;
		}
		return false;
	}

	public static void SetCurrencyTextRoller(TextRollerObject SetTextRollerObject)
	{
		if (!DifficultyManager.HackerMode)
		{
			_currencyTextRoller = SetTextRollerObject;
		}
	}

	public static void Tick()
	{
		if (!DifficultyManager.HackerMode && Time.time - _pendingCurrencyTimeStamp >= 5f)
		{
			_pendingCurrencyTimeStamp = Time.time;
			processPendingCurrency();
		}
	}

	private static void updateCurrencyText(float ToValue, bool ddf = false)
	{
		if (!DifficultyManager.HackerMode)
		{
			_currencyTextRoller.ProcessLinerRequest(_currentCurrenyAMT, ToValue, ddf ? CustomSoundLookUp.newDOSDrainer.AudioClip.length : 1f);
		}
	}

	private static void processPendingCurrency()
	{
		if (!DifficultyManager.HackerMode && _pendingCurrencyAMT > 0f)
		{
			CurrencyWasAdded.Execute(_pendingCurrencyAMT);
			updateCurrencyText(_currentCurrenyAMT + _pendingCurrencyAMT);
			_currentCurrenyAMT += _pendingCurrencyAMT;
			_pendingCurrencyAMT = 0f;
		}
	}
}
