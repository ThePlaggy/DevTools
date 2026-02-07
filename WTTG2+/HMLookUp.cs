using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HMLookUp : MonoBehaviour
{
	public HMBTNObject nodeHexerStart;

	public HMBTNObject stackPusherStart;

	public HMBTNObject cloudGridStart;

	public HMBTNObject dosBlockerStart;

	public HMBTNObject nodeHexerLeaderboards;

	public HMBTNObject stackPusherLeaderboards;

	public HMBTNObject cloudGridLeaderboards;

	public HMBTNObject dosBlockerLeaderboards;

	public HMBTNObject nodeHexerCustom;

	public HMBTNObject stackPusherCustom;

	public HMBTNObject cloudGridCustom;

	public HMBTNObject dosBlockerCustom;

	public HMBTNObject quitGame;

	public HMBTNObject quitToMenu;

	public HMBTNOptions musicButton;

	public GameObject chainModeHolder;

	public GameObject quitHolder;

	public HMBTNObject closeLeaderboards;

	public GameObject bannedHolder;

	public TMP_Text bannedDur;

	public HMBTNOptions sfxButton;

	public TMP_Text nodeHexerTop;

	public TMP_Text stackPusherTop;

	public TMP_Text cloudGridTop;

	public TMP_Text dosBlockerTop;

	public HMCustomHack customHack;

	public HMBTNOptions glitchButton;

	public HMBTNObject submitScoresButton;

	public Text submitScoresButtonText;

	public HMMusicSlider musicSlider;

	public HMUsernameInputField usernameInputField;

	public HMBTNObject presetsBTN;

	public GameObject presetsHolder;

	public GameObject level11button;

	public GameObject level12button;

	public void PresetSelected(int level)
	{
		switch (HackerModeManager.CurrentCustomHack)
		{
		case CUSTOM_HACK.DOS_BLOCKER:
		{
			DOSLevelDefinition dOSLevelDefinition = GameManager.HackerManager.myDosAttack.DOSLevels[level - 1];
			customHack.dosBlocker.matrixSizeSlider.value = dOSLevelDefinition.matrixSize;
			customHack.dosBlocker.actionBlockSizeSlider.value = dOSLevelDefinition.actionBlockSize;
			customHack.dosBlocker.gameTimeModifierSlider.value = dOSLevelDefinition.gameTimeModifier;
			customHack.dosBlocker.hotTimeSlider.value = dOSLevelDefinition.hotTime;
			customHack.dosBlocker.trollNodesActiveToggle.isOn = dOSLevelDefinition.trollNodesActive;
			customHack.dosBlocker.matrixSizeChanged();
			customHack.dosBlocker.actionBlockSizeChanged();
			customHack.dosBlocker.gameTimeModifierChanged();
			customHack.dosBlocker.hotTimeChanged();
			customHack.dosBlocker.trollNodesActiveChanged(dOSLevelDefinition.trollNodesActive);
			break;
		}
		case CUSTOM_HACK.VAPE_ATTACK:
		{
			VLevelDefinition vLevelDefinition = GameManager.HackerManager.myVapeAttack.VapeLevels[level - 1];
			customHack.vapeAttack.matrixSizeSlider.value = vLevelDefinition.matrixSize;
			customHack.vapeAttack.timePerBlockSlider.value = vLevelDefinition.timePerBlock;
			customHack.vapeAttack.freeCountPerSlider.value = vLevelDefinition.freeCountPer;
			customHack.vapeAttack.groupSizeSlider.value = vLevelDefinition.groupSize;
			customHack.vapeAttack.deadNoteSizeSlider.value = (vLevelDefinition.hasDeadNodes ? ((float)vLevelDefinition.deadNodeSize) : 0f);
			customHack.vapeAttack.matrixSizeChanged();
			customHack.vapeAttack.timePerBlockChanged();
			customHack.vapeAttack.freeCountPerChanged();
			customHack.vapeAttack.groupSizeChanged();
			customHack.vapeAttack.deadNoteChanged();
			break;
		}
		case CUSTOM_HACK.STACK_PUSHER:
		{
			StackPusherLevelDefinition stackPusherLevelDefinition = GameManager.HackerManager.StackPusherLevelRef[level - 1];
			customHack.stackPusher.matrixSizeSlider.value = stackPusherLevelDefinition.MatrixSize;
			customHack.stackPusher.stackPiecesSlider.value = stackPusherLevelDefinition.StackPeices;
			customHack.stackPusher.deadPiecesSlider.value = stackPusherLevelDefinition.DeadPeices;
			customHack.stackPusher.timePerPieceSlider.value = stackPusherLevelDefinition.TimePerPeice;
			customHack.stackPusher.matrixSizeChanged();
			customHack.stackPusher.stackPiecesChanged();
			customHack.stackPusher.deadPiecesChanged();
			customHack.stackPusher.timePerPieceChanged();
			break;
		}
		case CUSTOM_HACK.NODE_HEXER:
		{
			NodeHexerLevelDefinition nodeHexerLevelDefinition = GameManager.HackerManager.NodeHexerLevelRef[level - 1];
			customHack.nodeHexer.matrixSizeSlider.value = nodeHexerLevelDefinition.MatrixSize;
			customHack.nodeHexer.tagPiecesSlider.value = nodeHexerLevelDefinition.TagPieces;
			customHack.nodeHexer.timeBoostSlider.value = nodeHexerLevelDefinition.TimeBoost;
			customHack.nodeHexer.startTimeSlider.value = nodeHexerLevelDefinition.StartTime;
			customHack.nodeHexer.MatrixSizeChanged();
			customHack.nodeHexer.TagPiecesChanged();
			customHack.nodeHexer.TimeBoostChanged();
			customHack.nodeHexer.StartTimeChanged();
			break;
		}
		}
	}
}
