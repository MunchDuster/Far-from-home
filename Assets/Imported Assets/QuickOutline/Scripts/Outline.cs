using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour 
{
	static Material outlineMaskMaterial;
  	static Material outlineFillMaterial;
  	static Material outlineEdgeMaterial;
  	static bool initialized;

	public enum ColorMode{
		Normal,
		Completed,
		Problem
	}

	[SerializeField] ColorMode colorMode;


  	[SerializeField] Renderer[] renderers;
  
  	void Awake() 
	{
		if(!initialized)
		{
			initialized = true;

			outlineMaskMaterial = Resources.Load<Material>(@"Materials/OutlineMask");
			outlineFillMaterial = Resources.Load<Material>(@"Materials/OutlineFill");
			outlineEdgeMaterial = Resources.Load<Material>(@"Materials/OutlineEdge");
		}
	}

	public void SetColorMode(ColorMode mode)
	{
		colorMode = mode;
		UpdateOutlineColour();
	}

	void UpdateOutlineColour()
	{
		Color color = Color.white;

		switch(colorMode)
		{
			case ColorMode.Normal:
				color = new Color(1f, 0.7f, 0.2f, 1f);
				break;
			case ColorMode.Completed:
				color = new Color(0.44f, 0.94f, 0.44f, 1f);
				break;
			case ColorMode.Problem:
				color = new Color(0.85f, 0.23f, 0.23f);
				break;
		}

		outlineFillMaterial.SetColor("_OutlineColor", color);
		outlineEdgeMaterial.SetColor("_OutlineColor", new Color(color.r, color.g, color.b, 0.04f));
	}

	void OnEnable()
	{
		UpdateOutlineColour();
		
		//Add shaders to renderers
		foreach (var renderer in renderers) 
		{
			var materials = renderer.sharedMaterials.ToList();
			materials.Add(outlineEdgeMaterial);
			materials.Add(outlineMaskMaterial);
			materials.Add(outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

  	void OnDisable()	
	{
		foreach (var renderer in renderers) 
		{
			var materials = renderer.sharedMaterials.ToList();
			materials.Remove(outlineEdgeMaterial);
			materials.Remove(outlineMaskMaterial);
			materials.Remove(outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}
}
