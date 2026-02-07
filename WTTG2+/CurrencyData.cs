using System;

[Serializable]
public class CurrencyData : DataObject
{
	public float CurrentCurrency { get; set; }

	public CurrencyData(int SetID)
		: base(SetID)
	{
	}
}
