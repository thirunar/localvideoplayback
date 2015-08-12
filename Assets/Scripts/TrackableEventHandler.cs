/*============================================================================== 
 * Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc. All Rights Reserved. 
 * ==============================================================================*/

using UnityEngine;
using Vuforia;

/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class TrackableEventHandler : MonoBehaviour,
ITrackableEventHandler
{
	#region PRIVATE_MEMBER_VARIABLES
	
	private TrackableBehaviour mTrackableBehaviour;
	private VideoPlayBackCloudRecoBehaviour video;
	
	private bool mHasBeenFound = false;
	private bool mLostTracking;
	private bool videoFinished;
	private float mSecondsSinceLost;
	private float distanceToCamera;
	
	private float mVideoCurrentPosition;
	private float mCurrentVolume;
	
	private Transform mMyModel;
	
	
	#endregion // PRIVATE_MEMBER_VARIABLES
	
	
	
	#region UNITY_MONOBEHAVIOUR_METHODS
	
	void Start()
	{
		/*for custom animations on update
Transform[] allChildren = GetComponentsInChildren<Transform>();
foreach (Transform child in allChildren) {
     // do whatever with child transform here
if (child.name == "MyModel") mMyModel = child;
}
*/
		
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour)
		{
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
		
		video = GetComponentInChildren<VideoPlayBackCloudRecoBehaviour>();
		
		OnTrackingLost();
	}
	
	
	void Update()
	{
		
		if (video == null) return;
		
		if (!mLostTracking && mHasBeenFound)
		{
			
			/*
//whatever custom animation is performed per update frame if tracker is found
if (mMyModel)
{
mMyModel.Rotate(0.0f, -0.2666f, 0.0f);
}
*/
			//if video is playing, get distance to camera.
			if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
			{
				distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.root.position);
				mCurrentVolume = 1.0f - (Mathf.Clamp01(distanceToCamera * 0.0005f) * 0.5f);
				video.VideoPlayer.SetVolume(mCurrentVolume);
				
			}
			else if (video.CurrentState == VideoPlayerHelper.MediaState.REACHED_END)
			{
				
				//Loop automatically if marker is visible and video has reached the end
				//comment this out if you want the play button to appear when the video has reached the end 
				
				Debug.Log("Video Has ended, playing again");
				video.VideoPlayer.Play(false, 0);
			}
			
			
		}
		
		
		// Pause the video if tracking is lost for more than n seconds
		if (mHasBeenFound && mLostTracking && !videoFinished)
		{
			if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
			{
				//fade out volume from current if marker is lost
				Debug.Log(mCurrentVolume - mSecondsSinceLost);
				video.VideoPlayer.SetVolume(Mathf.Clamp01(mCurrentVolume - mSecondsSinceLost));
			}
			
			//n.0f is number of seconds before playback stops when marker is lost
			if (mSecondsSinceLost > 1.0f)
			{
				PauseAndUnloadVideo();
			}
			
			mSecondsSinceLost += Time.deltaTime;
		}
	}
	
	public void PauseAndUnloadVideo()
	{
		if (video.CurrentState == VideoPlayerHelper.MediaState.PLAYING)
		{
			//get last position so it can resume after video is unloaded and reloaded.
			mVideoCurrentPosition = video.VideoPlayer.GetCurrentPosition();
			video.VideoPlayer.Pause();
			
			if (video.VideoPlayer.Unload())
			{
				Debug.Log("UnLoaded Video: " + video.m_path);
				videoFinished = true;
			}
			
		}
	}
	#endregion // UNITY_MONOBEHAVIOUR_METHODS
	
	
	
	#region PUBLIC_METHODS
	
	// Implementation of the ITrackableEventHandler function called when the
	// tracking state changes.
	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		Debug.Log ("On trackable state changed");
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
		    newStatus == TrackableBehaviour.Status.TRACKED || 
		    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}
	
	#endregion // PUBLIC_METHODS
	
	
	
	#region PRIVATE_METHODS
	
	
	private void OnTrackingFound()
	{
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
		if (video != null) {
			Debug.Log("Video is null. So initialising in Tracking found");
			video = GetComponentInChildren<VideoPlayBackCloudRecoBehaviour>();
		}
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>();
		AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();
		
		// Enable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = true;
		}
		
		// Enable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = true;
		}
		//Play audio:
		foreach (AudioSource component in audioComponents)
		{
			component.Play();
			
		}
		
		
		
		// Optionally play the video automatically when the target is found
		//			video.InitializeVideoPlayback ();
		if (video != null)
		{
			Debug.Log("Video not null");
			string videoPath= "";
			video = GetComponentInChildren<VideoPlayBackCloudRecoBehaviour>();
			if(mTrackableBehaviour.TrackableName == "download")
				videoPath = "KFC.mp4";
			else if(mTrackableBehaviour.TrackableName == "hb")
				videoPath = "HBR.mp4";
			else if(mTrackableBehaviour.TrackableName == "model")
				videoPath = "model.mp4";
			else if(mTrackableBehaviour.TrackableName == "sg50")
				videoPath = "sg50.mp4";
			else if(mTrackableBehaviour.TrackableName == "wedding")
				videoPath = "wedding.mp4";
			video.m_path= videoPath;
			video.VideoPlayer.SetFilename(videoPath);
			
			if (video.VideoPlayer.Load(video.m_path, VideoPlayerHelper.MediaType.ON_TEXTURE, true, mVideoCurrentPosition))
			{
				Debug.Log("Loaded Video: " + video.m_path + " Video Texture Id: " + video.mVideoTexture.GetNativeTextureID());
			}
			
			if (video.VideoPlayer.IsPlayableOnTexture())
			{
				VideoPlayerHelper.MediaState state = video.VideoPlayer.GetStatus();
				if (state == VideoPlayerHelper.MediaState.PAUSED ||
				    state == VideoPlayerHelper.MediaState.READY ||
				    state == VideoPlayerHelper.MediaState.STOPPED)
				{
					Debug.Log("Video File: " + video.m_path);
					video.VideoPlayer.Play(false, video.VideoPlayer.GetCurrentPosition());
					
				}
				else if (state == VideoPlayerHelper.MediaState.REACHED_END)
				{
					// Play this video from the beginning
					video.VideoPlayer.Play(false, 0);
				}
			}
		}
		
		mHasBeenFound = true;
		mLostTracking = false;
		
	}
	
	
	public void OnTrackingLost()
	{
		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>();
		Collider[] colliderComponents = GetComponentsInChildren<Collider>();
		AudioSource[] audioComponents = GetComponentsInChildren<AudioSource>();
		
		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}
		
		// Disable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = false;
		}
		
		//Pause Audio:
		foreach (AudioSource component in audioComponents)
		{
			component.Pause();
		}
		
		//			Destroy (GetComponentsInChildren<VideoPlayBackCloudRecoBehaviour>());
		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
		
		mLostTracking = true;
		mSecondsSinceLost = 0;
		videoFinished = false;
	}
	
	
	#endregion // PRIVATE_METHODS
}
