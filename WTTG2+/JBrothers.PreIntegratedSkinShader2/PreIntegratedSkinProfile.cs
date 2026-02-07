using UnityEngine;

namespace JBrothers.PreIntegratedSkinShader2;

public class PreIntegratedSkinProfile : ScriptableObject
{
	public Vector4 gauss6_1;

	public Vector4 gauss6_2;

	public Vector4 gauss6_3;

	public Vector4 gauss6_4;

	public Vector4 gauss6_5;

	public Vector4 gauss6_6;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths1_var1;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths2_var2;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths3_var3;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths4_var4;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths5_var5;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_weighths6_var6;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_sqrtvar1234;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_transl123_sqrtvar5;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileHigh_transl456_sqrtvar6;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileMedium_weighths1_var1;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileMedium_weighths2_var2;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileMedium_weighths3_var3;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileMedium_transl123;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileMedium_sqrtvar123;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileLow_weighths1_var1;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileLow_weighths2_var2;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileLow_sqrtvar12;

	[HideInInspector]
	[SerializeField]
	private Vector4 _PSSProfileLow_transl;

	[HideInInspector]
	public bool needsRenormalize = true;

	[HideInInspector]
	public bool needsRecalcDerived = true;

	[HideInInspector]
	public Texture2D referenceTexture;

	public void NormalizeOriginalWeights()
	{
		RecalculateDerived();
		gauss6_1.x = _PSSProfileHigh_weighths1_var1.x;
		gauss6_1.y = _PSSProfileHigh_weighths1_var1.y;
		gauss6_1.z = _PSSProfileHigh_weighths1_var1.z;
		gauss6_2.x = _PSSProfileHigh_weighths2_var2.x;
		gauss6_2.y = _PSSProfileHigh_weighths2_var2.y;
		gauss6_2.z = _PSSProfileHigh_weighths2_var2.z;
		gauss6_3.x = _PSSProfileHigh_weighths3_var3.x;
		gauss6_3.y = _PSSProfileHigh_weighths3_var3.y;
		gauss6_3.z = _PSSProfileHigh_weighths3_var3.z;
		gauss6_4.x = _PSSProfileHigh_weighths4_var4.x;
		gauss6_4.y = _PSSProfileHigh_weighths4_var4.y;
		gauss6_4.z = _PSSProfileHigh_weighths4_var4.z;
		gauss6_5.x = _PSSProfileHigh_weighths5_var5.x;
		gauss6_5.y = _PSSProfileHigh_weighths5_var5.y;
		gauss6_5.z = _PSSProfileHigh_weighths5_var5.z;
		gauss6_6.x = _PSSProfileHigh_weighths6_var6.x;
		gauss6_6.y = _PSSProfileHigh_weighths6_var6.y;
		gauss6_6.z = _PSSProfileHigh_weighths6_var6.z;
		needsRenormalize = false;
	}

