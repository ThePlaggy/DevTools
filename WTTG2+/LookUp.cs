public static class LookUp
{
	private static DesktopUI _desktopUI;

	private static PlayerUI _playerUI;

	private static SoftwareProducts _softwareProducts;

	private static SoundLookUp _soundLookUp;

	private static Doors _doors;

	public static DesktopUI DesktopUI
	{
		get
		{
			return _desktopUI;
		}
		set
		{
			_desktopUI = value;
		}
	}

	public static PlayerUI PlayerUI
	{
		get
		{
			return _playerUI;
		}
		set
		{
			_playerUI = value;
		}
	}

	public static SoftwareProducts SoftwareProducts
	{
		get
		{
			return _softwareProducts;
		}
		set
		{
			_softwareProducts = value;
		}
	}

	public static SoundLookUp SoundLookUp
	{
		get
		{
			return _soundLookUp;
		}
		set
		{
			_soundLookUp = value;
		}
	}

	public static Doors Doors
	{
		get
		{
			return _doors;
		}
		set
		{
			_doors = value;
		}
	}
}
