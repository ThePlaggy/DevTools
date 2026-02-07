using System;
using UnityEngine;

[Serializable]
public class DOSChainDefinition : Definition
{
	[Range(3f, 12f)]
	public short matrixSize;

	[Range(2f, 8f)]
	public short actionBlockSize;

	[Range(0.5f, 6f)]
	public float warmUpTime;

	[Range(0.25f, 5.5f)]
	public float gameTimeModifier;

	[Range(0.15f, 1f)]
	public float hotTime;

	public bool trollNodesActive;

	public int pointsRequired;

	public int completeBonusPoints;

	public int timeBonusPoints;

	public int skillBonusPoints;

	[Range(1f, 6f)]
	public short bonusMultiplier;

	[Range(0f, 1f)]
	public float timeBonusRange;

	public short maxCompleteNodeTurn;
}
