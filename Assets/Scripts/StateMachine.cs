using UnityEngine;
using System.Collections;

public class StateMachine {

	AgentStateMachineBehavior agent;
	States currentState;

	public StateMachine(AgentStateMachineBehavior a){
		agent = a;
		ChangeState(new GoTo (a, this));
	}

	public void ChangeState(States state){
		currentState = state;
		currentState.Enter ();
	}

	public void Execute(){
		currentState.Execute ();
	}
}
