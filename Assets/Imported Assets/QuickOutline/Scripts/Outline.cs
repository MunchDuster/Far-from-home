//
//  Outline.cs
//  QuickOutline
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour {
	private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
  	private static Material outlineMaskMaterial;
  	private static Material outlineFillMaterial;
  	private static Material outlineMaskMaterialInstance;
  	private static Material outlineFillMaterialInstance;
  	private static bool initialized;

  	private Renderer[] renderers;
  
  	void Awake() {

    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

	if(!initialized)
	{
		initialized = true;

		outlineMaskMaterial = Resources.Load<Material>(@"Materials/OutlineMask");
		outlineFillMaterial = Resources.Load<Material>(@"Materials/OutlineFill");

		outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
		outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
	}

	outlineMaskMaterialInstance = Instantiate(outlineMaskMaterial);
	outlineFillMaterialInstance = Instantiate(outlineFillMaterial);
  }

  void OnEnable()
  {
	//Add shaders to renderers
	foreach (var renderer in renderers) 
	{
      var materials = renderer.sharedMaterials.ToList();
		materials.Add(outlineMaskMaterialInstance);
      	materials.Add(outlineFillMaterialInstance);
      renderer.materials = materials.ToArray();
    }
  }

  void OnDisable()
	{
		foreach (var renderer in renderers) 
		{
		var materials = renderer.sharedMaterials.ToList();
			materials.Remove(outlineMaskMaterialInstance);
			materials.Remove(outlineFillMaterialInstance);
		renderer.materials = materials.ToArray();
		}
	}
}
