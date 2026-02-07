using System;
using UnityEngine;

[Serializable]
public class VLevelDefinition : Definition
{
	[Range(0.5f, 5f)]
	public float timePerBlock;

	[Range(2f, 12f)]
	public short matrixSize;

	[Range(0f, 1f)]
	public float freeCountPer;

	[Range(2f, 6f)]
	public short groupSize;

	public bool hasDeadNodes;

	[Range(1f, 8f)]
	public short deadNodeSize;

	public int skillPointesRequired;

	[HideInInspector]
	public int SkillPointsA;

	[HideInInspector]
	public int SkillPointsB;
}
