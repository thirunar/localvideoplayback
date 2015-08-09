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
        public GameObject flashButton;
        public GameObject cloudRecognition;
        public GameObject imageTarget;

        private bool flashEnabled = false;
        private bool isPageActive = false;

        public override void OnNavigatingTo(NavigationEventArgs e)
        {

        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            isPageActive = true;
            Invoke("DelayedLoad", 0.5f);
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
            imageTarget.GetComponent<TrackableCloudRecoEventHandler>().PauseAndUnloadVideo();
            imageTarget.GetComponent<TrackableCloudRecoEventHandler>().OnTrackingLost();
            background.SetActive(true);
            cloudRecognition.SetActive(false);
            imageTarget.SetActive(false);
        }

        public void ToggleFlash()
        {
            if (CameraDevice.Instance.SetFlashTorchMode(!flashEnabled))
                flashEnabled = !flashEnabled;
            flashButton.GetComponent<ImageToggle>().SetSprite(!flashEnabled);
        }
    }
}