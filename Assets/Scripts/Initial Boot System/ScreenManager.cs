using UnityEngine;

namespace InitBootSystem
{
    public class ScreenManager : MonoBehaviour
    {
        private Camera mainCam;

        void Start()
        {
            mainCam = Camera.main;
            SetScreenResolution();
        }

        /// <summary>
        /// Устанавливает ширину экрана относительно разрешения смартфона.
        /// </summary>
        private void SetScreenResolution()
        {
            var ortSize = mainCam.orthographicSize;
            float currentAspect = 18f / 39f;

            var widthSize = ortSize * currentAspect / mainCam.aspect;
            mainCam.orthographicSize = widthSize;
        }
    }
}
