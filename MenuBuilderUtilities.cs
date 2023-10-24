using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace CC.SoundSystem
{
    public static class MenuBuilderUtilities
    {
        [UnityEditor.MenuItem("GameObject/CC/Mixer Menu")]
        public static void BuildMixerMenu()
        {
            Canvas canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            MixerMenu menu = new GameObject("Mixer Menu", typeof(MixerMenu)).GetComponent<MixerMenu>();
            menu.transform.SetParent(canvas.transform);
            (menu.transform as RectTransform).Fill();
            menu.UpdateMenu();
        }

        public static void Fill(this RectTransform child)
        {
            if (child.parent.transform.GetType() != typeof(RectTransform)) return;
            child.anchorMin = (child.parent.transform as RectTransform).anchorMin;
            child.anchorMax = (child.parent.transform as RectTransform).anchorMax;
            child.anchoredPosition = (child.parent.transform as RectTransform).anchoredPosition;
            child.sizeDelta = (child.parent.transform as RectTransform).sizeDelta;
        }

        [System.Serializable]
        public class MenuComponentException : System.Exception
        {
            public MenuComponentException() { }
            public MenuComponentException(string message) : base(message) { }
            public MenuComponentException(string message, System.Exception inner) : base(message, inner) { }
            protected MenuComponentException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
