using UnityEngine;
using System.Collections;
using MaterialUI;
using Vuforia;
using AssemblyCSharp;

namespace CustomUI
{
    public class ScanPage : BasePage
    {
        public GameObject arCamera;
        public GameObject background;
        public GameObject cloudRecognition;
        public GameObject imageTarget;

        public GameObject flashButton;
        public GameObject volumeButton;
        public GameObject shareButton;
        public GameObject linkButton;
        public GameObject captureButton;

        private TrackableCloudRecoEventHandler trackableCloudRecoEventHandler;
        private bool flashEnabled = false;
        private bool isPageActive = false;
        private bool isAudioMuted = false;

        public override void Start()
        {
            base.Start();
            try
            {
                arCamera.SetActive(true);
                Invoke("DisableArCamera", 0.5f);
                trackableCloudRecoEventHandler = imageTarget.GetComponent<TrackableCloudRecoEventHandler>();
                trackableCloudRecoEventHandler.OnTrackingLostHandler += trackableCloudRecoEventHandler_OnTrackingLostHandler;
                trackableCloudRecoEventHandler.OnTrackingFoundHandler += trackableCloudRecoEventHandler_OnTrackingFoundHandler;
                trackableCloudRecoEventHandler.OnVideoPlayHandler += trackableCloudRecoEventHandler_OnVideoPlayHandler;
                trackableCloudRecoEventHandler.OnVideoUnloadHandler += trackableCloudRecoEventHandler_OnVideoUnloadHandler;
            }
            catch { }
        }

        private void trackableCloudRecoEventHandler_OnTrackingFoundHandler(object sender, System.EventArgs e)
        {
            volumeButton.SetActive(true);
            shareButton.SetActive(true);
            linkButton.SetActive(true);
        }

        private void trackableCloudRecoEventHandler_OnTrackingLostHandler(object sender, System.EventArgs e)
        {
        }

        private void trackableCloudRecoEventHandler_OnVideoPlayHandler(object sender, System.EventArgs e)
        {
            captureButton.SetActive(true);
        }

        private void trackableCloudRecoEventHandler_OnVideoUnloadHandler(object sender, System.EventArgs e)
        {
            captureButton.SetActive(false);
        }

        public override void OnNavigatingTo(NavigationEventArgs e)
        {

        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            isPageActive = true;
            Invoke("DelayedLoad", 0.5f);
        }

        private void DisableArCamera()
        {
            arCamera.SetActive(false);
        }

        private void DelayedLoad()
        {
            if (isPageActive)
            {
                arCamera.SetActive(true);
                cloudRecognition.SetActive(true);
                imageTarget.SetActive(true);
                background.SetActive(false);
            }
        }

        public override void OnNavigatingFrom(NavigationEventArgs e)
        {
            isPageActive = false;
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            arCamera.SetActive(false);
            trackableCloudRecoEventHandler.PauseAndUnloadVideo();
            trackableCloudRecoEventHandler.OnTrackingLost();
            background.SetActive(true);
            cloudRecognition.SetActive(false);
            imageTarget.SetActive(false);
            SetFlash(false);
            volumeButton.SetActive(false);
            shareButton.SetActive(false);
            linkButton.SetActive(false);
        }

        public void ToggleVolume()
        {
            if (trackableCloudRecoEventHandler.SetVolume(isAudioMuted))
            {
                isAudioMuted = !isAudioMuted;
                volumeButton.GetComponent<ImageToggle>().SetSprite(!isAudioMuted);
            }
        }

        public void ToggleFlash()
        {
            SetFlash(!flashEnabled);
        }

        private void SetFlash(bool on)
        {
            if (flashEnabled == on) return;
            if (!CameraDevice.Instance.SetFlashTorchMode(on)) on = false;
            flashEnabled = on;
            flashButton.GetComponent<ImageToggle>().SetSprite(!on);
        }
    }
}