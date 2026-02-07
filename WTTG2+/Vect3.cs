using System;
using UnityEngine;

[Serializable]
public struct Vect3
{
	public float X;

	public float Y;

	public float Z;

	public static Vect3 zero => new Vect3(0f, 0f, 0f);

	public Vector3 ToVector3 => new Vector3(X, Y, Z);

	public Vect3(float x, float y, float z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public override bool Equals(object obj)
	{
		return obj is Vect3 && Equals((Vect3)obj);
	}

	public bool Equals(Vect3 other)
	{
		return X == other.X && Y == other.Y && Z == other.Z;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Vect3 lhs, Vect3 rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Vect3 lhs, Vect3 rhs)
	{
		return !lhs.Equals(rhs);
	}
}
