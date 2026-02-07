using System;

[Serializable]
public class SweeperHackData : DataObject
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

	public SweeperHackData(int SetID)
		: base(SetID)
	{
	}
}
