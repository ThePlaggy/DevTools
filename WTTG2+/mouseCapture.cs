using System;
using UnityEngine;

[Serializable]
public class mouseCapture
{
	public float XSensitivity = 2f;

	public float YSensitivity = 2f;

	public bool clampVerticalRotation = true;

	public bool clampHorzRotation;

	public float MinVertX = -90f;

	public float MaxVertX = 90f;

	public float MinHorzY = -180f;

	public float MaxHorzY = 180f;

	public bool smooth;

	public float smoothTime = 5f;

	private bool lockCharRot;

	private Camera myCamera;

	private GameObject MyRotatingCamera;

	private Quaternion myRotatingCameraTargetRot;

	private GameObject MyRotatingObject;

	private Quaternion myRotatingObjectTargetRot;

	private bool restrictHorz;

	private float restrictMaxHorzY;

	private float restrictMaxVertX;

	private float restrictMinHorzY;

	private float restrictMinVertX;

	private bool restrictMovement;

	private bool restricVert;

	public void Init(GameObject RotatingObject, GameObject RotatingCamera)
	{
		MyRotatingObject = RotatingObject;
		MyRotatingCamera = RotatingCamera;
		myRotatingObjectTargetRot = MyRotatingObject.transform.localRotation;
		myRotatingCameraTargetRot = MyRotatingCamera.transform.localRotation;
	}

	public void Init(Camera setCamera)
	{
		myCamera = setCamera;
		myRotatingCameraTargetRot = myCamera.transform.localRotation;
	}

	public void setCameraTargetRot(float setX = 0f)
	{
		myRotatingCameraTargetRot = Quaternion.Euler(setX, 0f, 0f);
	}

	public void setFullCameraTargetRot(Vector3 setRot)
	{
		myRotatingCameraTargetRot = Quaternion.Euler(setRot);
	}

	public void setRotatingObjectTargetRot(Vector3 setValue)
	{
		myRotatingObjectTargetRot = Quaternion.Euler(setValue);
	}

	public void setRotatingObjectRotation(Vector3 setvalue)
	{
		MyRotatingObject.transform.localRotation = Quaternion.Euler(setvalue);
	}

	public void setRestrictMovement(bool setValue)
	{
		setRestrictMovement(setValue, 0f, 0f, 0f, 0f);
	}

	public void setRestrictMovement(bool setValue, float setMinVX, float setMaxVX, float setMinHY, float setMaxHY)
	{
		restrictMovement = setValue;
		restrictHorz = setValue;
		restricVert = setValue;
		restrictMinVertX = setMinVX;
		restrictMaxVertX = setMaxVX;
		restrictMinHorzY = setMinHY;
		restrictMaxHorzY = setMaxHY;
	}

	public void setRestricVertMovement(bool setValue, float setMinVX, float setMaxVX)
	{
		restrictMovement = setValue;
		restricVert = setValue;
		restrictMinVertX = setMinVX;
		restrictMaxVertX = setMaxVX;
	}

	public void setRestricHorzMovement(bool setValue, float setMinHY, float setMaxHY)
	{
		restrictMovement = setValue;
		restrictHorz = setValue;
		restrictMinHorzY = setMinHY;
		restrictMaxHorzY = setMaxHY;
	}

	public void updateRestricVertMin(float setValue)
	{
		restrictMinVertX = setValue;
	}

	public void updateRestricVertMax(float setValue)
	{
		restrictMaxVertX = setValue;
	}

	public void updateRestricHorzMin(float setValue)
	{
		restrictMinHorzY = setValue;
	}

	public void updateRestricHorzMax(float setValue)
	{
		restrictMaxHorzY = setValue;
	}

	public void updateMinVertX(float setValue)
	{
		MinVertX = setValue;
	}

	public void setLockCharROT(bool setValue)
	{
		lockCharRot = setValue;
	}

	public void LookRotation()
	{
		float num = Input.GetAxis("Mouse Y") * XSensitivity;
		float y = Input.GetAxis("Mouse X") * YSensitivity;
		myRotatingObjectTargetRot *= Quaternion.Euler(0f, y, 0f);
		myRotatingCameraTargetRot *= Quaternion.Euler(0f - num, 0f, 0f);
		if (clampVerticalRotation)
		{
			myRotatingCameraTargetRot = ClampRotationAroundXAxis(myRotatingCameraTargetRot);
		}
		if (clampHorzRotation)
		{
			myRotatingObjectTargetRot = ClampRotationAroundYAxis(myRotatingObjectTargetRot);
		}
		if (restrictMovement)
		{
			if (restricVert)
			{
				myRotatingCameraTargetRot = ClampRotationAroundXAxis(myRotatingCameraTargetRot);
			}
			if (restrictHorz)
			{
				myRotatingObjectTargetRot = ClampRotationAroundYAxis(myRotatingObjectTargetRot);
			}
		}
		if (smooth)
		{
			if (!lockCharRot)
			{
				MyRotatingObject.transform.localRotation = Quaternion.Slerp(MyRotatingObject.transform.localRotation, myRotatingObjectTargetRot, smoothTime * Time.deltaTime);
			}
			MyRotatingCamera.transform.localRotation = Quaternion.Slerp(MyRotatingCamera.transform.localRotation, myRotatingCameraTargetRot, smoothTime * Time.deltaTime);
		}
		else
		{
			if (!lockCharRot)
			{
				MyRotatingObject.transform.localRotation = myRotatingObjectTargetRot;
			}
			MyRotatingCamera.transform.localRotation = myRotatingCameraTargetRot;
		}
	}

	public Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float value = 114.59156f * Mathf.Atan(q.x);
		value = ((!restricVert) ? Mathf.Clamp(value, MinVertX, MaxVertX) : Mathf.Clamp(value, restrictMinVertX, restrictMaxVertX));
		q.x = Mathf.Tan((float)Math.PI / 360f * value);
		return q;
	}

	public Quaternion ClampRotationAroundYAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1f;
		float value = 114.59156f * Mathf.Atan(q.y);
		value = ((!restrictHorz) ? Mathf.Clamp(value, MinHorzY, MaxHorzY) : Mathf.Clamp(value, restrictMinHorzY, restrictMaxHorzY));
		q.y = Mathf.Tan((float)Math.PI / 360f * value);
		return q;
	}
}
