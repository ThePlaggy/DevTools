using System;

[Serializable]
public class StackPusherHackData : DataObject
{
	private int skillPoints;

	public int SkillPoints
	{
		get
		{
			return skillPoints;
		}
		set
		{
			skillPoints = value;
			if (skillPoints <= 0)
			{
				skillPoints = 0;
			}
		}
	}

	public StackPusherHackData(int SetID)
		: base(SetID)
	{
	}
}
