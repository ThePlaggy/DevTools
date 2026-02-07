public class Device
{
	public string name;

	public float genDos;

	public float genTime;

	public DeviceType deviceType;

	public int hackPrice;

	public bool isOffline = false;

	public int hackDifficulty;

	public Device BuildMe(string myName, float dos, float time, DeviceType type, int price, int difficulty)
	{
		name = myName;
		genDos = dos;
		genTime = time;
		deviceType = type;
		hackPrice = price;
		hackDifficulty = difficulty;
		return this;
	}
}
