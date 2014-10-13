using UnityEngine;
using System.Collections;

public class ShowPaintingInfo : MonoBehaviour {

	private bool showGUI = false;
	private string text;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Ray ray = GetComponentInChildren<Camera>().camera.ScreenPointToRay(Input.mousePosition);
		
		if (Physics.Raycast(ray, out hit)) {
			Transform objectHit = hit.transform;
			if (objectHit.GetComponent<Painting>()&& Vector3.Distance(transform.position,objectHit.transform.position)<3.0f){
				Debug.Log("Artist : "+objectHit.GetComponent<Painting>().artist);
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
