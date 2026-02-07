using System;

public class ActionStorage : ActionSlinger
{
	private Action myCallBack;

	public ActionStorage(Action setCallBackAction)
	{
		myCallBack = setCallBackAction;
	}

	protected override void OnFire()
	{
		myCallBack();
	}
}
public class ActionStorage<A> : ActionSlinger
{
	private A ACallBackValue;

	private Action<A> myCallBack;

	public ActionStorage(Action<A> setCallBack, A setCallBackValue)
	{
		ACallBackValue = setCallBackValue;
		myCallBack = setCallBack;
	}

	protected override void OnFire()
	{
		myCallBack?.Invoke(ACallBackValue);
	}
}
public class ActionStorage<A, B> : ActionSlinger
{
	private A ACallBackValue;

	private B BCallBackValue;

	private Action<A, B> myCallBack;

	public ActionStorage(Action<A, B> setCallBack, A setACallBackValue, B setBCallBackValue)
	{
		ACallBackValue = setACallBackValue;
		BCallBackValue = setBCallBackValue;
		myCallBack = setCallBack;
	}

	protected override void OnFire()
	{
		myCallBack?.Invoke(ACallBackValue, BCallBackValue);
	}
}
public class ActionStorage<A, B, C> : ActionSlinger
{
	private A ACallBackValue;

	private B BCallBackValue;

	private C CCallBackValue;

	private Action<A, B, C> myCallBack;

	public ActionStorage(Action<A, B, C> setCallBack, A setACallBackValue, B setBCallBackValue, C setCCallBackValue)
	{
		ACallBackValue = setACallBackValue;
		BCallBackValue = setBCallBackValue;
		CCallBackValue = setCCallBackValue;
		myCallBack = setCallBack;
	}

	protected override void OnFire()
	{
		myCallBack?.Invoke(ACallBackValue, BCallBackValue, CCallBackValue);
	}
}
public class ActionStorage<A, B, C, D> : ActionSlinger
{
	private A ACallBackValue;

	private B BCallBackValue;

	private C CCallBackValue;

	private D DCallBackValue;

	private Action<A, B, C, D> myCallBack;

	public ActionStorage(Action<A, B, C, D> setCallBack, A setACallBackValue, B setBCallBackValue, C setCCallBackValue, D setDCallBackValue)
	{
		ACallBackValue = setACallBackValue;
		BCallBackValue = setBCallBackValue;
		CCallBackValue = setCCallBackValue;
		DCallBackValue = setDCallBackValue;
		myCallBack = setCallBack;
	}

	protected override void OnFire()
	{
		myCallBack?.Invoke(ACallBackValue, BCallBackValue, CCallBackValue, DCallBackValue);
	}
}
