using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

public class PaintingsAutoInit : MonoBehaviour {
	void Awake(){
		// Reset and rename the GameObject as soon as we attach this Scrip to it
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		gameObject.name = "Paintings";
	}
}