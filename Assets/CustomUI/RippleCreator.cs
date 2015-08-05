using UnityEngine;
using System.Collections;
using MaterialUI;

namespace CustomUI
{
    public class RippleCreator : RippleConfig
    {
        public void SetNormalColor(Color color)
        {
            this.normalColor = color;
        }
    }
}