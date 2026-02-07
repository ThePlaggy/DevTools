using System;

[Serializable]
public struct MatrixStackCord
{
	public int X;

	public int Y;

	public static MatrixStackCord zero => new MatrixStackCord(0, 0);

	public MatrixStackCord(int x, int y)
	{
		X = x;
		Y = y;
	}

	public override int GetHashCode()
	{
		return X.GetHashCode() ^ X.GetHashCode();
	}

	public static bool operator ==(MatrixStackCord lhs, MatrixStackCord rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MatrixStackCord lhs, MatrixStackCord rhs)
	{
		return !lhs.Equals(rhs);
	}
}
