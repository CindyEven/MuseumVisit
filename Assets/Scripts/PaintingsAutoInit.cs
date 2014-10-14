using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Linq;

public class PaintingsAutoInit : MonoBehaviour {
	void Awake(){
		transform.position = Vector3.zero;
		transform.rotation = Quaternion.identity;
		transform.localScale = Vector3.one;
		gameObject.name = "Paintings";
	}
}