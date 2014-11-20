using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisitorProfil : MonoBehaviour {
	public List<Painting.Tags> profil;
	// Use this for initialization
	public void updateFitness () {
		Painting[] paintingList = GameObject.FindObjectsOfType <Painting>();
		foreach( Painting.Tags t in profil){
			foreach(Painting p in paintingList){
				if (p.tagList.Contains(t)){
					p.fitness += 500.0f;
				}
			}
		}
		AgentStateMachineBehavior[] agentList = GameObject.FindObjectsOfType <AgentStateMachineBehavior>();
		foreach(AgentStateMachineBehavior a in agentList){
			a.initFitness();
		}
	}

}
