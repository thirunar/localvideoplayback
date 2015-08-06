using UnityEngine;
using System.Collections;

namespace CustomUI
{
    public enum NavigationType
    {
        New,
        Back
    }

    public class NavigationEventArgs
    {
        public NavigationType navigationType;

        public NavigationEventArgs()
        {

        }
    }

    public class CancellationEventArgs
    {
        public bool CancelEvent;

        public CancellationEventArgs() : base()
        {

        }
    }
}