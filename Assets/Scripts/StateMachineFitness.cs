using UnityEngine;
using System.Collections;

public class StateMachineFitness {
	
	public AgentWithFitnessBehaviour agent;
	States currentState;
	
	public StateMachineFitness(AgentWithFitnessBehaviour a){
		agent = a;
		ChangeState(new GoToFitness (this));
	}
	
	public void ChangeState(States state){
		currentState = state;
		currentState.Enter ();
	}
	
	public void Execute(){
		currentState.Execute ();
	}
}
