using UnityEngine;
using MaterialUI;
using System.Collections;

namespace CustomUI
{
    public class BasePage : ScreenConfig
    {
        public MenuItem associatedMenu;

        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        public virtual void OnNavigatedFrom(NavigationEventArgs e)
        {

        }
    }
}