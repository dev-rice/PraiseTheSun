using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CRTDisplay : MonoBehaviour {

	private Material material;

	// Creates a private material used to the effect
	void Awake(){
		material = new Material(Shader.Find("Hidden/CRTDisplayShader"));
	}

	// Postprocess the image
	void OnRenderImage(RenderTexture source, RenderTexture destination){
		material.SetFloat("_CurrentTime", Time.time);
		Graphics.Blit (source, destination, material);
	}
}
