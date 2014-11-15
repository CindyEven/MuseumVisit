using UnityEngine;
using System.Collections;

public class UserInterface : MonoBehaviour {

	public Camera camVisitor;
	public Camera camGlobal;
	public GameObject agentPrefab;

	private Rect windowRect1 = new Rect (20, 50, 250, 180);
	private Rect windowRect2 = new Rect (20, 50, 250, 380);
	private Rect windowRect3 = new Rect (20, 50, 250, 300);
	private Rect windowRect4 = new Rect (20, 50, 250, 220);


	private Painting paintingHit;
	private float rayDist;

	private int nbAgents = 1;

	private int algoSelected = 1;
	public float seekCoefDefault = 1.25f, sepCoefDefault = 1.20f, cohesionCoefDefault = 0.60f, alignCoefDefault = 1.0f;
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
				GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = false;
				showOptions = true;
			} else {
				camVisitor.GetComponent<MouseLook> ().enabled = true;
				GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = true;
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

	void NbAgentsUpdate(){
		Destroy (GameObject.Find ("Agents"));
		GameObject agentsParent = new GameObject ("Agents");

		for (int x=0; x<nbAgents; x++) {
			GameObject newAgent = (GameObject) Instantiate(agentPrefab, Vector3.zero, Quaternion.Euler(new Vector3(0,90,0)));
			newAgent.name = agentPrefab.name;
			newAgent.transform.parent = agentsParent.transform;
			int i = newAgent.transform.GetSiblingIndex ();
			newAgent.transform.position = new Vector3((-6.5f+(i/10)*1.5f),1f,(-15.5f-(i%10)));
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
				windowRect3 = GUI.Window (0, windowRect3, OptionsWindowFunction3, "Options");
				break;
			case 4: // Scene_q3b
				windowRect3 = GUI.Window (0, windowRect3, OptionsWindowFunction3, "Options");
				break;
			case 5: // Scene_visit1
				windowRect4 = GUI.Window (0, windowRect4, OptionsWindowFunction4, "Options");
				break;
			default:
				//Console.WriteLine("Default case");
				break;
			}
		}
		if (showInfoPainting) {
			string text = "Artiste : "+paintingHit.artist + "\nOeuvre : " + paintingHit.paintingName + "\nAnnée : "+paintingHit.year;
			GUI.Box (new Rect (10,Screen.height - 60,Screen.width-20,50), text);
		}

	}
	
	void OptionsWindowFunction1 (int windowID) {
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 240, 50), "Vous pouvez choisir le tableau de destination du visiteur en cliquant dessus.");
		string[] algoChoices = new string[] { " ", " ", " " };
		Rect position = new Rect(10, 150, 230, 30);
		algoSelected = GUI.SelectionGrid(position, algoSelected, algoChoices, algoChoices.Length+1, GUI.skin.toggle);
		GUI.Label(new Rect(10,150,230,30),"     BFS        A*           A* sans lien");
		AgentSimpleBehaviour.setAlgo (algoSelected);
		GUI.DragWindow();
	}

	void OptionsWindowFunction2 (int windowID) {
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 240, 50), "Vous pouvez choisir le tableau de destination des visiteurs en cliquant dessus.");
		GUI.Label(new Rect(10,150,230,30),"Nombre de visiteurs : ");
		GUI.Label (new Rect (100, 175, 60, 30), nbAgents.ToString());
		nbAgents = (int) GUI.HorizontalSlider (new Rect (10, 180, 80, 30), nbAgents, 1, 100);
		if (GUI.Button (new Rect (130,175,90,20), "Appliquer")) {
			NbAgentsUpdate();
		}
		string[] algoChoices = new string[] { " ", " ", " " };
		Rect position = new Rect(10, 200, 230, 30);
		algoSelected = GUI.SelectionGrid(position, algoSelected, algoChoices, algoChoices.Length+1, GUI.skin.toggle);
		GUI.Label(new Rect(10,200,230,30),"     BFS        A*           A* sans lien");
		AgentSimpleGroupBehaviour.setAlgo (algoSelected);
		GUI.Label(new Rect(10,225,230,30),"Modification des coeficients :");
		GUI.Label(new Rect(10,250,230,30),"Seek");
		GUI.Label (new Rect (200, 250, 60, 30), seekCoef.ToString("f2"));
		GUI.Label(new Rect(10,275,230,30),"Séparation");
		GUI.Label (new Rect (200, 275, 60, 30), sepCoef.ToString("f2"));
		GUI.Label(new Rect(10,300,230,30),"Cohésion");
		GUI.Label (new Rect (200, 300, 60, 30), cohesionCoef.ToString("f2"));
		GUI.Label(new Rect(10,325,230,30),"Alignement");
		GUI.Label (new Rect (200, 325, 60, 30), alignCoef.ToString("f2"));
		if (GUI.Button (new Rect (10,350,60,20), "Reset")) {
			seekCoef = GUI.HorizontalSlider (new Rect (90, 255, 100, 30), seekCoefDefault, 0.0f, 5.0f);
			sepCoef = GUI.HorizontalSlider (new Rect (90, 280, 100, 30), sepCoefDefault, 0.0f, 5.0f);
			cohesionCoef = GUI.HorizontalSlider (new Rect (90, 305, 100, 30), cohesionCoefDefault, 0.0f, 5.0f);
			alignCoef = GUI.HorizontalSlider (new Rect (90, 330, 100, 30), alignCoefDefault, 0.0f, 5.0f);
		}else{
			seekCoef = GUI.HorizontalSlider (new Rect (90, 255, 100, 30), seekCoef, 0.0f, 5.0f);
			sepCoef = GUI.HorizontalSlider (new Rect (90, 280, 100, 30), sepCoef, 0.0f, 5.0f);
			cohesionCoef = GUI.HorizontalSlider (new Rect (90, 305, 100, 30), cohesionCoef, 0.0f, 5.0f);
			alignCoef = GUI.HorizontalSlider (new Rect (90, 330, 100, 30), alignCoef, 0.0f, 5.0f);
		}
		SteeringBehaviour.updateSteeringCoeff (seekCoef, sepCoef, cohesionCoef, alignCoef);
		GUI.DragWindow();
	}

	void OptionsWindowFunction3 (int windowID) {
		GlobalOptions ();
		GUI.Label(new Rect(10,95,230,30),"Nombre de visiteurs : ");
		GUI.Label (new Rect (100, 120, 60, 30), nbAgents.ToString());
		nbAgents = (int) GUI.HorizontalSlider (new Rect (10, 125, 80, 30), nbAgents, 1, 100);
		if (GUI.Button (new Rect (130,120,90,20), "Appliquer")) {
			NbAgentsUpdate();
		}
		GUI.Label(new Rect(10,145,230,30),"Modification des coeficients :");
		GUI.Label(new Rect(10,170,230,30),"Seek");
		GUI.Label (new Rect (200, 170, 60, 30), seekCoef.ToString("f2"));
		GUI.Label(new Rect(10,195,230,30),"Séparation");
		GUI.Label (new Rect (200, 195, 60, 30), sepCoef.ToString("f2"));
		GUI.Label(new Rect(10,220,230,30),"Cohésion");
		GUI.Label (new Rect (200, 220, 60, 30), cohesionCoef.ToString("f2"));
		GUI.Label(new Rect(10,245,230,30),"Alignement");
		GUI.Label (new Rect (200, 245, 60, 30), alignCoef.ToString("f2"));
		if (GUI.Button (new Rect (10,270,60,20), "Reset")) {
			seekCoef = GUI.HorizontalSlider (new Rect (90, 175, 100, 30), seekCoefDefault, 0.0f, 5.0f);
			sepCoef = GUI.HorizontalSlider (new Rect (90, 200, 100, 30), sepCoefDefault, 0.0f, 5.0f);
			cohesionCoef = GUI.HorizontalSlider (new Rect (90, 225, 100, 30), cohesionCoefDefault, 0.0f, 5.0f);
			alignCoef = GUI.HorizontalSlider (new Rect (90, 250, 100, 30), alignCoefDefault, 0.0f, 5.0f);
		}else{
			seekCoef = GUI.HorizontalSlider (new Rect (90, 175, 100, 30), seekCoef, 0.0f, 5.0f);
			sepCoef = GUI.HorizontalSlider (new Rect (90, 200, 100, 30), sepCoef, 0.0f, 5.0f);
			cohesionCoef = GUI.HorizontalSlider (new Rect (90, 225, 100, 30), cohesionCoef, 0.0f, 5.0f);
			alignCoef = GUI.HorizontalSlider (new Rect (90, 250, 100, 30), alignCoef, 0.0f, 5.0f);
		}
		SteeringBehaviour.updateSteeringCoeff (seekCoef, sepCoef, cohesionCoef, alignCoef);
		GUI.DragWindow();
	}

	void OptionsWindowFunction4 (int windowID) {
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 230, 42), "Choisissez les critères de fin d'observation : ");
		Watch.timerCond = GUI.Toggle(new Rect(10,135,230,30), Watch.timerCond, "Timer");
		Watch.visitorNearCond = GUI.Toggle(new Rect(10,160,230,30), Watch.visitorNearCond, "Visiteur à coté");
		Watch.nbAgentsNearCond = GUI.Toggle(new Rect(10,185,230,30), Watch.nbAgentsNearCond, "Nombre d'agents limite");
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
