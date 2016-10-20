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

}
