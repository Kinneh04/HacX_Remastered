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

		public void OnSelectBuilding()
        {
			isSelected = true;
			//_highlightMaterial.color = Color.green;
			_meshRenderer.material = _highlightMaterial;
		}

		public void OnDeselectBuilding()
        {
			isSelected = false;
			_highlightMaterial.color = Color.red;
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