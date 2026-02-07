using System;
using UnityEngine;

[Serializable]
public struct Vect2
{
	public float X;

	public float Y;

	public static Vect2 zero => new Vect2(0f, 0f);

	public Vector2 ToVector2 => new Vector2(X, Y);

	public Vect2(float x, float y)
	{
		X = x;
		Y = y;
	}

	public static Vect2 Convert(Vector2 From)
	{
		return new Vect2(From.x, From.y);
	}

	public override bool Equals(object obj)
	{
		return obj is Vect2 && Equals((Vect2)obj);
	}

	public bool Equals(Vect2 other)
	{
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static bool operator ==(Vect2 lhs, Vect2 rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Vect2 lhs, Vect2 rhs)
	{
		return !lhs.Equals(rhs);
	}
}
