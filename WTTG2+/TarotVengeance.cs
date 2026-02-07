using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TarotVengeance
{
	private static Dictionary<ENEMY_STATE, bool> _tarotVengeanceEnemyPool = new Dictionary<ENEMY_STATE, bool>
	{
		{
			ENEMY_STATE.BREATHER,
			false
		},
		{
			ENEMY_STATE.HITMAN,
			false
		},
		{
			ENEMY_STATE.TANNER,
			false
		},
		{
			ENEMY_STATE.EXECUTIONER,
			false
		},
		{
			ENEMY_STATE.KIDNAPPER,
			false
		},
		{
			ENEMY_STATE.DOLL_MAKER,
			false
		},
		{
			ENEMY_STATE.BOMB_MAKER,
			false
		},
		{
			ENEMY_STATE.DELFALCO,
			false
		}
	};

	public static List<ENEMY_STATE> _tarotVengeanceActivePool = new List<ENEMY_STATE>();

	public static string ActiveEnemies => string.Join(" + ", _tarotVengeanceActivePool.Select((ENEMY_STATE e) => e.ToString()).ToArray());

	public static string KilledEnemies => string.Join(" + ", _tarotVengeanceEnemyPool.Where(delegate(KeyValuePair<ENEMY_STATE, bool> kvp)
	{
		KeyValuePair<ENEMY_STATE, bool> keyValuePair = kvp;
		return keyValuePair.Value;
	}).Select(delegate(KeyValuePair<ENEMY_STATE, bool> kvp)
	{
		KeyValuePair<ENEMY_STATE, bool> keyValuePair = kvp;
		return keyValuePair.Key.ToString();
	}).ToArray());

	public static void ActivateEnemy(ENEMY_STATE state)
	{
		if (!_tarotVengeanceActivePool.Contains(state))
		{
			_tarotVengeanceActivePool.Add(state);
		}
	}

	public static void Clear()
	{
		_tarotVengeanceActivePool.Clear();
		_tarotVengeanceEnemyPool[ENEMY_STATE.BREATHER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.HITMAN] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.TANNER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.EXECUTIONER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.KIDNAPPER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.DOLL_MAKER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.BOMB_MAKER] = false;
		_tarotVengeanceEnemyPool[ENEMY_STATE.DELFALCO] = false;
	}

	public static void KillActiveEnemy()
	{
		if (_tarotVengeanceActivePool.Count > 0)
		{
			ENEMY_STATE eNEMY_STATE = _tarotVengeanceActivePool[Random.Range(0, _tarotVengeanceActivePool.Count)];
			Debug.LogFormat("[TarotVengeance] Killed {0}", eNEMY_STATE);
			_tarotVengeanceEnemyPool[eNEMY_STATE] = true;
		}
	}

	public static bool Killed(ENEMY_STATE state)
	{
		return _tarotVengeanceEnemyPool[state];
	}
}
