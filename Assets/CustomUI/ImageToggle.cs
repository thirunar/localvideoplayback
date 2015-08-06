using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CustomUI
{
    public class ImageToggle : MonoBehaviour
    {
        private Sprite OriginalImage;
        private Image thisImage;
        public Sprite ToggleImage;

        void Start()
        {
            thisImage = gameObject.GetComponent<Image>();
            if (thisImage != null)
                OriginalImage = thisImage.sprite;
        }

        public void ToggleSprite()
        {
            if (thisImage != null)
                SetSprite(thisImage.sprite != OriginalImage);
        }

        public void SetSprite(bool setOriginal)
        {
            if (thisImage != null)
                thisImage.sprite = setOriginal ? OriginalImage : ToggleImage;
        }
    }
}