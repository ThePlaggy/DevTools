using System;
using UnityEngine;

[Serializable]
public struct SerTrans
{
	public float POSX;

	public float POSY;

	public float POSZ;

	public float ROTX;

	public float ROTY;

	public float ROTZ;

	public Vector3 PositionToVector3 => new Vector3(POSX, POSY, POSZ);

	public Vector3 RotationToVector3 => new Vector3(ROTX, ROTY, ROTZ);

	public SerTrans(float POSX = 0f, float POSY = 0f, float POSZ = 0f, float ROTX = 0f, float ROTY = 0f, float ROTZ = 0f)
	{
		this.POSX = POSX;
		this.POSY = POSY;
		this.POSZ = POSZ;
		this.ROTX = ROTX;
		this.ROTY = ROTY;
		this.ROTZ = ROTZ;
	}

	public static SerTrans Convert(Vector3 SetPOS, Vector3 SetROT)
	{
		return new SerTrans(SetPOS.x, SetPOS.y, SetPOS.z, SetROT.x, SetROT.y, SetROT.z);
	}

	public override bool Equals(object obj)
	{
		return obj is SerTrans && Equals((SerTrans)obj);
	}

	public bool Equals(SerTrans other)
	{
		return POSX == other.POSX && POSY == other.POSY && POSZ == other.POSZ && ROTX == other.ROTX && ROTY == other.ROTY && ROTZ == other.ROTZ;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(SerTrans lhs, SerTrans rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SerTrans lhs, SerTrans rhs)
	{
		return !lhs.Equals(rhs);
	}
}
