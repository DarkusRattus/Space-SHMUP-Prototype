using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour {

	// ************************** Bounds Functions **************************

	// Creates bounds that encapsulate the two Bounds passed in
	public static Bounds BoundsUnion(Bounds b0, Bounds b1){

		// if the size of one of the bounds is Vector3.zero, ignore that one
		if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
			return b1;
		} else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
			return b0;
		} else if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
			return b0;
		}

		// Stretch b0 to include the b1.min and b1.max
		b0.Encapsulate (b1.min);
		b0.Encapsulate (b1.max);
		return(b0);

	}

	public static Bounds CombineBoundsOfChildren(GameObject go){
		// Create an empty Bounds b
		Bounds b = new Bounds (Vector3.zero, Vector3.zero);
		// If this GameObject has a Renderer Component...
		if(go.renderer != null){
			// Expand b to contain the Renderer's bounds
			b = BoundsUnion(b, go.renderer.bounds);
		}
		// If this GameObject has a Collider Component...
		if(go.collider != null){
			// Expand b to contain the Collider''s bounds
			b = BoundsUnion(b, go.collider.bounds);
		}
		// Recursively iterate through each child of this gameObject.transform
		foreach (Transform t in go.transform) {
			// Expand b to contain their bounds as well
			b = BoundsUnion (b, CombineBoundsOfChildren (t.gameObject));
		}
		return b;
	}

	// Make a static read-only public property camBounds
	static public Bounds camBounds {
		get {
			// If _camBounds hasn't been set yet
			if (_camBounds.size == Vector3.zero) {
				// SetCameraBounds using the default camera
				SetCameraBounds ();
			}
			return (_camBounds);
		}
	}

	// This is a private static field that camBounds uses
	static private Bounds _camBounds;

	// This function is used by camBounds to set _camBouds and can also be called directly
	public static void SetCameraBounds(Camera cam = null){
		// If no Camera was passed in, use the Main Camera
		if (cam == null)
			cam = Camera.main;
		// This makes a couple important assumptions about the camera!:
		// 1. The camera is Orthographic
		// 2. The camera is at a rotation of R:[0,0,0]

		// Make Vector3s at the topLeft and bottomRight of the Screen coords
		Vector3 topLeft = new Vector3 (0, 0, 0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);

		// Convert these to world coordinates
		Vector3 boundTLN = cam.ScreenToWorldPoint (topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight);

		// Adjust their zs to be at the near and far Camera clipping positions
		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;

		// Find the center of the Bounds
		Vector3 center = (boundBRF + boundTLN) / 2f;
		_camBounds = new Bounds (center, Vector3.zero);
		// Expand _camBounds to encapsulate the extents,
		_camBounds.Encapsulate (boundTLN);
		_camBounds.Encapsulate (boundBRF);
	}

}
