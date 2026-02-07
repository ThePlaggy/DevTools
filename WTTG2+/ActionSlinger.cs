public abstract class ActionSlinger
{
	public void Fire()
	{
		OnFire();
	}

	protected abstract void OnFire();
}
