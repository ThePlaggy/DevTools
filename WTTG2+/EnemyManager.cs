using System;

public static class EnemyManager
{
	private static PoliceManager _policeManager;

	private static HitManManager _hitManManager;

	private static CultManager _cultManager;

	private static DollMakerManager _dollMakerManager;

	private static BreatherManager _breatherManager;

	private static BombMakerManager _bombMakerManager;

	private static KidnapperManager _kidnapperManager;

	public static PoliceManager PoliceManager
	{
		get
		{
			return _policeManager;
		}
		set
		{
			_policeManager = value;
		}
	}

	public static HitManManager HitManManager
	{
		get
		{
			return _hitManManager;
		}
		set
		{
			_hitManManager = value;
		}
	}

	public static CultManager CultManager
	{
		get
		{
			return _cultManager;
		}
		set
		{
			_cultManager = value;
		}
	}

	public static DollMakerManager DollMakerManager
	{
		get
		{
			return _dollMakerManager;
		}
		set
		{
			_dollMakerManager = value;
		}
	}

	public static BreatherManager BreatherManager
	{
		get
		{
			return _breatherManager;
		}
		set
		{
			_breatherManager = value;
		}
	}

	public static BombMakerManager BombMakerManager
	{
		get
		{
			return _bombMakerManager;
		}
		set
		{
			_bombMakerManager = value;
		}
	}

	public static KidnapperManager KidnapperManager
	{
		get
		{
			return _kidnapperManager;
		}
		set
		{
			_kidnapperManager = value;
		}
	}

	public static void Clear()
	{
		EnemyStateManager.Reset();
		_policeManager = null;
		_hitManManager = null;
		_cultManager = null;
		_dollMakerManager = null;
		_breatherManager = null;
		_bombMakerManager = null;
		_kidnapperManager = null;
		GC.Collect();
	}
}
