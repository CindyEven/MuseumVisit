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
	private Rect windowRect5 = new Rect (20, 50, 250, 220);
	private Rect windowProfilUser = new Rect (20, 50, 250, 415);
	private Rect windowRatePainting = new Rect (20,50,250,150);

	private Painting paintingHit;
	private float rayDist;

	private int nbAgents = 1;

	private int algoSelected = 1;
	public float seekCoefDefault = 3.0f, sepCoefDefault = 2.5f, cohesionCoefDefault = 1.0f, alignCoefDefault = 1.0f;
	private float seekCoef = 3.0f, sepCoef = 2.5f, cohesionCoef = 1.0f, alignCoef = 1.0f;

	private bool showOptions = false;
	private bool showChoiceProfil = true;
	private bool visitorPOV = true;
	private bool showInfoPainting = false;
	private bool showRatingMenu = false;

	bool[] tagsToggles = new bool[13]{false,false,false,false,false,false,false,false,false,false,false,false,false};

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
						case 7:
							showRatingMenu = true;
							camVisitor.GetComponent<MouseLook> ().enabled = false;
							GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = false;
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
		if (Application.loadedLevel == 7 && showChoiceProfil) {
			windowProfilUser = GUI.Window (0, windowProfilUser, OptionsWindowFunctionProfil, "Options");
			camVisitor.GetComponent<MouseLook> ().enabled = false;
			GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = false;
		}
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
			case 6: // Scene_visit2
				windowRect5 = GUI.Window (0, windowRect5, OptionsWindowFunction5, "Options");
				break;
			case 7 :
				showChoiceProfil = true;
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
		if (showRatingMenu) {
			windowRatePainting = GUI.Window (0, windowRatePainting, OptionsWindowRatePainting, "Avis");
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

	void OptionsWindowFunction5 (int windowID) {
		GlobalOptions ();
		GUI.Label (new Rect (10, 95, 230, 42), "Choisissez les critères de fin d'observation : ");
		WatchFitness.timerCond = GUI.Toggle(new Rect(10,135,230,30), WatchFitness.timerCond, "Timer");
		WatchFitness.visitorNearCond = GUI.Toggle(new Rect(10,160,230,30), WatchFitness.visitorNearCond, "Visiteur à coté");
		WatchFitness.nbAgentsNearCond = GUI.Toggle(new Rect(10,185,230,30), WatchFitness.nbAgentsNearCond, "Nombre d'agents limite");
		GUI.DragWindow();
	}

	void OptionsWindowFunctionProfil (int windowID){
		GlobalOptions ();
		VisitorProfil pr = GameObject.FindObjectOfType<VisitorProfil> ();
		GUI.Label (new Rect (10, 95, 230, 42), "Choisissez les critères de fin d'observation : ");
		Watch.timerCond = GUI.Toggle(new Rect(10,135,230,30), Watch.timerCond, "Timer");
		Watch.visitorNearCond = GUI.Toggle(new Rect(10,160,230,30), Watch.visitorNearCond, "Visiteur à coté");
		Watch.nbAgentsNearCond = GUI.Toggle(new Rect(10,185,230,30), Watch.nbAgentsNearCond, "Nombre d'agents limite");
		int i = 100;
		GUI.Label (new Rect (10, 110+i, 230, 42), "Choisissez vos préférences : ");
		tagsToggles[0] = GUI.Toggle(new Rect(10,135+i,100,30), tagsToggles[0], "Femme");
		tagsToggles[1] = GUI.Toggle(new Rect(10,160+i,100,30), tagsToggles[1], "Combat");
		tagsToggles[2] = GUI.Toggle(new Rect(10,185+i,100,30), tagsToggles[2], "Animaux");
		tagsToggles[3] = GUI.Toggle(new Rect(10,210+i,100,30), tagsToggles[3], "Portrait");
		tagsToggles[4] = GUI.Toggle(new Rect(10,235+i,100,30), tagsToggles[4], "Science");
		tagsToggles[5] = GUI.Toggle(new Rect(10,260+i,100,30), tagsToggles[5], "Musique");
		tagsToggles[6] = GUI.Toggle(new Rect(10,285+i,100,30), tagsToggles[6], "Nature");

		tagsToggles[7] = GUI.Toggle(new Rect(125,135+i,100,30), tagsToggles[7], "Urbanisme");
		tagsToggles[8] = GUI.Toggle(new Rect(125,160+i,100,30), tagsToggles[8], "Baroque");
		tagsToggles[9] = GUI.Toggle(new Rect(125,185+i,100,30), tagsToggles[9], "Cubisme");
		tagsToggles[10] = GUI.Toggle(new Rect(125,210+i,100,30), tagsToggles[10], "Futurisme");
		tagsToggles[11] = GUI.Toggle(new Rect(125,235+i,100,30), tagsToggles[11], "Surrealisme");
		tagsToggles[12] = GUI.Toggle(new Rect(125,260+i,100,30), tagsToggles[12], "Romantisme");

		if (tagsToggles [0] && !pr.profil.Contains(Painting.Tags.Femme)) pr.profil.Add (Painting.Tags.Femme);
		else pr.profil.Remove(Painting.Tags.Femme);
		if (tagsToggles [1] && !pr.profil.Contains(Painting.Tags.Combat)) pr.profil.Add (Painting.Tags.Combat);
		else pr.profil.Remove(Painting.Tags.Combat);
		if (tagsToggles [2] && !pr.profil.Contains(Painting.Tags.Animaux)) pr.profil.Add (Painting.Tags.Animaux);
		else pr.profil.Remove(Painting.Tags.Animaux);
		if (tagsToggles [3] && !pr.profil.Contains(Painting.Tags.Portrait)) pr.profil.Add (Painting.Tags.Portrait);
		else pr.profil.Remove(Painting.Tags.Portrait);
		if (tagsToggles [4] && !pr.profil.Contains(Painting.Tags.Science)) pr.profil.Add (Painting.Tags.Science);
		else pr.profil.Remove(Painting.Tags.Science);
		if (tagsToggles [5] && !pr.profil.Contains(Painting.Tags.Musique)) pr.profil.Add (Painting.Tags.Musique);
		else pr.profil.Remove(Painting.Tags.Musique);
		if (tagsToggles [6] && !pr.profil.Contains(Painting.Tags.Nature)) pr.profil.Add (Painting.Tags.Nature);
		else pr.profil.Remove(Painting.Tags.Nature);
		if (tagsToggles [7] && !pr.profil.Contains(Painting.Tags.Urbanisme)) pr.profil.Add (Painting.Tags.Urbanisme);
		else pr.profil.Remove(Painting.Tags.Urbanisme);
		if (tagsToggles [8] && !pr.profil.Contains(Painting.Tags.Baroque)) pr.profil.Add (Painting.Tags.Baroque);
		else pr.profil.Remove(Painting.Tags.Baroque);
		if (tagsToggles [9] && !pr.profil.Contains(Painting.Tags.Cubisme)) pr.profil.Add (Painting.Tags.Cubisme);
		else pr.profil.Remove(Painting.Tags.Cubisme);
		if (tagsToggles [10] && !pr.profil.Contains(Painting.Tags.Futurisme)) pr.profil.Add (Painting.Tags.Futurisme);
		else pr.profil.Remove(Painting.Tags.Futurisme);
		if (tagsToggles [11] && !pr.profil.Contains(Painting.Tags.Surrealisme)) pr.profil.Add (Painting.Tags.Surrealisme);
		else pr.profil.Remove(Painting.Tags.Surrealisme);
		if (tagsToggles [12] && !pr.profil.Contains(Painting.Tags.Romantisme)) pr.profil.Add (Painting.Tags.Romantisme);
		else pr.profil.Remove(Painting.Tags.Romantisme);

		if(GUI.Button(new Rect(125,285+i,75,25),"Valider")){
			camVisitor.GetComponent<MouseLook> ().enabled = true;
			GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = true;
			showChoiceProfil = false;
			showOptions = false;
			pr.updateFitness();
		}
		GUI.DragWindow();
	}

	void OptionsWindowRatePainting (int windowID) {
		GUI.Label (new Rect (25,50,430,40), "Aimez-vous cette peinture ?");
		if (GUI.Button (new Rect (50,100,50,40), "Oui")) {
			showRatingMenu = false;
			camVisitor.GetComponent<MouseLook> ().enabled = true;
			GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = true;
			paintingHit.IsLiked();
		}
		if (GUI.Button (new Rect (150,100,50,40), "Non")) {
			showRatingMenu = false;
			camVisitor.GetComponent<MouseLook> ().enabled = true;
			GameObject.Find ("Visitor").GetComponent<VisitorMouvement> ().enabled = true;
		}
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
