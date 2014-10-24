using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour {

	public Camera camVisitor;
	public Camera camGlobal;

	public Rect windowRect1 = new Rect (20, 50, 250, 180);
	public Rect windowRect2 = new Rect (20, 20, 120, 100);

	private Painting paintingHit;
	private float rayDist;

	private int algoSelected = 1;

	private bool showOptions = false;
	private bool visitorPOV = true;
	private bool showInfoPainting = false;
	private bool canSelectPainting = false;
	
	void Update () {
		// If I press the Space Key, we stop the movement of the camera and show the GUI Window with the options
		// and when we repress it, it does the oposit
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!showOptions) {
				camVisitor.GetComponent<MouseLook> ().enabled = false;
				GameObject.Find ("Visitor").GetComponent<MouseLook> ().enabled = false;
				showOptions = true;
			} else {
				camVisitor.GetComponent<MouseLook> ().enabled = true;
				GameObject.Find ("Visitor").GetComponent<MouseLook> ().enabled = true;
				showOptions = false;
			}
		}
		if (Input.GetKeyDown (KeyCode.A)) visitorPOV = !visitorPOV;
		if (Input.GetKeyDown (KeyCode.M)) Application.LoadLevel("Scene_Menu");
		if (visitorPOV) {
			camGlobal.enabled = false;
			camVisitor.enabled = true;
			rayDist = 3f;
		}else{
			camVisitor.enabled = false;
			camGlobal.enabled = true;
			rayDist = 100f;
		}

		// Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray ray = Camera.main.camera.ScreenPointToRay(Input.mousePosition);
		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit hit;
		// Perform the raycast and if it hits something ...
		if (Physics.Raycast(ray, out hit)) {
			// And if it is a Painting ...
			if (hit.transform.GetComponent<Painting>() && Vector3.Distance(Camera.main.transform.position,hit.transform.position)<rayDist){
				paintingHit = hit.transform.GetComponent<Painting>();
				if(Input.GetMouseButtonDown(0)){
					int caseSwitch = Application.loadedLevel;
					switch (caseSwitch)
					{
						case 1:
							AgentSimpleBehaviour.setTargetPainting(paintingHit);
							break;
						case 2:
							AgentSimpleGroupBehaviour.setTargetPainting(paintingHit);
							break;
					}
				}
				if(visitorPOV) showInfoPainting = true;
				else showInfoPainting = false;
			}else{showInfoPainting = false;}
		}

	}

	void OnGUI () {
		if (!showOptions) {
			GUI.Label (new Rect (10, 10, 175, 30), "Espace : afficher les options.");
		}else{
			GUI.Label (new Rect (10, 10, 175, 30), "Espace : cacher les options.");
			int caseSwitch = Application.loadedLevel;
			switch (caseSwitch)
			{
			case 1: // Scene_q1
				canSelectPainting = true;
				windowRect1 = GUI.Window (0, windowRect1, OptionsWindowFunction1, "Options");
				break;
			case 2: // Scene_q2
				canSelectPainting = true;
				windowRect2 = GUI.Window (0, windowRect2, OptionsWindowFunction2, "Options");
				break;
			default:
				//Console.WriteLine("Default case");
				break;
			}
		}
		if (showInfoPainting) {
			string text = "Artiste : "+paintingHit.artist + "\nOeuvre : " + paintingHit.paintingName + "\nAnnée : "+paintingHit.year;
			GUI.Box (new Rect (Screen.width - 300,Screen.height - 100,250,50), text);
		}

	}

	void OptionsWindowFunction1 (int windowID) {
		if(GUI.Button(new Rect(10, 20, 100, 30), "Menu Principal")) {
			Application.LoadLevel("Scene_Menu");
		}
		GUI.Label (new Rect (120,25,100,25), "Touche M");
		if (camVisitor.enabled) {
			if (GUI.Button (new Rect (10, 60, 100, 30), "Vue Globale")) {
				visitorPOV = false;
			}
		}else{
			if (GUI.Button (new Rect (10, 60, 100, 30), "Vue Visiteur")) {
				visitorPOV = true;
			}
		}
		GUI.Label (new Rect (120,65,100,25), "Touche A");
		GUI.Label (new Rect (10, 95, 240, 50), "Vous pouvez choisir le tableau de destination de l'agent virtuel en cliquant dessus.");

		string[] algoChoices = new string[] { " ", " ", " " };
		Rect position = new Rect(10, 150, 230, 30);
		algoSelected = GUI.SelectionGrid(position, algoSelected, algoChoices, algoChoices.Length+1, GUI.skin.toggle);
		GUI.Label(new Rect(10,150,230,30),"     BFS        A*           A* sans lien");
		AgentSimpleBehaviour.setAlgo (algoSelected);
		GUI.DragWindow();
	}

	void OptionsWindowFunction2 (int windowID) {

		AgentSimpleGroupBehaviour.setAlgo (algoSelected);
		GUI.DragWindow();
	}

}
