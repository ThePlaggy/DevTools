using System.Collections.Generic;

public class MatrixStack<T>
{
	private MatrixStackCord pointer = new MatrixStackCord(-1, 0);

	public int Size { get; private set; }

	public MatrixStackCord Pointer => pointer;

	public T[,] Data { get; private set; }

	public MatrixStack()
	{
		Size = 0;
		Data = new T[0, 0];
	}

	public MatrixStack(int MatrixSize)
	{
		Size = MatrixSize;
		Data = new T[MatrixSize, MatrixSize];
	}

	public void SetMatrixSize(int SetValue)
	{
		if (Size != SetValue)
		{
			Size = SetValue;
			Data = resizeArray(Data, SetValue, SetValue);
		}
	}

	public void Push(T Item)
	{
		if ((pointer.X + 1) * (pointer.Y + 1) < Size * Size)
		{
			bool flag = pointer.X + 1 > Size - 1;
			pointer.X = ((!flag) ? (pointer.X + 1) : 0);
			pointer.Y = ((!flag) ? pointer.Y : (pointer.Y + 1));
			Data[pointer.X, pointer.Y] = Item;
		}
	}

	public bool Set(T SetValue, MatrixStackCord SetCord)
	{
		if (SetCord.X < Size && SetCord.X >= 0 && SetCord.Y < Size && SetCord.Y >= 0)
		{
			Data[SetCord.X, SetCord.Y] = SetValue;
			return true;
		}
		return false;
	}

	public bool Set(T SetValue, int XCord, int YCord)
	{
		if (XCord < Size && XCord >= 0 && YCord < Size && YCord >= 0)
		{
			Data[XCord, YCord] = SetValue;
			return true;
		}
		return false;
	}

	public T Get(MatrixStackCord SetCord)
	{
		if (SetCord.X < Size && SetCord.X >= 0 && SetCord.Y < Size && SetCord.Y >= 0)
		{
			return Data[SetCord.X, SetCord.Y];
		}
		return default(T);
	}

	public T Get(int XCord, int YCord)
	{
		if (XCord < Size && XCord >= 0 && YCord < Size && YCord >= 0)
		{
			return Data[XCord, YCord];
		}
		return default(T);
	}

	public bool TryAndGetValue(out T ReturnValue, MatrixStackCord SetCord)
	{
		if (SetCord.X < Size && SetCord.X >= 0 && SetCord.Y < Size && SetCord.Y >= 0)
		{
			ReturnValue = Data[SetCord.X, SetCord.Y];
			return true;
		}
		ReturnValue = default(T);
		return false;
	}

	public bool TryAndGetValue(out T ReturnValue, int XCord, int YCord)
	{
		if (XCord < Size && XCord >= 0 && YCord < Size && YCord >= 0)
		{
			ReturnValue = Data[XCord, YCord];
			return true;
		}
		ReturnValue = default(T);
		return false;
	}

	public bool TryAndGetValueByClock(out T ReturnValue, MatrixStackCord SetCord, MATRIX_STACK_CLOCK_POSITION ClockPOS)
	{
		bool result = false;
		switch (ClockPOS)
		{
		case MATRIX_STACK_CLOCK_POSITION.HIGH_NOON:
		{
			MatrixStackCord setCord9 = new MatrixStackCord(SetCord.X, SetCord.Y - 2);
			result = TryAndGetValue(out ReturnValue, setCord9);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.NOON:
		{
			MatrixStackCord setCord8 = new MatrixStackCord(SetCord.X, SetCord.Y - 1);
			result = TryAndGetValue(out ReturnValue, setCord8);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.ONE:
		{
			MatrixStackCord setCord7 = new MatrixStackCord(SetCord.X + 1, SetCord.Y - 1);
			result = TryAndGetValue(out ReturnValue, setCord7);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.THREE:
		{
			MatrixStackCord setCord6 = new MatrixStackCord(SetCord.X + 1, SetCord.Y);
			result = TryAndGetValue(out ReturnValue, setCord6);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.FOUR:
		{
			MatrixStackCord setCord5 = new MatrixStackCord(SetCord.X + 1, SetCord.Y + 1);
			result = TryAndGetValue(out ReturnValue, setCord5);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.SIX:
		{
			MatrixStackCord setCord4 = new MatrixStackCord(SetCord.X, SetCord.Y + 1);
			result = TryAndGetValue(out ReturnValue, setCord4);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.SEVEN:
		{
			MatrixStackCord setCord3 = new MatrixStackCord(SetCord.X - 1, SetCord.Y + 1);
			result = TryAndGetValue(out ReturnValue, setCord3);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.NINE:
		{
			MatrixStackCord setCord2 = new MatrixStackCord(SetCord.X - 1, SetCord.Y);
			result = TryAndGetValue(out ReturnValue, setCord2);
			break;
		}
		case MATRIX_STACK_CLOCK_POSITION.TEN:
		{
			MatrixStackCord setCord = new MatrixStackCord(SetCord.X - 1, SetCord.Y - 1);
			result = TryAndGetValue(out ReturnValue, setCord);
			break;
		}
		default:
			ReturnValue = default(T);
			break;
		}
		return result;
	}

	public IEnumerable<T> GetAll()
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				yield return Data[j, i];
			}
		}
	}

	public void Clear()
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				Data[i, j] = default(T);
			}
		}
		pointer = new MatrixStackCord(-1, 0);
	}

	private T[,] resizeArray(T[,] original, int rows, int cols)
	{
		return new T[rows, cols];
	}
}
