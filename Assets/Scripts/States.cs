using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class States {
	abstract public void Enter ();
	abstract public void Execute ();
	abstract public void Exit ();
}

public class GoTo : States {

	StateMachine sm;
	Point startPoint;
	List<Point> path;
	int indexPath = 0;
	public GoTo (StateMachine s){
		sm = s;
	}

	public override void Enter(){
		// Find the next Painting to see
		sm.agent.targetPainting = RouletteWheelSelection.getAPainting (sm.agent.listPainting);
		sm.agent.targetPoint = sm.agent.FindTheNearestPoint (sm.agent.targetPainting.gameObject);
		startPoint = sm.agent.FindTheNearestPoint (sm.agent.gameObject);
		path = AStar.search(startPoint, sm.agent.targetPoint);
		sm.agent.nextDestination = path [indexPath];
	}

	public override void Execute(){
		// Go to see the Painting
		if(sm.agent.isBlock()){
			startPoint = sm.agent.FindTheNearestPoint (sm.agent.gameObject);
			indexPath=0;
			path = AStar.search(startPoint, sm.agent.targetPoint);
			sm.agent.nextDestination = path [indexPath];
		}
		
		Vector3 position = sm.agent.nextDestination.transform.position;
		
		sm.agent.force = sm.agent.steering.getForce (sm.agent.gameObject, position, sm.agent.visiteurs, sm.agent.visiteursWithSameDestination);
		sm.agent.direction = sm.agent.steering.getDirection (sm.agent.gameObject);
		
		if((position - sm.agent.transform.position).sqrMagnitude < sm.agent.distArrive){
			if(indexPath < path.Count-1){
				
				indexPath++;
				sm.agent.nextDestination = path [indexPath];
				
			}else{
				Exit ();
			}
		}
	}

	public override void Exit() {
//		sm.agent.targetPainting = RouletteWheelSelection.getAPainting2 (sm.agent.listPainting,sm.agent.paintingsFitness);
		sm.ChangeState(new Watch(sm));
	}
}

public class Watch : States {

	StateMachine sm;
	protected int timer;
	protected int nbAgents;
	protected bool VisitorNearMe;

	public static bool timerCond = true;
	public static bool visitorNearCond = false;
	public static bool nbAgentsNearCond = false;

	public Watch (StateMachine s){
		sm = s;
		timer = 0;
		nbAgents = 0;
		VisitorNearMe = false;
	}

	public override void Enter(){
		sm.agent.rigidbody.velocity = Vector3.zero;
		sm.agent.rigidbody.angularVelocity = Vector3.zero;
		sm.agent.force = Vector3.zero;
		sm.agent.direction = sm.agent.targetPainting.transform.position;
	}
	
	public override void Execute(){
		timer ++;
		Vector3 position = sm.agent.nextDestination.transform.position;
		sm.agent.force = sm.agent.steering.getForce (sm.agent.gameObject, position, sm.agent.visiteurs, sm.agent.visiteursWithSameDestination);
		nbAgents = sm.agent.visiteursWithSameDestination.Count;
		VisitorNearMe = sm.agent.visiteurs.Contains (GameObject.Find ("Visitor"));
		if(visitorNearCond && VisitorNearMe){
			Exit ();
		}else if(timerCond && timer > 800){
			Exit ();
		}else if(nbAgentsNearCond && nbAgents > 4){
			Exit ();
		}
	}
	
	public override void Exit() {
		sm.ChangeState (new GoTo (sm));
	}
}

public class Wait : States {

	StateMachine sm;
	Point meetingPoint;
	Vector3 position;
	public Wait (StateMachine s){
		sm = s;
	}

	public override void Enter(){
		meetingPoint = sm.agent.FindTheNearestMeetingPoint ();
		position = meetingPoint.transform.position;
	}
	
