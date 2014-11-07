﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class States {
	abstract public void Enter ();
	abstract public void Execute ();
	abstract public void Exit ();
}

public class GoTo : States {

	StateMachine sm;
	AgentStateMachineBehavior agent;
	Point startPoint;
	List<Point> path;
	int indexPath = 0;
	public GoTo (AgentStateMachineBehavior a, StateMachine s){
		agent = a;
		sm = s;
	}

	public override void Enter(){
		// Find the next Painting to see
		agent.targetPainting = RouletteWheelSelection.getAPainting (agent.listPainting);
		agent.targetPoint = agent.FindTheNearestPoint (agent.targetPainting.gameObject);
		startPoint = agent.FindTheNearestPoint (agent.gameObject);
		path = AStar.search(startPoint, agent.targetPoint);
		agent.nextDestination = path [indexPath];
	}

	public override void Execute(){
		// Go to see the Painting
		if(agent.isBlock()){
			startPoint = agent.FindTheNearestPoint (agent.gameObject);
			indexPath=0;
			path = AStar.search(startPoint, agent.targetPoint);
			agent.nextDestination = path [indexPath];
		}
		
		Vector3 position = agent.nextDestination.transform.position;
		
		agent.force = agent.steering.getForce (agent.gameObject, position, agent.visiteurs, agent.visiteursWithSameDestination);
		agent.direction = agent.steering.getDirection (agent.gameObject);
		
		if((position - agent.transform.position).sqrMagnitude < agent.distArrive){
			if(indexPath < path.Count-1){
				
				indexPath++;
				agent.nextDestination = path [indexPath];
				
			}else{
				Exit ();
			}
		}
	}

	public override void Exit() {
		// ?
		sm.ChangeState(new Watch(agent,sm));
	}
}

public class Watch : States {

	StateMachine sm;
	AgentStateMachineBehavior agent;
	protected int timer;
	protected int nbAgents;
	protected bool VisitorNearMe;

	public Watch (AgentStateMachineBehavior a, StateMachine s){
		agent = a;
		sm = s;
		timer = 0;
		nbAgents = 0;
		VisitorNearMe = false;
	}

	public override void Enter(){
		agent.rigidbody.velocity = Vector3.zero;
		agent.rigidbody.angularVelocity = Vector3.zero;
		agent.force = Vector3.zero;
		agent.direction = agent.targetPainting.transform.position;
	}
	
	public override void Execute(){
		timer ++;
		nbAgents = agent.visiteursWithSameDestination.Count;
		VisitorNearMe = agent.visiteurs.Contains (GameObject.Find ("Visitor"));

		if(VisitorNearMe){
			Exit ();
		}else if(timer > 800){
			Exit ();
		}else if(nbAgents > 25){
			Exit ();
		}
	}
	
	public override void Exit() {
		sm.ChangeState (new GoTo (agent, sm));
	}
}

public class Wait : States {

	StateMachine sm;
	AgentStateMachineBehavior agent;

	public Wait (AgentStateMachineBehavior a, StateMachine s){
		agent = a;
		sm = s;
	}

	public override void Enter(){
		
	}
	
	public override void Execute(){
		
	}
	
	public override void Exit() {
		
	}
}
