public static class UIDialogManager
{
	private static NetworkDialog _networkDialog;

	private static VWipeDialog _vWipeDialog;

	public static NetworkDialog NetworkDialog
	{
		get
		{
			return _networkDialog;
		}
		set
		{
			_networkDialog = value;
		}
	}

	public static VWipeDialog VWipeDialog
	{
		get
		{
			return _vWipeDialog;
		}
		set
		{
			_vWipeDialog = value;
		}
	}
}
