using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public static class ScreenUtils
    {
        private static ScreenOrientation currentOrientation = ScreenOrientation.AutoRotation; // This default value will force first time setup

        public static Vector2 CenterScreen
        {
            get
            {
                EnsureVariables();
                return centerScreen;
            }
        }

        private static Vector2 centerScreen;

        private static void EnsureVariables()
        {
            if (currentOrientation == Screen.orientation)
            {
                return;
            }

            var midPointWidth = Screen.width * 0.5f;
            var midPointHeight = Screen.height * 0.5f;

            if (Screen.orientation == ScreenOrientation.Landscape)
            {
                centerScreen = new Vector2(midPointWidth, midPointHeight);
                return;
            }

            centerScreen = new Vector2(midPointWidth, midPointHeight);
        }
    }
}