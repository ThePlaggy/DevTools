using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZenFulcrum.EmbeddedBrowser;

public class BrowserCursor
{
	public class CursorInfo
	{
		public int atlasOffset;

		public Vector2 hotspot;
	}

	private static Dictionary<BrowserNative.CursorType, CursorInfo> mapping = new Dictionary<BrowserNative.CursorType, CursorInfo>();

	private static bool loaded;

	private static int size;

	private static Texture2D allCursors;

	protected Texture2D customTexture;

	protected Texture2D normalTexture;

	public virtual Texture2D Texture { get; protected set; }

	public virtual Vector2 Hotspot { get; protected set; }

	public event Action cursorChange = delegate
	{
	};

	public BrowserCursor()
	{
		Load();
		normalTexture = new Texture2D(size, size, TextureFormat.ARGB32, mipChain: false);
		SetActiveCursor(BrowserNative.CursorType.Pointer);
	}

	private static void Load()
	{
		if (!loaded)
		{
			allCursors = Resources.Load<Texture2D>("Browser/Cursors");
			if (!allCursors)
			{
				throw new Exception("Failed to find browser allCursors");
			}
			size = allCursors.height;
			TextAsset textAsset = Resources.Load<TextAsset>("Browser/Cursors");
			string[] array = textAsset.text.Split('\n');
			foreach (string text in array)
			{
				string[] array2 = text.Split(',');
				BrowserNative.CursorType key = (BrowserNative.CursorType)Enum.Parse(typeof(BrowserNative.CursorType), array2[0]);
				CursorInfo value = new CursorInfo
				{
					atlasOffset = int.Parse(array2[1]),
					hotspot = new Vector2(int.Parse(array2[2]), int.Parse(array2[3]))
				};
				mapping[key] = value;
			}
			loaded = true;
		}
	}

	public virtual void SetActiveCursor(BrowserNative.CursorType type)
	{
		switch (type)
		{
		case BrowserNative.CursorType.Custom:
			throw new ArgumentException("Use SetCustomCursor to set custom cursors.", "type");
		case BrowserNative.CursorType.None:
			Texture = null;
			this.cursorChange();
			return;
		}
		CursorInfo cursorInfo = mapping[type];
		Color[] pixels = allCursors.GetPixels(cursorInfo.atlasOffset * size, 0, size, size);
		Hotspot = cursorInfo.hotspot;
		normalTexture.SetPixels(pixels);
		normalTexture.Apply(updateMipmaps: true);
		Texture = normalTexture;
		this.cursorChange();
	}

	public virtual void SetCustomCursor(Texture2D cursor, Vector2 hotspot)
	{
		if (!customTexture || customTexture.width != cursor.width || customTexture.height != cursor.height)
		{
			UnityEngine.Object.Destroy(customTexture);
			customTexture = new Texture2D(cursor.width, cursor.height, TextureFormat.ARGB32, mipChain: false);
		}
		customTexture.SetPixels32(cursor.GetPixels32());
		customTexture.Apply(updateMipmaps: true);
		Hotspot = hotspot;
		Texture = customTexture;
		this.cursorChange();
	}
}
