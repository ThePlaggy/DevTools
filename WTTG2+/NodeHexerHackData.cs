using System;

[Serializable]
public class NodeHexerHackData : DataObject
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

	public NodeHexerHackData(int SetID)
		: base(SetID)
	{
	}
}
