using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemDeFragHack : MonoBehaviour
{
	public int MEMORY_CELL_START_POOL_COUNT;

	public GraphicRaycaster MyRayCaster;

	public GameObject MemoryDefragContentHolder;

	public GameObject MemoryCellObjectHolder;

	public GameObject MemoryCellObject;

	public GameObject MemoryDefragObject;

	public AudioFileDefinition MemCellPresentSFX;

	public List<MemDefragLevelDefinition> MemDefragLevels;
}
