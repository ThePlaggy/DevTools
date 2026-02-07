using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TitleHelpMeHook : MonoBehaviour
{
	[SerializeField]
	private RawImage helpMeRawImage;

	[SerializeField]
	private RenderTexture helpMeRenderTexture;

	private VideoPlayer myVideoPlayer;

	private float videoDelay;

	private float videoPreparedTimeStamp;

	private bool videoReadyToPlay;

	private void Awake()
	{
		myVideoPlayer = GetComponent<VideoPlayer>();
		myVideoPlayer.renderMode = VideoRenderMode.RenderTexture;
		myVideoPlayer.targetTexture = helpMeRenderTexture;
		myVideoPlayer.playOnAwake = false;
		myVideoPlayer.waitForFirstFrame = false;
		helpMeRawImage.enabled = false;
		helpMeRawImage.texture = helpMeRenderTexture;
		myVideoPlayer.prepareCompleted += videoIsPrepared;
		myVideoPlayer.loopPointReached += videoDonePlaying;
		myVideoPlayer.Prepare();
	}

	private void Update()
	{
		if (videoReadyToPlay && Time.time - videoPreparedTimeStamp >= videoDelay)
		{
			videoReadyToPlay = false;
			helpMeRawImage.enabled = true;
			myVideoPlayer.Play();
		}
	}

	private void videoIsPrepared(VideoPlayer VP)
	{
		myVideoPlayer.prepareCompleted -= videoIsPrepared;
		videoPreparedTimeStamp = Time.time;
		videoDelay = 2f;
		videoReadyToPlay = true;
	}

	private void videoDonePlaying(VideoPlayer VP)
	{
		myVideoPlayer.loopPointReached -= videoDonePlaying;
		helpMeRawImage.enabled = false;
		myVideoPlayer.enabled = false;
	}
}
