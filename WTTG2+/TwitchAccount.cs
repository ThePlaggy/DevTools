using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class TwitchAccount
{
	private static string DataPath = Application.persistentDataPath + "/WTTG2PlusTTV.gd";

	private static byte[] key = new byte[8] { 5, 2, 1, 4, 3, 7, 6, 8 };

	private static byte[] iv = new byte[8] { 5, 2, 1, 4, 3, 7, 6, 8 };

	public bool DataExists;

	public long ExpiresIn;

	public string Id;

	public bool Manual;

	public string OAuth;

	public string Login;

	public string Username;

	public TwitchAccount Build(TTVAccountModel Model)
	{
		ExpiresIn = Model.ExpiresIn;
		Id = Model.TwitchId;
		OAuth = Model.Data;
		Login = Model.TwitchLogin;
		Username = Model.TwitchUsername;
		Manual = Model.Manual;
		Save();
		return this;
	}

	public void Save()
	{
		DataExists = OAuth != string.Empty;
		File.WriteAllText(DataPath, Crypt(JsonUtility.ToJson(this)));
	}

	public TwitchAccount Load()
	{
		if (File.Exists(Application.persistentDataPath + "/WTTG2Plus.gd"))
		{
			File.Delete(Application.persistentDataPath + "/WTTG2Plus.gd");
		}
		if (!File.Exists(DataPath))
		{
			ExpiresIn = 0L;
			Id = "Unknown";
			OAuth = string.Empty;
			Login = null;
			Username = "Unknown";
			Manual = true;
			Save();
			return this;
		}
		JsonUtility.FromJsonOverwrite(Decrypt(File.ReadAllText(DataPath)), this);
		return this;
	}

	public void Clear()
	{
		Id = "Unknown";
		Username = "Unknown";
		ExpiresIn = 0L;
		Login = null;
		OAuth = string.Empty;
		Manual = true;
		Save();
	}

	private static string Crypt(string text)
	{
		ICryptoTransform cryptoTransform = DES.Create().CreateEncryptor(key, iv);
		byte[] bytes = Encoding.Unicode.GetBytes(text);
		return Convert.ToBase64String(cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length));
	}

	private static string Decrypt(string text)
	{
		ICryptoTransform cryptoTransform = DES.Create().CreateDecryptor(key, iv);
		byte[] array = Convert.FromBase64String(text);
		byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.Unicode.GetString(bytes);
	}
}
