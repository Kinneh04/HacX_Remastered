namespace Mapbox.Examples
{
	using UnityEngine;
	using System.Collections.Generic;

	public class HighlightFeature : MonoBehaviour
	{
		static Material _highlightMaterial;

		private List<Material> _materials = new List<Material>();

		MeshRenderer _meshRenderer;

		public bool isSelected = false;

		public List<GameObject> IntersectingObjects = new();

		public MapPickerManager mapPicker;

        public void OnTriggerEnter(Collider other)
        {
			if (!IntersectingObjects.Contains( other.gameObject))
			{
				IntersectingObjects.Add( other.gameObject);
			}
		}

        public void OnTriggerExit(Collider other)
        {
            if(IntersectingObjects.Contains(other.gameObject))
            {
				IntersectingObjects.Remove(other.gameObject);
            }
        }

        private void OnTriggerStay(Collider other)
        {
			if (!IntersectingObjects.Contains( other.gameObject))
			{
				IntersectingObjects.Add( other.gameObject);
			}
		}
        public void OnCollisionEnter(Collision collision)
        {
            if(!IntersectingObjects.Contains(collision.gameObject))
            {
				IntersectingObjects.Add(collision.gameObject);
            }
        }

        public void OnCollisionStay(Collision collision)
        {
			if (!IntersectingObjects.Contains(collision.gameObject))
			{
				IntersectingObjects.Add(collision.gameObject);
			}
		}

        public void SelectIntersecting()
        {

			foreach(GameObject C in IntersectingObjects)
            {
				HighlightFeature f = C.GetComponent<HighlightFeature>();
				
				if (f&&!f.isSelected) f.OnSelectBuilding();
            }

			//List<Transform> OverlappingTransforms = new();
			//Collider[] allColliders = Physics.OverlapBox(transform.position, transform.localScale);
			//foreach (Collider otherCollider in allColliders)
			//{
			//	HighlightFeature h = otherCollider.GetComponent<HighlightFeature>();
			//	if (!h.isSelected)
			//	{
			//		OverlappingTransforms.Add(otherCollider.transform);

			//		h.OnSelectBuilding() ;
					
			//	}
			//}
		}

		public void DeSelectIntersecting()
		{

			foreach (GameObject C in IntersectingObjects)
			{
				HighlightFeature f = C.GetComponent<HighlightFeature>();
				if (f&&f.isSelected) f.OnDeselectBuilding();
			}
			//List<Transform> OverlappingTransforms = new();
			//Collider[] allColliders = Physics.OverlapBox(transform.position, transform.localScale / 2);
			//foreach (Collider otherCollider in allColliders)
			//{
			//	HighlightFeature h = otherCollider.GetComponent<HighlightFeature>();
			//	if (h.isSelected)
			//	{
			//		OverlappingTransforms.Add(otherCollider.transform);
			//		if (otherCollider.gameObject != gameObject)
			//		{
			//			h.OnDeselectBuilding();
			//		}
			//	}
			//}
		}

		public void OnSelectBuilding()
        {
			if (isSelected) DeSelectIntersecting();
			else
			{
				isSelected = true;
				//_highlightMaterial.color = Color.green;
				_meshRenderer.material = _highlightMaterial;
				SelectIntersecting();
			}
		}

		public void OnDeselectBuilding()
        {
			isSelected = false;
			_highlightMaterial.color = Color.red;
			_meshRenderer.materials = _materials.ToArray();
			if(mapPicker.SelectedBuildings.Contains(gameObject)) mapPicker.SelectedBuildings.Remove(gameObject);
			DeSelectIntersecting();
		}

        void Start()
		{
			if (_highlightMaterial == null)
			{
				_highlightMaterial = Instantiate(GetComponent<MeshRenderer>().material);
				_highlightMaterial.color = Color.red;
			}

			_meshRenderer = GetComponent<MeshRenderer>();
			
			foreach (var item in _meshRenderer.sharedMaterials)
			{
				_materials.Add(item);
			}
			mapPicker = GameObject.FindObjectOfType<MapPickerManager>();
		}

		public void OnMouseEnter()
		{
			if (isSelected) return;
			_meshRenderer.material = _highlightMaterial;
		}

		public void OnMouseExit()
		{
			if (isSelected) return;
			_meshRenderer.materials = _materials.ToArray();
		}
	}
}