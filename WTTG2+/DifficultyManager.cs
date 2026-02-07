public static class DifficultyManager
{
	private static bool _casualMode;

	private static bool _leetMode;

	private static bool _nightmare;

	private static bool _hackerMode;

	public static bool CasualMode
	{
		get
		{
			return _casualMode;
		}
		set
		{
			_casualMode = value;
		}
	}

	public static bool LeetMode
	{
		get
		{
			return _leetMode;
		}
		set
		{
			_leetMode = value;
		}
	}

	public static bool Nightmare
	{
		get
		{
			return _nightmare;
		}
		set
		{
			_nightmare = value;
		}
	}

	public static bool HackerMode
	{
		get
		{
			return _hackerMode;
		}
		set
		{
			_hackerMode = value;
		}
	}
}
