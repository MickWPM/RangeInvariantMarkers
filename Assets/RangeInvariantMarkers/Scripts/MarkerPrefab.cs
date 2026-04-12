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
        }
    }
}