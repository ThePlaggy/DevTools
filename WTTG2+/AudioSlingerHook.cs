public static class AudioSlingerHook
{
	private static audioSlinger _audioSlinger;

	public static audioSlinger Ins
	{
		get
		{
			if (_audioSlinger == null)
			{
				_audioSlinger = new audioSlinger();
			}
			return _audioSlinger;
		}
	}
}