	public void RecalculateDerived()
	{
		Vector3 zero = Vector3.zero;
		Vector3 vector = gauss6_1;
		Vector3 vector2 = gauss6_2;
		Vector3 vector3 = gauss6_3;
		Vector3 vector4 = gauss6_4;
		Vector3 vector5 = gauss6_5;
		Vector3 vector6 = gauss6_6;
		zero += vector;
		zero += vector2;
		zero += vector3;
		zero += vector4;
		zero += vector5;
		zero += vector6;
		vector.x /= zero.x;
		vector.y /= zero.y;
		vector.z /= zero.z;
		vector2.x /= zero.x;
		vector2.y /= zero.y;
		vector2.z /= zero.z;
		vector3.x /= zero.x;
		vector3.y /= zero.y;
		vector3.z /= zero.z;
		vector4.x /= zero.x;
		vector4.y /= zero.y;
		vector4.z /= zero.z;
		vector5.x /= zero.x;
		vector5.y /= zero.y;
		vector5.z /= zero.z;
		vector6.x /= zero.x;
		vector6.y /= zero.y;
		vector6.z /= zero.z;
		float grayscale = new Color(vector.x, vector.y, vector.z).grayscale;
		float grayscale2 = new Color(vector2.x, vector2.y, vector2.z).grayscale;
		float grayscale3 = new Color(vector3.x, vector3.y, vector3.z).grayscale;
		float grayscale4 = new Color(vector4.x, vector4.y, vector4.z).grayscale;
		float grayscale5 = new Color(vector5.x, vector5.y, vector5.z).grayscale;
		float grayscale6 = new Color(vector6.x, vector6.y, vector6.z).grayscale;
		_PSSProfileHigh_weighths1_var1 = new Vector4(vector.x, vector.y, vector.z, gauss6_1.w);
		_PSSProfileHigh_weighths2_var2 = new Vector4(vector2.x, vector2.y, vector2.z, gauss6_2.w);
		_PSSProfileHigh_weighths3_var3 = new Vector4(vector3.x, vector3.y, vector3.z, gauss6_3.w);
		_PSSProfileHigh_weighths4_var4 = new Vector4(vector4.x, vector4.y, vector4.z, gauss6_4.w);
		_PSSProfileHigh_weighths5_var5 = new Vector4(vector5.x, vector5.y, vector5.z, gauss6_5.w);
		_PSSProfileHigh_weighths6_var6 = new Vector4(vector6.x, vector6.y, vector6.z, gauss6_6.w);
		_PSSProfileMedium_weighths1_var1 = new Vector4(vector.x + vector2.x, vector.y + vector2.y, vector.z + vector2.z, (gauss6_1.w * grayscale + gauss6_2.w * grayscale2) / (grayscale + grayscale2));
		_PSSProfileMedium_weighths2_var2 = new Vector4(vector3.x + vector4.x, vector3.y + vector4.y, vector3.z + vector4.z, (gauss6_3.w * grayscale3 + gauss6_4.w * grayscale4) / (grayscale3 + grayscale4));
		_PSSProfileMedium_weighths3_var3 = new Vector4(vector5.x + vector6.x, vector5.y + vector6.y, vector5.z + vector6.z, (gauss6_5.w * grayscale5 + gauss6_6.w * grayscale6) / (grayscale5 + grayscale6));
		_PSSProfileLow_weighths1_var1 = new Vector4(vector.x + vector2.x + vector3.x, vector.y + vector2.y + vector3.y, vector.z + vector2.z + vector3.z, (gauss6_1.w * grayscale + gauss6_2.w * grayscale2 + gauss6_3.w * grayscale3) / (grayscale + grayscale2 + grayscale3));
		_PSSProfileLow_weighths2_var2 = new Vector4(vector4.x + vector5.x + vector6.x, vector4.y + vector5.y + vector6.y, vector4.z + vector5.z + vector6.z, (gauss6_4.w * grayscale4 + gauss6_5.w * grayscale5 + gauss6_6.w * grayscale6) / (grayscale4 + grayscale5 + grayscale6));
		_PSSProfileHigh_sqrtvar1234.x = Mathf.Sqrt(_PSSProfileHigh_weighths1_var1.w);
		_PSSProfileHigh_sqrtvar1234.y = Mathf.Sqrt(_PSSProfileHigh_weighths2_var2.w);
		_PSSProfileHigh_sqrtvar1234.z = Mathf.Sqrt(_PSSProfileHigh_weighths3_var3.w);
		_PSSProfileHigh_sqrtvar1234.w = Mathf.Sqrt(_PSSProfileHigh_weighths4_var4.w);
		_PSSProfileMedium_sqrtvar123.x = Mathf.Sqrt(_PSSProfileMedium_weighths1_var1.w);
		_PSSProfileMedium_sqrtvar123.y = Mathf.Sqrt(_PSSProfileMedium_weighths2_var2.w);
		_PSSProfileMedium_sqrtvar123.z = Mathf.Sqrt(_PSSProfileMedium_weighths3_var3.w);
		_PSSProfileLow_sqrtvar12.x = Mathf.Sqrt(_PSSProfileLow_weighths1_var1.w);
		_PSSProfileLow_sqrtvar12.y = Mathf.Sqrt(_PSSProfileLow_weighths2_var2.w);
		_PSSProfileHigh_transl123_sqrtvar5.w = Mathf.Sqrt(_PSSProfileHigh_weighths5_var5.w);
		_PSSProfileHigh_transl456_sqrtvar6.w = Mathf.Sqrt(_PSSProfileHigh_weighths6_var6.w);
		float num = -1.442695f;
		_PSSProfileHigh_transl123_sqrtvar5.x = num / gauss6_1.w;
		_PSSProfileHigh_transl123_sqrtvar5.y = num / gauss6_2.w;
		_PSSProfileHigh_transl123_sqrtvar5.z = num / gauss6_3.w;
		_PSSProfileHigh_transl456_sqrtvar6.x = num / gauss6_4.w;
		_PSSProfileHigh_transl456_sqrtvar6.y = num / gauss6_5.w;
		_PSSProfileHigh_transl456_sqrtvar6.z = num / gauss6_6.w;
		_PSSProfileMedium_transl123.x = num / _PSSProfileMedium_weighths1_var1.w;
		_PSSProfileMedium_transl123.y = num / _PSSProfileMedium_weighths2_var2.w;
		_PSSProfileMedium_transl123.z = num / _PSSProfileMedium_weighths3_var3.w;
		Vector3 vector7 = default(Vector3);
		vector7.x = gauss6_1.w * vector.x + gauss6_2.w * vector2.x + gauss6_3.w * vector3.x + gauss6_4.w * vector4.x + gauss6_5.w * vector5.x + gauss6_6.w * vector6.x;
		vector7.y = gauss6_1.w * vector.y + gauss6_2.w * vector2.y + gauss6_3.w * vector3.y + gauss6_4.w * vector4.y + gauss6_5.w * vector5.y + gauss6_6.w * vector6.y;
		vector7.z = gauss6_1.w * vector.z + gauss6_2.w * vector2.z + gauss6_3.w * vector3.z + gauss6_4.w * vector4.z + gauss6_5.w * vector5.z + gauss6_6.w * vector6.z;
		_PSSProfileLow_transl.x = num / vector7.x;
		_PSSProfileLow_transl.y = num / vector7.y;
		_PSSProfileLow_transl.z = num / vector7.z;
		needsRecalcDerived = false;
	}

