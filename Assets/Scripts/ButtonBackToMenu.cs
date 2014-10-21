using UnityEngine;
using System.Collections;

public class ButtonBackToMenu : MonoBehaviour {

	void OnGUI(){
		if(GUI.Button(new Rect(Screen.width-60,10,50,30), "Menu")) {
			Application.LoadLevel("Scene_Menu");
		}
	}
}
