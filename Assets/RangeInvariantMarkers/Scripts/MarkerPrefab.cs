using UnityEngine;
using UnityEngine.InputSystem;

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

        //public AnimationCurve fadeCurve;
        public float fadeOutTimeThreshold = 0.2f;
        public float fadeInTimeThreshold = 1.0f;

        public float fadeOutDuration = 0.5f;
        public float fadeInDuration = 1.0f;

        private float lastGazeTimestamp = 0f;
        private float gazeLossTimeout = 0.2f;
        private bool currentlyLookedAt = false;
        private float lookedAtTimer = 0f, lookAwayTimer = 0f;
        void IMarkerVisuals.ProcessGaze()
        {
            lastGazeTimestamp = Time.time;
            currentlyLookedAt = true;
        }

        private void UpdateGazeFade()
        {
            if (Time.time - lastGazeTimestamp > gazeLossTimeout)
            {
                currentlyLookedAt = false;
            }

            if (currentlyLookedAt)
            {
                lookAwayTimer = 0;
                lookedAtTimer += Time.deltaTime;
            } else
            {
                lookedAtTimer = 0;
                lookAwayTimer += Time.deltaTime;
            }

            if (lookedAtTimer > fadeOutTimeThreshold)
            {
                UpdateVisualsAlpha(0, fadeOutDuration);
            }
            else if (lookAwayTimer > fadeInTimeThreshold)
            {
                UpdateVisualsAlpha(1, fadeInDuration);
            }


            var _instancedMaterial = gameObject.GetComponentInChildren<Renderer>().material;
            Color color = _instancedMaterial.GetColor(BaseColorId);

            // 2. Modify the alpha
            color.a = visualsAlpha;

            // 3. Set it back
            _instancedMaterial.SetColor(BaseColorId, color);
        }
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        public float visualsAlpha = 1.0f;

        private void UpdateVisualsAlpha(float targetValue, float duration)
        {
            if (duration < 0)
            {
                visualsAlpha = targetValue;
                return;
            }

            float step = Time.deltaTime / duration;
            visualsAlpha = Mathf.MoveTowards(visualsAlpha, targetValue, step);
        }

        #endregion
    }
}