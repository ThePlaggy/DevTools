public struct CustomStackPusher
{
	public int MatrixSize;

	public int StackPeices;

	public int DeadPeices;

	public float TimePerPeice;

	public int WarmUpTime;

	public bool RandomExit;

	public CustomStackPusher(int MatrixSize, int StackPeices, int DeadPeices, float TimePerPeice, int WarmUpTime, bool RandomExit)
	{
		this.MatrixSize = MatrixSize;
		this.StackPeices = StackPeices;
		this.DeadPeices = DeadPeices;
		this.TimePerPeice = TimePerPeice;
		this.WarmUpTime = WarmUpTime;
		this.RandomExit = RandomExit;
	}
}
