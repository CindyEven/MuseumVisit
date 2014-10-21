using UnityEngine;
using System.Collections;

public class ShowPaintingInfo : MonoBehaviour {

	private bool showGUI = false;
	private string text;

	// Update is called once per frame
	void Update () {
		// Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray ray = GetComponentInChildren<Camera>().camera.ScreenPointToRay(Input.mousePosition);

		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit hit;
		// Perform the raycast and if it hits something ...
		if (Physics.Raycast(ray, out hit)) {
			Transform objectHit = hit.transform;
			// And if it is a Painting ...
			if (objectHit.GetComponent<Painting>()&& Vector3.Distance(transform.position,objectHit.transform.position)<3.0f){
				// We show the informations about the Painting.
				text = "Artiste : "+objectHit.GetComponent<Painting>().artist + "\nOeuvre : " + objectHit.GetComponent<Painting>().paintingName + "\nAnnée : "+objectHit.GetComponent<Painting>().year;
				showGUI = true;
			}else{
				showGUI = false;
			}
		}
	}

	void OnGUI(){
		if (showGUI) {
			GUI.Box (new Rect (Screen.width - 300,Screen.height - 100,250,50), text);
		}
	}
}
