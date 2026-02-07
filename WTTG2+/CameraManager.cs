using System.Collections.Generic;
using UnityEngine;

public static class CameraManager
{
	private static Dictionary<CAMERA_ID, Camera> _cameras = new Dictionary<CAMERA_ID, Camera>();

	public static void Add(CAMERA_ID SetCameraID, Camera SetCamera)
	{
		if (!_cameras.ContainsKey(SetCameraID))
		{
			_cameras.Add(SetCameraID, SetCamera);
		}
	}

	public static bool Get(CAMERA_ID CameraToGetID, out Camera ReturnCamera)
	{
		return _cameras.TryGetValue(CameraToGetID, out ReturnCamera);
	}

	public static CameraHook GetCameraHook(CAMERA_ID CameraToGetID)
	{
		if (_cameras.TryGetValue(CameraToGetID, out var value))
		{
			return value.gameObject.GetComponent<CameraHook>();
		}
		return null;
	}

	public static void Remove(CAMERA_ID CameraToRemove)
	{
		_cameras.Remove(CameraToRemove);
	}
}
