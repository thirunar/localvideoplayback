using UnityEngine;
using MaterialUI;
using System.Collections;

namespace CustomUI
{
    public class BasePage : ScreenConfig
    {
        public MenuItem associatedMenu;

        /// <summary>
        /// This function is called when a page is about to be displayed
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnNavigatingTo(NavigationEventArgs e)
        {
            Debug.Log("Before Navigation");
        }

        /// <summary>
        /// This function is called after displaying the page.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.Log("After Navigation");
        }

        /// <summary>
        /// This function is called when hardware back key is pressed
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnBackKeyPress(CancellationEventArgs e)
        {
            Debug.Log("Back key press");
        }

        /// <summary>
        /// This function is called when menu key is pressed
        /// </summary>
        public virtual void OnMenuKeyPress()
        {
            Debug.Log("Menu key press");
        }

        /// <summary>
        /// This function is called before leaving the page
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnNavigatingFrom(NavigationEventArgs e)
        {
            Debug.Log("Before leaving");
        }

        /// <summary>
        /// This function is called when navigated away from the page
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.Log("After leaving");
        }
    }
}