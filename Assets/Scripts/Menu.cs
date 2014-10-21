﻿using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,10,Screen.width-20,Screen.height-20), "Choisissez une question : ");
		
		// Make the first button. If it is pressed, it will show the answer of the first question.
		if(GUI.Button(new Rect(20,40,Screen.width-40,30), "1) Faire se déplacer un visiteur vituel")) {
			Application.LoadLevel("Scene_q1");
		}
		
		// Make the second button.
		if(GUI.Button(new Rect(20,80,Screen.width-40,30), "2) Faire se déplacer un groupe de visiteurs vers UN tableau")) {
			Application.LoadLevel("Scene_q2");
		}

		// Make the third button.
		if(GUI.Button(new Rect(20,120,Screen.width-40,30), "3) a) Faire se déplacer un groupe de visiteurs vers DES tableaux (meme probabilité)")) {
			Application.LoadLevel("Scene_q3a");
		}

		// Make the fourth button.
		if(GUI.Button(new Rect(20,160,Screen.width-40,30), "3) b) Faire se déplacer un groupe de visiteurs vers DES tableaux (probabilité différentes)")) {
			Application.LoadLevel("Scene_q3b");
		}

		// Make the fifth button.
		if(GUI.Button(new Rect(20,200,Screen.width-40,30), "4) Créer une visite (1)")) {
			Application.LoadLevel("Scene_visit1");
		}

		// Make the sixth button.
		if(GUI.Button(new Rect(20,240,Screen.width-40,30), "5) Créer une visite (2)")) {
			Application.LoadLevel("Scene_visit2");
		}

		// Make the seventh button.
		if(GUI.Button(new Rect(20,280,Screen.width-40,30), "6) Créer une visite (3)")) {
			Application.LoadLevel("Scene_visit3");
		}
	}
}
