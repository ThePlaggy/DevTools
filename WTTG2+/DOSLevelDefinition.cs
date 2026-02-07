using System;
using UnityEngine;

[Serializable]
public class DOSLevelDefinition : Definition
{
	[Range(3f, 12f)]
	public short matrixSize;

	[Range(2f, 8f)]
	public short actionBlockSize;

	[Range(0.25f, 5.5f)]
	public float gameTimeModifier;

	[Range(0.15f, 1f)]
	public float hotTime;

	public bool trollNodesActive;

	public int skillPointesRequired;

	[HideInInspector]
	public int SkillPointsA;

	[HideInInspector]
	public int SkillPointsB;
}
