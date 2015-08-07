/*==============================================================================
            Copyright (c) 2012 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
 
This  Vuforia(TM) sample application in source code form ("Sample Code") for the
Vuforia Software Development Kit and/or Vuforia Extension for Unity
(collectively, the "Vuforia SDK") may in all cases only be used in conjunction
with use of the Vuforia SDK, and is subject in all respects to all of the terms
and conditions of the Vuforia SDK License Agreement, which may be found at
<a href="https://ar.qualcomm.at/legal/license" title="https://ar.qualcomm.at/legal/license">https://ar.qualcomm.at/legal/license</a>.
 
By retaining or using the Sample Code in any manner, you confirm your agreement
to all the terms and conditions of the Vuforia SDK License Agreement.  If you do
not agree to all the terms and conditions of the Vuforia SDK License Agreement,
then you may not retain or use any of the Sample Code in any manner.
==============================================================================*/

using UnityEngine;
using System.Collections;
using Vuforia;

public class VideoPlayBackCloudRecoBehaviour : MonoBehaviour
{
    #region PUBLIC_MEMBER_VARIABLES

    // URL of the video, either a path to a local file or a remote address
    public string m_path = null;

    // Texture for the play icon
    public Texture m_playTexture = null;

    // Texture for the busy icon
    public Texture m_busyTexture = null;

    // Texture for the error icon
    public Texture m_errorTexture = null;

    #endregion // PUBLIC_MEMBER_VARIABLES



    #region PRIVATE_MEMBER_VARIABLES

    private VideoPlayerHelper mVideoPlayer = null;
    private bool mIsInited = false;
    private bool mIsPrepared = false;

    public Texture2D mVideoTexture = null;

    [SerializeField]
    [HideInInspector]
    private Texture mKeyframeTexture = null;

    private VideoPlayerHelper.MediaType mMediaType =
        VideoPlayerHelper.MediaType.ON_TEXTURE_FULLSCREEN;

    private VideoPlayerHelper.MediaState mCurrentState =
        VideoPlayerHelper.MediaState.NOT_READY;

    private float mSeekPosition = 0.0f;

    private bool isPlayableOnTexture;

    private GameObject mIconPlane = null;
    private bool mIconPlaneActive = false;

    #endregion // PRIVATE_MEMBER_VARIABLES



    #region PROPERTIES

    // Returns the video player
    public VideoPlayerHelper VideoPlayer
    {
        get { return mVideoPlayer; }
    }

    // Returns the current playback state
    public VideoPlayerHelper.MediaState CurrentState
    {
        get { return mCurrentState; }
    }

    // Type of playback (on-texture only, fullscreen only, or both)
    public VideoPlayerHelper.MediaType MediaType
    {
        get { return mMediaType; }
        set { mMediaType = value; }
    }

    // Texture displayed before video playback begins
    public Texture KeyframeTexture
    {
        get { return mKeyframeTexture; }
        set { mKeyframeTexture = value; }
    }

    #endregion // PROPERTIES



    #region UNITY_MONOBEHAVIOUR_METHODS

