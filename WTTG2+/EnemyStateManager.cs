using System.Collections.Generic;
using System.Linq;

public static class EnemyStateManager
{
	private static Dictionary<ENEMY_STATE, bool> _enemyStateStackPool = new Dictionary<ENEMY_STATE, bool>
	{
		{
			ENEMY_STATE.POILCE,
			false
		},
		{
			ENEMY_STATE.HITMAN,
			false
		},
		{
			ENEMY_STATE.BREATHER,
			false
		},
		{
			ENEMY_STATE.DOLL_MAKER,
			false
		},
		{
			ENEMY_STATE.CULT,
			false
		},
		{
			ENEMY_STATE.BOMB_MAKER,
			false
		},
		{
			ENEMY_STATE.TANNER,
			false
		},
		{
			ENEMY_STATE.KIDNAPPER,
			false
		},
		{
			ENEMY_STATE.EXECUTIONER,
			false
		},
		{
			ENEMY_STATE.DELFALCO,
			false
		},
		{
			ENEMY_STATE.NEWNOIR,
			false
		}
	};

	private static Dictionary<STATE_LOCK_OCCASION, bool> _lockedStateStackPool = new Dictionary<STATE_LOCK_OCCASION, bool>
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

	public static string EnemyStateDebug => (!IsInEnemyState()) ? "IDLE" : string.Join(" + ", _enemyStateStackPool.Where(delegate(KeyValuePair<ENEMY_STATE, bool> kvp)
	{
		KeyValuePair<ENEMY_STATE, bool> keyValuePair = kvp;
		return keyValuePair.Value;
	}).Select(delegate(KeyValuePair<ENEMY_STATE, bool> kvp)
	{
		KeyValuePair<ENEMY_STATE, bool> keyValuePair = kvp;
		return keyValuePair.Key.ToString();
	}).ToArray());

	public static string LockedStateDebug => (!IsEnemyStateLocked()) ? "NOT_LOCKED" : string.Join(" + ", _lockedStateStackPool.Where(delegate(KeyValuePair<STATE_LOCK_OCCASION, bool> kvp)
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
		RemoveEnemyState(ENEMY_STATE.POILCE);
		RemoveEnemyState(ENEMY_STATE.HITMAN);
		RemoveEnemyState(ENEMY_STATE.BREATHER);
		RemoveEnemyState(ENEMY_STATE.DOLL_MAKER);
		RemoveEnemyState(ENEMY_STATE.CULT);
		RemoveEnemyState(ENEMY_STATE.BOMB_MAKER);
		RemoveEnemyState(ENEMY_STATE.TANNER);
		RemoveEnemyState(ENEMY_STATE.KIDNAPPER);
		RemoveEnemyState(ENEMY_STATE.EXECUTIONER);
		RemoveEnemyState(ENEMY_STATE.DELFALCO);
		RemoveEnemyState(ENEMY_STATE.NEWNOIR);
		UnlockEnemyState(STATE_LOCK_OCCASION.POILCE);
		UnlockEnemyState(STATE_LOCK_OCCASION.HITMAN);
		UnlockEnemyState(STATE_LOCK_OCCASION.BREATHER);
		UnlockEnemyState(STATE_LOCK_OCCASION.DOLL_MAKER);
		UnlockEnemyState(STATE_LOCK_OCCASION.CULT);
		UnlockEnemyState(STATE_LOCK_OCCASION.BOMB_MAKER);
		UnlockEnemyState(STATE_LOCK_OCCASION.TANNER);
		UnlockEnemyState(STATE_LOCK_OCCASION.KIDNAPPER);
		UnlockEnemyState(STATE_LOCK_OCCASION.EXECUTIONER);
		UnlockEnemyState(STATE_LOCK_OCCASION.DELFALCO);
		UnlockEnemyState(STATE_LOCK_OCCASION.NEWNOIR);
		UnlockEnemyState(STATE_LOCK_OCCASION.TAROT);
		UnlockEnemyState(STATE_LOCK_OCCASION.OUTLAW);
		UnlockEnemyState(STATE_LOCK_OCCASION.INSANITY);
		UnlockEnemyState(STATE_LOCK_OCCASION.GAME_END);
		UnlockEnemyState(STATE_LOCK_OCCASION.DEVTOOLS);
	}

	public static void AddEnemyState(ENEMY_STATE state)
	{
		_enemyStateStackPool[state] = true;
	}

	public static void RemoveEnemyState(ENEMY_STATE state)
	{
		_enemyStateStackPool[state] = false;
	}

	public static bool HasEnemyState(ENEMY_STATE state)
	{
		return _enemyStateStackPool[state];
	}

	public static bool IsInEnemyState()
	{
		return _enemyStateStackPool.ContainsValue(value: true);
	}

	public static void LockEnemyState(STATE_LOCK_OCCASION state)
	{
		_lockedStateStackPool[state] = true;
	}

	public static void UnlockEnemyState(STATE_LOCK_OCCASION state)
	{
		_lockedStateStackPool[state] = false;
	}

	public static bool IsEnemyStateLocked()
	{
		return _lockedStateStackPool.ContainsValue(value: true);
	}

	public static bool IsInEnemyStateOrLocked()
	{
		return IsEnemyStateLocked() || IsInEnemyState();
	}
}
