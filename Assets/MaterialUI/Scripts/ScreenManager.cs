//  Copyright 2014 Invex Games http://invexgames.com
//	Licensed under the Apache License, Version 2.0 (the "License");
//	you may not use this file except in compliance with the License.
//	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//	Unless required by applicable law or agreed to in writing, software
//	distributed under the License is distributed on an "AS IS" BASIS,
//	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//	See the License for the specific language governing permissions and
//	limitations under the License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MaterialUI
{
	public class ScreenManager : MonoBehaviour
	{
		public ScreenConfig[] screens;
        public ScreenConfig homeScreen;
		[HideInInspector]
		public ScreenConfig currentScreen;
		[HideInInspector]
		public Stack<ScreenConfig> lastScreens;

        private void init()
        {
            if (lastScreens == null)
                lastScreens = new Stack<ScreenConfig>();
            if (homeScreen == null)
                throw new NullReferenceException("Home screen not set");
            if (currentScreen == null)
                currentScreen = homeScreen;
        }

        private ScreenConfig GetLastScreen()
        {
            init();
            if (lastScreens.Count > 0)
            {
                return lastScreens.Pop();
            }
            return homeScreen;
        }

        private void SetLastScreen(ScreenConfig screen)
        {
            init();
            lastScreens.Push(screen);
        }

        private void ClearLastScreen()
        {
            init();
            lastScreens.Clear();
        }

		private void SetScreen(string name)
		{
            init();
            int index = 0;
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i].screenName == name)
                {
                    index = i;
                    break;
                }
            }
            if (index == 0) return;
			screens[index].transform.SetAsLastSibling();
			screens[index].Show(currentScreen);
			currentScreen = screens[index];
            if (currentScreen == homeScreen)
            {
                ClearLastScreen();
            }
		}

		public void Set(string name)
		{
            init();
            ClearLastScreen();
            SetScreen(name);
		}

        public void Navigate(string name)
        {
            init();
            SetLastScreen(currentScreen);
            SetScreen(name);
        }

		public void Back()
		{
            init();
            var lastScreen = GetLastScreen();
			lastScreen.ShowWithoutTransition();
			currentScreen.Hide();
			currentScreen = lastScreen;
		}
	}
}