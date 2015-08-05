using UnityEngine;
using System;
using MaterialUI;

namespace CustomUI
{
    public class MenuManager : NavDrawerConfig
    {
        // Use this for initialization
        void Start()
        {
            RecalcSize();
        }

        void RecalcSize()
        {
            maxPosition = thisRectTransform.rect.width / 2;
            minPosition = -maxPosition;

            backgroundRectTransform.sizeDelta = new Vector2(CanvasConstants.canvasWidth, backgroundRectTransform.sizeDelta.y);
        }

        public new void Open()
        {
            RecalcSize();
            base.Open();
        }
    }
}