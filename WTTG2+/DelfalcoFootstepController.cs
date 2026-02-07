using UnityEngine;
using UnityEngine.SceneManagement;

public class DelfalcoFootstepController : MonoBehaviour
{
	public AudioHubObject myAho;

	private RaycastHit hitInfo;

	private float stepCycle;

	private float nextStep;

	public bool running;

	public AudioFileDefinition[] WalkFootStepSFXS;

	public AudioFileDefinition[] RunFootStepSFXS;

	public bool dismissFootsteps;

	private void Awake()
	{
		if (!(SceneManager.GetActiveScene().name.ToLower() == "titlescreen"))
		{
			myAho = base.gameObject.AddComponent<AudioHubObject>();
			WalkFootStepSFXS = roamController.Ins.WalkFootStepSFXS;
			RunFootStepSFXS = roamController.Ins.RunFootStepSFXS;
		}
	}

	public void PlayFootstep()
	{
		if (dismissFootsteps)
		{
			return;
		}
		Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, 2f, roamController.Ins.hitLayers);
		if (hitInfo.collider != null)
		{
			SurfaceTypeObject component = hitInfo.collider.GetComponent<SurfaceTypeObject>();
			if (component != null && component.HasCustomFootStepSFXS)
			{
				playSurfaceFootStepAudio(component);
			}
			else
			{
				playFootStepAudio();
			}
		}
	}

	private void playFootStepAudio()
	{
		if (running)
		{
			int num = Random.Range(1, RunFootStepSFXS.Length);
			myAho.PlaySoundWithWildPitch(RunFootStepSFXS[num], 0.85f, 1.1f);
			AudioFileDefinition audioFileDefinition = RunFootStepSFXS[num];
			RunFootStepSFXS[num] = RunFootStepSFXS[0];
			RunFootStepSFXS[0] = audioFileDefinition;
		}
		else
		{
			int num2 = Random.Range(1, WalkFootStepSFXS.Length);
			myAho.PlaySoundWithWildPitch(WalkFootStepSFXS[num2], 0.85f, 1.1f);
			AudioFileDefinition audioFileDefinition2 = WalkFootStepSFXS[num2];
			WalkFootStepSFXS[num2] = WalkFootStepSFXS[0];
			WalkFootStepSFXS[0] = audioFileDefinition2;
		}
	}

	private void playSurfaceFootStepAudio(SurfaceTypeObject STO)
	{
		if (running)
		{
			int index = Random.Range(1, STO.MyFootStepSFXS.RunFootStepSFXS.Count);
			myAho.PlaySoundWithWildPitch(STO.MyFootStepSFXS.RunFootStepSFXS[index], 0.85f, 1.1f);
			AudioFileDefinition value = STO.MyFootStepSFXS.RunFootStepSFXS[index];
			STO.MyFootStepSFXS.RunFootStepSFXS[index] = STO.MyFootStepSFXS.RunFootStepSFXS[0];
			STO.MyFootStepSFXS.RunFootStepSFXS[0] = value;
		}
		else
		{
			int index2 = Random.Range(1, STO.MyFootStepSFXS.WalkFootStepSFXS.Count);
			myAho.PlaySoundWithWildPitch(STO.MyFootStepSFXS.WalkFootStepSFXS[index2], 0.85f, 1.1f);
			AudioFileDefinition value2 = STO.MyFootStepSFXS.WalkFootStepSFXS[index2];
			STO.MyFootStepSFXS.WalkFootStepSFXS[index2] = STO.MyFootStepSFXS.WalkFootStepSFXS[0];
			STO.MyFootStepSFXS.WalkFootStepSFXS[0] = value2;
		}
	}
}
