using System;

[Serializable]
public class CameraData
{
	public CAMERA_ID MyID;

	public Vect3 LastPosition;

	public Vect3 LastRotation;

	public float LastFOV;

	public CameraData(CAMERA_ID SetCameraID, Vect3 SetLastPosition, Vect3 SetLastRotation, float SetLastFOV)
	{
		MyID = SetCameraID;
		LastPosition = SetLastPosition;
		LastRotation = SetLastRotation;
		LastFOV = SetLastFOV;
	}
}
