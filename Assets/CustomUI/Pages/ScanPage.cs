using UnityEngine;
using System.Collections;
using MaterialUI;
using Vuforia;

namespace CustomUI
{
    public class ScanPage : BasePage
    {
        public GameObject arCamera;
        public GameObject background;
        public GameObject flashButton;

        private bool flashEnabled = false;

        public override void OnNavigatingTo(NavigationEventArgs e)
        {
            arCamera.SetActive(true);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            background.SetActive(false);
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            arCamera.SetActive(false);
            background.SetActive(true);
        }

        public void ToggleFlash()
        {
            if (CameraDevice.Instance.SetFlashTorchMode(!flashEnabled))
                flashEnabled = !flashEnabled;
            flashButton.GetComponent<ImageToggle>().SetSprite(!flashEnabled);
        }
    }
}