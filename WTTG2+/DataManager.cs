using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager
{
	private static bool _lockSave;

	private static bool _continueFile;

	private static bool _savedOptions;

	private static DataLookUp _dataLookUp;

	private static DataLookUp _optionLookUp;

	private static BinaryFormatter _bf;

	private static string _gameFileName;

	private static string _optionFileName;

	public static bool LockSave
	{
		get
		{
			return _lockSave;
		}
		set
		{
			_lockSave = value;
		}
	}

	public static bool ContinuedGame => false;

	public static bool SavedOptions => _savedOptions;

	static DataManager()
	{
		_bf = new BinaryFormatter();
		_gameFileName = Application.persistentDataPath + "/WTTG2PlusSave.gd";
		_optionFileName = Application.persistentDataPath + "/WTTG2PlusOptions.gd";
		_dataLookUp = new DataLookUp();
		_optionLookUp = new DataLookUp();
		if (File.Exists(_gameFileName))
		{
			_continueFile = true;
			FileStream fileStream = File.Open(_gameFileName, FileMode.Open);
			_dataLookUp = (DataLookUp)_bf.Deserialize(fileStream);
			fileStream.Close();
		}
		if (File.Exists(_optionFileName))
		{
			_savedOptions = true;
			FileStream fileStream2 = File.Open(_optionFileName, FileMode.Open);
			_optionLookUp = (DataLookUp)_bf.Deserialize(fileStream2);
			fileStream2.Close();
		}
	}

	public static void Save<T>(T DataToSave) where T : class, IDataObject
	{
		if (!_lockSave && StateManager.PlayerState != PLAYER_STATE.BUSY)
		{
			if (_dataLookUp.Data.TryGetValue(typeof(T), out var value))
			{
				value.Add(DataToSave.ID, DataToSave);
				_dataLookUp.Data[typeof(T)] = value;
			}
			else
			{
				value = new DataContainer();
				value.Add(DataToSave.ID, DataToSave);
				_dataLookUp.Data.Add(typeof(T), value);
			}
		}
	}

	public static void SaveOption<T>(T DataToSave) where T : class, IDataObject
	{
		if (_optionLookUp.Data.TryGetValue(typeof(T), out var value))
		{
			value.Add(DataToSave.ID, DataToSave);
			_optionLookUp.Data[typeof(T)] = value;
		}
		else
		{
			value = new DataContainer();
			value.Add(DataToSave.ID, DataToSave);
			_optionLookUp.Data.Add(typeof(T), value);
		}
	}

	public static T Load<T>(int LookUpID) where T : class, IDataObject
	{
		if (!_dataLookUp.Data.TryGetValue(typeof(T), out var value))
		{
			return null;
		}
		if (value.MyData.TryGetValue(LookUpID, out var value2))
		{
			return (T)value2;
		}
		return null;
	}

	public static T LoadOption<T>(int LookUpID) where T : class, IDataObject
	{
		if (!_optionLookUp.Data.TryGetValue(typeof(T), out var value))
		{
			return null;
		}
		if (value.MyData.TryGetValue(LookUpID, out var value2))
		{
			return (T)value2;
		}
		return null;
	}

	public static void Reset()
	{
		_lockSave = false;
		_dataLookUp = new DataLookUp();
		_optionLookUp = new DataLookUp();
		if (File.Exists(_gameFileName))
		{
			_continueFile = true;
			FileStream fileStream = File.Open(_gameFileName, FileMode.Open);
			_dataLookUp = (DataLookUp)_bf.Deserialize(fileStream);
			fileStream.Close();
		}
		if (File.Exists(_optionFileName))
		{
			_savedOptions = true;
			FileStream fileStream2 = File.Open(_optionFileName, FileMode.Open);
			_optionLookUp = (DataLookUp)_bf.Deserialize(fileStream2);
			fileStream2.Close();
		}
	}

	public static void ClearGameData()
	{
		_continueFile = false;
		_dataLookUp = new DataLookUp();
		File.Delete(_gameFileName);
	}

	public static void WriteData()
	{
	}

	public static void WriteOptionData()
	{
		FileStream fileStream = File.Create(_optionFileName);
		_bf.Serialize(fileStream, _optionLookUp);
		fileStream.Close();
	}
}
