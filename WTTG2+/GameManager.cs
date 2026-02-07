using System;

public class GameManager
{
	private static GameManager _instance;

	private static audioSlinger _audioSlinger;

	private static timeSlinger _timeSlinger;

	private static tweenSlinger _tweenSlinger;

	private static PauseManager _pauseManager;

	private static BehaviourManager _behaviourManager;

	private static ManagerSlinger _managerSlinger;

	private static WorldManager _worldManager;

	private static StageManager _stageManager;

	private static InteractionManager _interactionManager;

	private static TheCloud _theCloud;

	private static TimeKeeper _timeKeeper;

	private static HackerManager _hackerManager;

	private static MicManager _micManager;

	private bool inited;

	public static GameManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameManager();
			}
			return _instance;
		}
	}

	public static audioSlinger AudioSlinger
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

	public static timeSlinger TimeSlinger
	{
		get
		{
			if (_timeSlinger == null)
			{
				_timeSlinger = new timeSlinger();
			}
			return _timeSlinger;
		}
	}

	public static tweenSlinger TweenSlinger
	{
		get
		{
			if (_tweenSlinger == null)
			{
				_tweenSlinger = new tweenSlinger();
			}
			return _tweenSlinger;
		}
	}

	public static PauseManager PauseManager
	{
		get
		{
			if (_pauseManager == null)
			{
				_pauseManager = new PauseManager();
			}
			return _pauseManager;
		}
	}

	public static BehaviourManager BehaviourManager
	{
		get
		{
			if (_behaviourManager == null)
			{
				_behaviourManager = new BehaviourManager();
			}
			return _behaviourManager;
		}
	}

	public static ManagerSlinger ManagerSlinger
	{
		get
		{
			if (_managerSlinger == null)
			{
				_managerSlinger = new ManagerSlinger();
			}
			return _managerSlinger;
		}
	}

	public static WorldManager WorldManager
	{
		get
		{
			return _worldManager;
		}
		set
		{
			_worldManager = value;
		}
	}

	public static StageManager StageManager
	{
		get
		{
			return _stageManager;
		}
		set
		{
			_stageManager = value;
		}
	}

	public static InteractionManager InteractionManager
	{
		get
		{
			return _interactionManager;
		}
		set
		{
			_interactionManager = value;
		}
	}

	public static TheCloud TheCloud
	{
		get
		{
			return _theCloud;
		}
		set
		{
			_theCloud = value;
		}
	}

	public static TimeKeeper TimeKeeper
	{
		get
		{
			return _timeKeeper;
		}
		set
		{
			_timeKeeper = value;
		}
	}

	public static HackerManager HackerManager
	{
		get
		{
			return _hackerManager;
		}
		set
		{
			_hackerManager = value;
		}
	}

	public GameManager()
	{
		inited = false;
	}

	public static void Kill()
	{
		_instance = null;
		_audioSlinger = null;
		_timeSlinger = null;
		_tweenSlinger = null;
		_pauseManager = null;
		_behaviourManager = null;
		_managerSlinger = null;
		_worldManager = null;
		_stageManager = null;
		_interactionManager = null;
		_theCloud = null;
		_timeKeeper = null;
		_hackerManager = null;
		_micManager = null;
		GC.Collect();
	}

	public void Init()
	{
		inited = true;
	}

	public bool isInited()
	{
		return inited;
	}

	public void Update()
	{
		TweenSlinger.Update();
		PauseManager.Update();
		TimeSlinger.Update();
		DOSCoinsCurrencyManager.Tick();
	}
}
