using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    internal static class CanvasExtensions
    {
        public static Vector2 SizeToParent(this RawImage image, float padding = 0)
        {
            var parent = image.transform.parent.GetComponentInParent<RectTransform>();
            var imageTransform = image.GetComponent<RectTransform>();
            if (!parent) return imageTransform.sizeDelta;

            padding = 1 - padding;
            var w = 0f;
            var h = 0f;
            var ratio = image.texture.width / (float) image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
                bounds.size = new Vector2(bounds.height, bounds.width);

            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            {
                //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }

            imageTransform.sizeDelta = new Vector2(w, h);
            return imageTransform.sizeDelta;
        }
    }
}