	public override void Execute(){

		if((position - sm.agent.transform.position).sqrMagnitude < sm.agent.distArrive){

		}else{
			sm.agent.force = sm.agent.steering.getForce (sm.agent.gameObject, position, sm.agent.visiteurs, sm.agent.visiteursWithSameDestination);
			sm.agent.direction = sm.agent.steering.getDirection (sm.agent.gameObject);
		}
	}
	
	public override void Exit() {
		
	}
}

public class GoToFitness : States {
	
	StateMachineFitness sm;
	Point startPoint;
	List<Point> path;
	int indexPath = 0;
	public GoToFitness (StateMachineFitness s){
		sm = s;
	}
	
	public override void Enter(){
		// Find the next Painting to see
		sm.agent.targetPainting = RouletteWheelSelection.getAPainting2 (sm.agent.listPainting,sm.agent.paintingsFitness);	
		sm.agent.targetPoint = sm.agent.FindTheNearestPoint (sm.agent.targetPainting.gameObject);
		startPoint = sm.agent.FindTheNearestPoint (sm.agent.gameObject);
		path = AStar.search(startPoint, sm.agent.targetPoint);
		sm.agent.nextDestination = path [indexPath];
	}
	
	public override void Execute(){
		// Go to see the Painting
		if(sm.agent.isBlock()){
			startPoint = sm.agent.FindTheNearestPoint (sm.agent.gameObject);
			indexPath=0;
			path = AStar.search(startPoint, sm.agent.targetPoint);
			sm.agent.nextDestination = path [indexPath];
		}
		
		Vector3 position = sm.agent.nextDestination.transform.position;
		
		sm.agent.force = sm.agent.steering.getForce (sm.agent.gameObject, position, sm.agent.visiteurs, sm.agent.visiteursWithSameDestination);
		sm.agent.direction = sm.agent.steering.getDirection (sm.agent.gameObject);
		
		if((position - sm.agent.transform.position).sqrMagnitude < sm.agent.distArrive){
			if(indexPath < path.Count-1){
				
				indexPath++;
				sm.agent.nextDestination = path [indexPath];
				
			}else{
				Exit ();
			}
		}
	}
	
	public override void Exit() {
		// ?
		sm.agent.updateFitness ();
		//		sm.agent.targetPainting = RouletteWheelSelection.getAPainting2 (sm.agent.listPainting,sm.agent.paintingsFitness);
		sm.ChangeState(new WatchFitness(sm));
	}
}

public class WatchFitness : States {
	
	StateMachineFitness sm;
	protected int timer;
	protected int nbAgents;
	protected bool VisitorNearMe;
	
	public static bool timerCond = true;
	public static bool visitorNearCond = false;
	public static bool nbAgentsNearCond = false;
	
	public WatchFitness (StateMachineFitness s){
		sm = s;
		timer = 0;
		nbAgents = 0;
		VisitorNearMe = false;
	}
	
	public override void Enter(){
		sm.agent.rigidbody.velocity = Vector3.zero;
		sm.agent.rigidbody.angularVelocity = Vector3.zero;
		sm.agent.force = Vector3.zero;
		sm.agent.direction = sm.agent.targetPainting.transform.position;
	}
	
	public override void Execute(){
		timer ++;
		Vector3 position = sm.agent.nextDestination.transform.position;
		sm.agent.force = sm.agent.steering.getForce (sm.agent.gameObject, position, sm.agent.visiteurs, sm.agent.visiteursWithSameDestination);
		nbAgents = sm.agent.visiteursWithSameDestination.Count;
		VisitorNearMe = sm.agent.visiteurs.Contains (GameObject.Find ("Visitor"));
		if(visitorNearCond && VisitorNearMe){
			Exit ();
		}else if(timerCond && timer > 800){
			Exit ();
		}else if(nbAgentsNearCond && nbAgents > 4){
			Exit ();
		}
	}
	
	public override void Exit() {
		sm.ChangeState (new GoToFitness (sm));
	}
}