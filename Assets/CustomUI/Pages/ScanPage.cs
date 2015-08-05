using UnityEngine;
using System.Collections;
using MaterialUI;

namespace CustomUI
{
    public class ScanPage : BasePage
    {
        public GameObject camera;
        public GameObject background;

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            background.SetActive(false);
            camera.SetActive(true);
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            camera.SetActive(false);
            background.SetActive(true);
        }
    }
}