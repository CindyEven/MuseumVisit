using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour {

	public Camera camVisitor;
	public Camera camGlobal;

	private Rect windowRect1 = new Rect (20, 50, 250, 180);
	private Rect windowRect2 = new Rect (20, 50, 250, 310);
	public Rect windowRect3a = new Rect (20, 50, 250, 310);
//	private Rect windowRect3b = new Rect (20, 50, 250, 310);


	private Painting paintingHit;
	private float rayDist;

	private int algoSelected = 1;
	private float seekCoef = 1.25f, sepCoef = 1.20f, cohesionCoef = 0.60f, alignCoef = 1.0f;

	private bool showOptions = false;
	private bool visitorPOV = true;
	private bool showInfoPainting = false;
	
	void Update () {
		// On Space key pressed, the movement of the camera is stopped and the GUI Window with the options appears
		// If the key is pressed again it does the opposite
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
					switch (caseSwitch) // For the scene 1 and 2 we can select the paiting 'destination' by clicking on it
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
				windowRect1 = GUI.Window (0, windowRect1, OptionsWindowFunction1, "Options");
				break;
			case 2: // Scene_q2
				windowRect2 = GUI.Window (0, windowRect2, OptionsWindowFunction2, "Options");
				break;
			case 3: // Scene_q3a
				windowRect3a = GUI.Window (0, windowRect3a, OptionsWindowFunction3a, "Options");
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
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 240, 50), "Vous pouvez choisir le tableau de destination de l'agent virtuel en cliquant dessus.");
		string[] algoChoices = new string[] { " ", " ", " " };
		Rect position = new Rect(10, 150, 230, 30);
		algoSelected = GUI.SelectionGrid(position, algoSelected, algoChoices, algoChoices.Length+1, GUI.skin.toggle);
		GUI.Label(new Rect(10,150,230,30),"     BFS        A*           A* sans lien");
		AgentSimpleBehaviour.setAlgo (algoSelected);
		GUI.DragWindow();
	}

	void OptionsWindowFunction2 (int windowID) {
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 240, 50), "Vous pouvez choisir le tableau de destination de l'agent virtuel en cliquant dessus.");
		string[] algoChoices = new string[] { " ", " ", " " };
		Rect position = new Rect(10, 150, 230, 30);
		algoSelected = GUI.SelectionGrid(position, algoSelected, algoChoices, algoChoices.Length+1, GUI.skin.toggle);
		GUI.Label(new Rect(10,150,230,30),"     BFS        A*           A* sans lien");
		AgentSimpleGroupBehaviour.setAlgo (algoSelected);
		GUI.Label(new Rect(10,175,230,30),"Modification des coeficients :");
		GUI.Label(new Rect(10,200,230,30),"Seek");
		seekCoef = GUI.HorizontalSlider (new Rect (100, 205, 100, 30), seekCoef, 0.0f, 5.0f);
		GUI.Label(new Rect(10,225,230,30),"Séparation");
		sepCoef = GUI.HorizontalSlider (new Rect (100, 230, 100, 30), sepCoef, 0.0f, 5.0f);
		GUI.Label(new Rect(10,250,230,30),"Cohésion");
		cohesionCoef = GUI.HorizontalSlider (new Rect (100, 255, 100, 30), cohesionCoef, 0.0f, 5.0f);
		GUI.Label(new Rect(10,275,230,30),"Alignement");
		alignCoef = GUI.HorizontalSlider (new Rect (100, 280, 100, 30), alignCoef, 0.0f, 5.0f);
		SteeringBehaviour.updateSteeringCoeff (seekCoef, sepCoef, cohesionCoef, alignCoef);
		GUI.DragWindow();
	}

	void OptionsWindowFunction3a (int windowID) {
		GlobalOptions ();
		GUI.DragWindow();
	}

	// Elements of the options Window common for all the scenes :
	void GlobalOptions(){
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
	}

}