    void Start()
    {
        // A filename or url must be set in the inspector
        if (m_path == null || m_path.Length == 0)
        {
            Debug.Log("Please set a video url in the Inspector");
            this.enabled = false;
        }


        // Create the video player and set the filename
        mVideoPlayer = new VideoPlayerHelper();
        mVideoPlayer.SetFilename(m_path);

        // Find the icon plane (child of this object)
        mIconPlane = transform.Find("Icon").gameObject;

        // Set the current state to Not Ready
        HandleStateChange(VideoPlayerHelper.MediaState.NOT_READY);
        mCurrentState = VideoPlayerHelper.MediaState.NOT_READY;

        // Flip the plane as the video texture is mirrored on the horizontal
        transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x),
                                           transform.localScale.y, transform.localScale.z);

        // Scale the icon
        ScaleIcon();
    }


    void Update()
    {
        if (!mIsInited)
        {
            // Initialize the video player
            if (mVideoPlayer.Init() == false)
            {
                Debug.Log("Could not initialize video player");
                HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
                this.enabled = false;
                return;
            }

            // Initialize the video texture
            InitVideoTexture();

            // Load the video
            /*
            if (mVideoPlayer.Load(m_path, mMediaType, false, 0) == false)
            {
                Debug.Log("ERR1: Could not load video '" + m_path + "' for media type " + mMediaType);
                HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
                this.enabled = false;
                return;
            }
            */

            // Successfully initialized
            mIsInited = true;
        }
        else if (!mIsPrepared)
        {

            // Get the video player status
            VideoPlayerHelper.MediaState state = mVideoPlayer.GetStatus();

            if (state == VideoPlayerHelper.MediaState.ERROR)
            {
                Debug.Log("ERR2: Could not load video '" + m_path + "' for media type " + mMediaType);
                HandleStateChange(VideoPlayerHelper.MediaState.ERROR);
                this.enabled = false;
            }
            else if (state < VideoPlayerHelper.MediaState.NOT_READY)
            {
                // Video player is ready

                // Can we play this video on a texture?
                isPlayableOnTexture = mVideoPlayer.IsPlayableOnTexture();

                if (isPlayableOnTexture)
                {


                    // Pass the video texture id to the video player
                    Debug.Log("Setting Texture width: " + mVideoTexture.width + " height: " + mVideoTexture.height);
                    int nativeTextureID = mVideoTexture.GetNativeTextureID();

                    mVideoPlayer.SetVideoTextureID(nativeTextureID);

                    // Get the video width and height
                    int videoWidth = mVideoPlayer.GetVideoWidth();
                    int videoHeight = mVideoPlayer.GetVideoHeight();


                    //					TextureScaler.scale(mVideoTexture, videoWidth, videoHeight, FilterMode.Bilinear);
                    if (videoWidth > 0 && videoHeight > 0)
                    {
                        //						 Scale the video plane to match the video aspect ratio
                        float aspect = videoHeight / (float)videoWidth;

                        //						 Flip the plane as the video texture is mirrored on the horizontal
                        transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f * aspect);
                    }

                    // Seek ahead if necessary
                    if (mSeekPosition > 0)
                    {
                        mVideoPlayer.SeekTo(mSeekPosition);
                    }
                }
                else
                {
                    // Handle the state change
                    state = mVideoPlayer.GetStatus();
                    HandleStateChange(state);
                    mCurrentState = state;
                }

                // Scale the icon
                ScaleIcon();

                // Video is prepared, ready for playback
                mIsPrepared = true;
            }
        }
        else
        {
            if (isPlayableOnTexture)
            {
                // Update the video texture with the latest video frame
                VideoPlayerHelper.MediaState state = mVideoPlayer.UpdateVideoData();

                // Check for playback state change
                if (state != mCurrentState)
                {
                    HandleStateChange(state);
                    mCurrentState = state;
                }
            }
            else
            {
                // Get the current status
                VideoPlayerHelper.MediaState state = mVideoPlayer.GetStatus();

                // Check for playback state change
                if (state != mCurrentState)
                {
                    HandleStateChange(state);
                    mCurrentState = state;
                }
            }
        }

        CheckIconPlaneVisibility();
    }


    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            // Handle pause event natively
            mVideoPlayer.OnPause();

            // Store the playback position for later
            mSeekPosition = mVideoPlayer.GetCurrentPosition();

            // Deinit the video
            mVideoPlayer.Deinit();

            // Reset initialization parameters
            mIsInited = false;
            mIsPrepared = false;

            // Set the current state to Not Ready
            HandleStateChange(VideoPlayerHelper.MediaState.NOT_READY);
            mCurrentState = VideoPlayerHelper.MediaState.NOT_READY;
        }
    }


    void OnDestroy()
    {
        // Deinit the video
        mVideoPlayer.Deinit();
    }

    #endregion // UNITY_MONOBEHAVIOUR_METHODS



    #region PUBLIC_METHODS

    public void ShowBusyIcon()
    {
        mIconPlane.GetComponent<Renderer>().material.mainTexture = m_busyTexture;
    }

    #endregion // PUBLIC_METHODS



    #region PRIVATE_METHODS

    // Initialize the video texture
    private void InitVideoTexture()
    {
        // Create texture of size 0 that will be updated in the plugin (we allocate buffers in native code)
        mVideoTexture = new Texture2D(0, 0, TextureFormat.RGB565, false);
        mVideoTexture.filterMode = FilterMode.Bilinear;
        mVideoTexture.wrapMode = TextureWrapMode.Clamp;
    }


    // Handle video playback state changes
    private void HandleStateChange(VideoPlayerHelper.MediaState newState)
    {
        Renderer renderer = GetComponent<Renderer>();
        // If the movie is playing or paused render the video texture
        // Otherwise render the keyframe
        if (newState == VideoPlayerHelper.MediaState.PLAYING ||
            newState == VideoPlayerHelper.MediaState.PAUSED)
        {

            renderer.material.mainTexture = mVideoTexture;

            renderer.material.mainTextureScale = new Vector2(1, 1);
        }
        else
        {
            if (mKeyframeTexture != null)
            {
                renderer.material.mainTexture = mKeyframeTexture;
                renderer.material.mainTextureScale = new Vector2(1, -1);
            }
        }

        // Display the appropriate icon, or disable if not needed
        switch (newState)
        {
            case VideoPlayerHelper.MediaState.READY:
            case VideoPlayerHelper.MediaState.REACHED_END:
            case VideoPlayerHelper.MediaState.PAUSED:
            case VideoPlayerHelper.MediaState.STOPPED:
                mIconPlane.GetComponent<Renderer>().material.mainTexture = m_playTexture;
                mIconPlaneActive = true;
                break;

            case VideoPlayerHelper.MediaState.NOT_READY:
            case VideoPlayerHelper.MediaState.PLAYING_FULLSCREEN:
                mIconPlane.GetComponent<Renderer>().material.mainTexture = m_busyTexture;
                mIconPlaneActive = true;
                break;

            case VideoPlayerHelper.MediaState.ERROR:
                mIconPlane.GetComponent<Renderer>().material.mainTexture = m_errorTexture;
                mIconPlaneActive = true;
                break;

            default:
                mIconPlaneActive = false;
                break;
        }

        if (newState == VideoPlayerHelper.MediaState.PLAYING_FULLSCREEN)
        {
            // Switching to full screen, disable QCARBehaviour (only applicable for iOS)
            QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
            qcarBehaviour.enabled = false;
        }
        else if (mCurrentState == VideoPlayerHelper.MediaState.PLAYING_FULLSCREEN)
        {
            // Switching away from full screen, enable QCARBehaviour (only applicable for iOS)
            QCARBehaviour qcarBehaviour = (QCARBehaviour)FindObjectOfType(typeof(QCARBehaviour));
            qcarBehaviour.enabled = true;
        }
    }


    private void ScaleIcon()
    {
        // Icon should fill 50% of the narrowest side of the video

        float videoWidth = Mathf.Abs(transform.localScale.x);
        float videoHeight = Mathf.Abs(transform.localScale.z);
        float iconWidth, iconHeight;

        if (videoWidth > videoHeight)
        {
            iconWidth = 0.5f * videoHeight / videoWidth;
            iconHeight = 0.5f;
        }
        else
        {
            iconWidth = 0.5f;
            iconHeight = 0.5f * videoWidth / videoHeight;
        }

        mIconPlane.transform.localScale = new Vector3(-iconWidth, 1.0f, iconHeight);
    }


    private void CheckIconPlaneVisibility()
    {
        Renderer renderer = GetComponent<Renderer>();
        // If the video object renderer is currently enabled, we might need to toggle the icon plane visibility
        if (renderer.enabled)
        {
            // Check if the icon plane renderer has to be disabled explicitly in case it was enabled by another script (e.g. TrackableEventHandler)
            if (mIconPlane.GetComponent<Renderer>().enabled != mIconPlaneActive)
                mIconPlane.GetComponent<Renderer>().enabled = mIconPlaneActive;
        }
    }

    #endregion // PRIVATE_METHODS
}