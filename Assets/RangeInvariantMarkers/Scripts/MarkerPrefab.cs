using UnityEngine;

namespace MM.RangeInvariantMarkers
{

    public class MarkerPrefab : MonoBehaviour, IMarkerVisuals
    {
        public TMPro.TextMeshProUGUI markerInfoText;
        public GameObject rootUI;

        GameObject IMarkerVisuals.GetPrefabGO()
        {
            return this.gameObject;
        }

        void IMarkerVisuals.SetMessage(string message)
        {
            markerInfoText.text = message;
        }

        void IMarkerVisuals.UpdateVisuals(Vector3 observerPosition)
        {
            rootUI.transform.LookAt(observerPosition);
            UpdateGazeFade();
        }

        public void LateUpdate()
        {
            UpdateGazeFade();
        }


        #region GazeManagement
        [SerializeField]
        private VisualStatus currentVisualStatus = VisualStatus.VISIBLE;
        //public AnimationCurve fadeCurve;

        public float fadeOutTimeThreshold = 0.2f;
        public float fadeInTimeThreshold = 1.0f;

        public float fadeOutTime = 1.0f;
        public float fadeInTime = 1.0f;

        [SerializeField]
        float timeOfLastGaze = float.MinValue;
        [SerializeField]
        float runningGazeTime = 0.0f;
        float gazeLossTimeout = 0f;

        void IMarkerVisuals.ProcessGaze()
        {
            timeOfLastGaze = Time.time;
            runningGazeTime += Time.deltaTime;
        }

        private void UpdateGazeFade()
        {
            float timeSinceLastGaze = Time.time - timeOfLastGaze;
            if (timeSinceLastGaze > gazeLossTimeout) runningGazeTime = 0;

            switch (currentVisualStatus)
            {
                case VisualStatus.VISIBLE:
                    if (runningGazeTime > fadeOutTimeThreshold)
                        StartFadeOut();
                    break;
                case VisualStatus.FADING_OUT:
                    if (runningGazeTime > 0)
                        FadeOut();
                    else
                        currentVisualStatus = VisualStatus.HIDDEN;
                    break;
                case VisualStatus.HIDDEN:
                    if (runningGazeTime <= float.Epsilon && Time.time - timeOfLastGaze > fadeInTimeThreshold)
                        StartFadeIn();
                    break;
                case VisualStatus.FADING_IN:
                    if (runningGazeTime > 0)
                    {
                        currentVisualStatus = VisualStatus.HIDDEN;
                    }
                    else
                        FadeIn();

                   break;
                default:
                    break;
            }

            var _instancedMaterial = gameObject.GetComponentInChildren<Renderer>().material;
            Color color = _instancedMaterial.GetColor(BaseColorId);

            // 2. Modify the alpha
            color.a = visualsAlpha;

            // 3. Set it back
            _instancedMaterial.SetColor(BaseColorId, color);
        }
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        public Material mat;
        public float visualsAlpha = 1.0f;
        private void StartFadeOut()
        {
            currentVisualStatus = VisualStatus.FADING_OUT;
        }

        private void FadeOut()
        {
            var fadeDuration = (runningGazeTime - fadeOutTimeThreshold);
            var fadePercent = 1 - fadeDuration / fadeOutTime;

            //TODO - ANIMATION CURVE
            visualsAlpha = Mathf.Clamp01(fadePercent);
            if (visualsAlpha < Mathf.Epsilon)
                currentVisualStatus = VisualStatus.HIDDEN;
        }

        private void StartFadeIn()
        {
            currentVisualStatus = VisualStatus.FADING_IN;
        }

        private void FadeIn()
        {
            var lookAwayTime = Time.time - timeOfLastGaze;
            var fadeInTimeSoFar = lookAwayTime - fadeInTimeThreshold;

            var fadePercent = fadeInTimeSoFar / fadeInTime;
            Debug.Log($"Fade in: fadeInTimeSoFar={lookAwayTime} - {fadeInTimeThreshold} = {fadeInTimeSoFar} (fadePercent = {fadePercent})");
            visualsAlpha = Mathf.Clamp01(fadePercent);
            if (1 - visualsAlpha  < Mathf.Epsilon)
                currentVisualStatus = VisualStatus.VISIBLE;
        }

        private enum VisualStatus
        {
            VISIBLE,
            FADING_OUT,
            HIDDEN,
            FADING_IN
        }
        #endregion
    }
}