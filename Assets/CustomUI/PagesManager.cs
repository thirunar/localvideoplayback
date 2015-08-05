using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;

namespace CustomUI
{
    public class PagesManager : ScreenManager
    {
        public BasePage[] pages;
        public BasePage homePage;
        [HideInInspector]
        public BasePage currentPage;
        [HideInInspector]
        public Stack<BasePage> lastPages;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            if (pages == null)
                throw new NullReferenceException("Pages not added");
            if (lastPages == null)
                lastPages = new Stack<BasePage>();
            if (homePage == null)
                throw new NullReferenceException("Home page not set");
            if (currentPage == null)
                SetCurrentPage(homePage, NavigationType.New);
        }

        /// <summary>
        /// Set the current page
        /// </summary>
        /// <param name="page"></param>
        private void SetCurrentPage(BasePage page, NavigationType navigationType)
        {
            if (currentPage != null && currentPage.associatedMenu != null)
                currentPage.associatedMenu.UnsetActive();
            currentPage = page;
            var e = new NavigationEventArgs()
            {
                navigationType = navigationType
            };
            currentPage.OnNavigatedTo(e);
            if (currentPage == homePage) ClearLastPages();
            if (currentPage.associatedMenu != null)
                currentPage.associatedMenu.SetActive();
        }

        /// <summary>
        /// Get the last page
        /// </summary>
        /// <returns></returns>
        private BasePage GetLastPage()
        {
            if (lastPages.Count > 0)
            {
                return lastPages.Pop();
            }
            return homePage;
        }

        /// <summary>
        /// Set the last page
        /// </summary>
        /// <param name="page"></param>
        private void SetLastPage(BasePage page)
        {
            lastPages.Push(page);
        }

        /// <summary>
        /// Clear the last pages
        /// </summary>
        private void ClearLastPages()
        {
            lastPages.Clear();
        }

        /// <summary>
        /// Set page
        /// </summary>
        /// <param name="page"></param>
        private void SetPage(BasePage page)
        {
            if (page == null) return;
            if (currentPage && currentPage.CurrentState != BasePage.AnimationState.Stationary)
            {
                return;
            }
            page.transform.SetAsLastSibling();
            var e = new NavigationEventArgs()
            {
                navigationType = NavigationType.New
            };
            currentPage.OnNavigatedFrom(e);
            page.Show(currentPage);
            SetCurrentPage(page, NavigationType.New);
        }

        /// <summary>
        /// Get Page from Menu
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        private BasePage GetPage(MenuItem menuItem)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i].associatedMenu == menuItem)
                {
                    return pages[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Get page from name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private BasePage GetPage(string name)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                if (pages[i].screenName == name)
                {
                    return pages[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Navigate to a page without history
        /// </summary>
        /// <param name="menuItem"></param>
        public void HandleMenuClick(MenuItem menuItem)
        {
            ClearLastPages();
            SetPage(GetPage(menuItem));
        }

        /// <summary>
        /// Naviage to other page with history recorded
        /// </summary>
        /// <param name="name"></param>
        public void Navigate(string name)
        {
            SetLastPage(currentPage);
            var page = GetPage(name);
            if (page != null) SetPage(page);
        }

        /// <summary>
        /// Navigate back
        /// </summary>
        public new void Back()
        {
            var lastPage = GetLastPage();
            var e = new NavigationEventArgs()
            {
                navigationType = NavigationType.Back
            };
            lastPage.ShowWithoutTransition();
            currentPage.OnNavigatedFrom(e);
            currentPage.Hide();
            SetCurrentPage(lastPage, NavigationType.Back);
        }
    }
}