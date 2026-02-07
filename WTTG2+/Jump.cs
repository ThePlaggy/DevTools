public abstract class Jump
{
	protected abstract void DoStage();

	protected abstract void DoExecute();

	public void Stage()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		DoStage();
	}

	public void Execute()
	{
		DataManager.LockSave = true;
		DataManager.ClearGameData();
		DoExecute();
	}
}
