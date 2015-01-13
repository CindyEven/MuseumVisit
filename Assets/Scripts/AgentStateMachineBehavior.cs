using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentStateMachineBehavior : MonoBehaviour {

	public Point nextDestination;
	public Point targetPoint;
	//	Point previousDestination;
	
	public Painting targetPainting;
	public Painting[] listPainting;
	
	public List<GameObject> visiteurs;
	public List<GameObject> visiteursWithSameDestination;

	public Vector3 force;
	public Vector3 direction;

	public SteeringBehaviour steering;
	
	public float distArrive = 1.5f;

	StateMachine sm;

	// Use this for initialization
	void Start () {

		//targetPainting = RouletteWheelSelection.getAPainting2 (listPainting,paintingsFitness);

		steering = gameObject.GetComponent<SteeringBehaviour> ();
		
		listPainting = GameObject.FindObjectsOfType <Painting>();
		visiteurs = new List<GameObject> ();
		visiteursWithSameDestination = new List<GameObject> ();

		sm = new StateMachine (this);

		force = Vector3.zero;
		direction = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		sm.Execute ();
		transform.LookAt (new Vector3(direction.x,1.0f,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0.0f,force.z));
	}
	
	public Point FindTheNearestPoint(GameObject go){
		
		Point[] listPoints = GameObject.FindObjectsOfType <Point>();
		Point nearPoint = null;
		float distMin = float.PositiveInfinity;
		
		foreach (Point p in listPoints) {
			
			float dist = Vector3.Distance(p.transform.position,go.transform.position);
			
			if(dist < distMin && !Physics.Raycast (go.transform.position, p.transform.position - go.transform.position, dist)){
				
				distMin = dist;
				nearPoint = p;
			}
		}
		
		return nearPoint;
	}

	public Point FindTheNearestMeetingPoint(){

		GameObject go = gameObject;
		Point[] listPoints = GameObject.FindObjectsOfType <Point>();
		Point nearPoint = null;
		float distMin = float.PositiveInfinity;
		
		foreach (Point p in listPoints) {

			if(p.typePoint == Point.Type.Attente){

				float dist = Vector3.Distance(p.transform.position,go.transform.position);
			
				if(dist < distMin && !Physics.Raycast (go.transform.position, p.transform.position - go.transform.position, dist)){
				
					distMin = dist;
					nearPoint = p;
				}
			}
		}
		
		return nearPoint;
	}
	
	public bool isBlock(){
		Vector3 pos = transform.position;
		Vector3 destPos = nextDestination.transform.position;
		
		if(Physics.Raycast (pos, destPos - pos, Vector3.Distance(pos,destPos))){
			
			return true;
		}
		
		return false;
	}
	

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Agent" && !gameObject.Equals(other.gameObject) && isVisible(gameObject, other.gameObject)) {
			
			if(!visiteurs.Contains(other.gameObject)){
				
				visiteurs.Add (other.gameObject);
			}
			
			AgentStateMachineBehavior a = other.gameObject.GetComponent<AgentStateMachineBehavior>();
			
			if(targetPoint == a.targetPoint || nextDestination == a.nextDestination){
				
				if(!visiteursWithSameDestination.Contains(other.gameObject)){
					
					visiteursWithSameDestination.Add(other.gameObject);
				}
			}
		}
		
		if (other.gameObject.tag == "Player" && isVisible(gameObject, other.gameObject)){
			
			if(!visiteurs.Contains(other.gameObject)){
				
				visiteurs.Add (other.gameObject);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if (other.gameObject.tag == "Agent") {
			
			visiteurs.Remove (other.gameObject);
			visiteursWithSameDestination.Remove(other.gameObject);
		}
		
		if (other.gameObject.tag == "Player"){
			
			visiteurs.Remove (other.gameObject);
		}
	}

	bool isVisible(GameObject me, GameObject target){
		
		Vector3 myPosition = me.transform.position;
		Vector3 targetPosition = target.transform.position;
		float dist = Vector3.Distance (targetPosition, myPosition);
		
		if(!Physics.Raycast (myPosition, targetPosition - myPosition, dist)){
			
			return true;
		}
		
		return false;
	}
}