	public void ApplyProfile(Material material)
	{
		ApplyProfile(material, noWarn: false);
	}

	public void ApplyProfile(Material material, bool noWarn)
	{
		if (needsRecalcDerived)
		{
			RecalculateDerived();
		}
		material.SetVector("_PSSProfileHigh_weighths1_var1", _PSSProfileHigh_weighths1_var1);
		material.SetVector("_PSSProfileHigh_weighths2_var2", _PSSProfileHigh_weighths2_var2);
		material.SetVector("_PSSProfileHigh_weighths3_var3", _PSSProfileHigh_weighths3_var3);
		material.SetVector("_PSSProfileHigh_weighths4_var4", _PSSProfileHigh_weighths4_var4);
		material.SetVector("_PSSProfileHigh_weighths5_var5", _PSSProfileHigh_weighths5_var5);
		material.SetVector("_PSSProfileHigh_weighths6_var6", _PSSProfileHigh_weighths6_var6);
		material.SetVector("_PSSProfileHigh_sqrtvar1234", _PSSProfileHigh_sqrtvar1234);
		material.SetVector("_PSSProfileHigh_transl123_sqrtvar5", _PSSProfileHigh_transl123_sqrtvar5);
		material.SetVector("_PSSProfileHigh_transl456_sqrtvar6", _PSSProfileHigh_transl456_sqrtvar6);
		material.SetVector("_PSSProfileMedium_weighths1_var1", _PSSProfileMedium_weighths1_var1);
		material.SetVector("_PSSProfileMedium_weighths2_var2", _PSSProfileMedium_weighths2_var2);
		material.SetVector("_PSSProfileMedium_weighths3_var3", _PSSProfileMedium_weighths3_var3);
		material.SetVector("_PSSProfileMedium_transl123", _PSSProfileMedium_transl123);
		material.SetVector("_PSSProfileMedium_sqrtvar123", _PSSProfileMedium_sqrtvar123);
		material.SetVector("_PSSProfileLow_weighths1_var1", _PSSProfileLow_weighths1_var1);
		material.SetVector("_PSSProfileLow_weighths2_var2", _PSSProfileLow_weighths2_var2);
		material.SetVector("_PSSProfileLow_sqrtvar12", _PSSProfileLow_sqrtvar12);
		material.SetVector("_PSSProfileLow_transl", _PSSProfileLow_transl);
	}
}
