using System.Collections.Generic;

public static class WindowManager
{
	private static Dictionary<SOFTWARE_PRODUCTS, WindowBehaviour> _windows = new Dictionary<SOFTWARE_PRODUCTS, WindowBehaviour>();

	private static Dictionary<int, WindowBehaviour> _uniWindows = new Dictionary<int, WindowBehaviour>();

	public static void Add(WindowBehaviour WindowToAdd)
	{
		if (WindowToAdd.Product == SOFTWARE_PRODUCTS.UNIVERSAL)
		{
			if (!_uniWindows.ContainsKey(WindowToAdd.UniProductData.GetHashCode()))
			{
				_uniWindows.Add(WindowToAdd.UniProductData.GetHashCode(), WindowToAdd);
			}
		}
		else if (!_windows.ContainsKey(WindowToAdd.Product))
		{
			_windows.Add(WindowToAdd.Product, WindowToAdd);
		}
	}

	public static WindowBehaviour Get(SOFTWARE_PRODUCTS WindowToGet)
	{
		return _windows[WindowToGet];
	}

	public static WindowBehaviour Get(SoftwareProductDefinition UniWindow)
	{
		return _uniWindows[UniWindow.GetHashCode()];
	}

	public static void Remove(SOFTWARE_PRODUCTS WindowToRemove)
	{
		_windows.Remove(WindowToRemove);
	}

	public static void Remove(SoftwareProductDefinition UniWindow)
	{
		_uniWindows.Remove(UniWindow.GetHashCode());
	}

	public static void Clear()
	{
		_windows.Clear();
		_uniWindows.Clear();
		_windows = new Dictionary<SOFTWARE_PRODUCTS, WindowBehaviour>();
		_uniWindows = new Dictionary<int, WindowBehaviour>();
	}
}
