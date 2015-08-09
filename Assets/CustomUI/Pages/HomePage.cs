using UnityEngine;
using System.Collections;
using MaterialUI;
using System;

namespace CustomUI
{
    public class HomePage : BasePage
    {
        public GameObject menuBar;
        private DateTime firstBackKeyPress;

        void Start()
        {
            firstBackKeyPress = DateTime.Today;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            menuBar.SetActive(true);
        }

        public override void OnNavigatingFrom(NavigationEventArgs e)
        {
            menuBar.SetActive(false);
        }

        public override void OnBackKeyPress(CancellationEventArgs e)
        {
            e.CancelEvent = true;
            if (firstBackKeyPress.AddSeconds(2) > DateTime.Now)
            {
                Application.Quit();
            }
            else
            {
                firstBackKeyPress = DateTime.Now;
                PagesManager.DisplayToast("Press Back again to exit");
            }
        }
    }
}