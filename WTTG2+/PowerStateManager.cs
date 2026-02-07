using System.Collections.Generic;
using System.Linq;

public static class PowerStateManager
{
	private static Dictionary<STATE_LOCK_OCCASION, bool> _powerStateStackPool = new Dictionary<STATE_LOCK_OCCASION, bool>
	{
		{
			STATE_LOCK_OCCASION.POILCE,
			false
		},
		{
			STATE_LOCK_OCCASION.HITMAN,
			false
		},
		{
			STATE_LOCK_OCCASION.BREATHER,
			false
		},
		{
			STATE_LOCK_OCCASION.DOLL_MAKER,
			false
		},
		{
			STATE_LOCK_OCCASION.CULT,
			false
		},
		{
			STATE_LOCK_OCCASION.BOMB_MAKER,
			false
		},
		{
			STATE_LOCK_OCCASION.TANNER,
			false
		},
		{
			STATE_LOCK_OCCASION.KIDNAPPER,
			false
		},
		{
			STATE_LOCK_OCCASION.EXECUTIONER,
			false
		},
		{
			STATE_LOCK_OCCASION.DELFALCO,
			false
		},
		{
			STATE_LOCK_OCCASION.NEWNOIR,
			false
		},
		{
			STATE_LOCK_OCCASION.TAROT,
			false
		},
		{
			STATE_LOCK_OCCASION.OUTLAW,
			false
		},
		{
			STATE_LOCK_OCCASION.INSANITY,
			false
		},
		{
			STATE_LOCK_OCCASION.GAME_END,
			false
		},
		{
			STATE_LOCK_OCCASION.DEVTOOLS,
			false
		}
	};

	public static string PowerStateDebug => (!IsLocked()) ? EnvironmentManager.PowerState.ToString() : string.Join(" + ", _powerStateStackPool.Where(delegate(KeyValuePair<STATE_LOCK_OCCASION, bool> kvp)
	{
		KeyValuePair<STATE_LOCK_OCCASION, bool> keyValuePair = kvp;
		return keyValuePair.Value;
	}).Select(delegate(KeyValuePair<STATE_LOCK_OCCASION, bool> kvp)
	{
		KeyValuePair<STATE_LOCK_OCCASION, bool> keyValuePair = kvp;
		return keyValuePair.Key.ToString();
	}).ToArray());

	public static void Reset()
	{
		RemovePowerStateLock(STATE_LOCK_OCCASION.POILCE);
		RemovePowerStateLock(STATE_LOCK_OCCASION.HITMAN);
		RemovePowerStateLock(STATE_LOCK_OCCASION.BREATHER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.DOLL_MAKER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.CULT);
		RemovePowerStateLock(STATE_LOCK_OCCASION.BOMB_MAKER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.TANNER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.KIDNAPPER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.EXECUTIONER);
		RemovePowerStateLock(STATE_LOCK_OCCASION.DELFALCO);
		RemovePowerStateLock(STATE_LOCK_OCCASION.NEWNOIR);
		RemovePowerStateLock(STATE_LOCK_OCCASION.TAROT);
		RemovePowerStateLock(STATE_LOCK_OCCASION.OUTLAW);
		RemovePowerStateLock(STATE_LOCK_OCCASION.INSANITY);
		RemovePowerStateLock(STATE_LOCK_OCCASION.GAME_END);
		RemovePowerStateLock(STATE_LOCK_OCCASION.DEVTOOLS);
	}

	public static void AddPowerStateLock(STATE_LOCK_OCCASION state)
	{
		_powerStateStackPool[state] = true;
	}

	public static void RemovePowerStateLock(STATE_LOCK_OCCASION state)
	{
		_powerStateStackPool[state] = false;
	}

	public static bool IsLocked()
	{
		return _powerStateStackPool.ContainsValue(value: true);
	}
}
