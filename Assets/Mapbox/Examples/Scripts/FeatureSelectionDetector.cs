namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Unity.MeshGeneration.Data;

	public class FeatureSelectionDetector : MonoBehaviour
	{
		private FeatureUiMarker _marker;
		private VectorEntity _feature;

		public MapPickerManager mapPickerManager;

		public void OnMouseUpAsButton()
		{
			//_marker.Show(_feature);

			// Select Building here 

			if (!mapPickerManager) mapPickerManager = GameObject.FindObjectOfType<MapPickerManager>();
			mapPickerManager.ToggleBuildingSelect(gameObject);
		}

		internal void Initialize(FeatureUiMarker marker, VectorEntity ve)
		{
			_marker = marker;
			_feature = ve;
		}
	}
}