
using UnityEngine;

namespace MM.RangeInvariantMarkers
{

    public interface IMarkerVisuals
    {
        public void SetMessage(string message);
        public GameObject GetPrefabGO();
        public void UpdateVisuals(Vector3 observerPosition);

    }
}