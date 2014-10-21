using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentSimpleGroupBehaviour : MonoBehaviour {
	
	Point startPoint;
	Point targetPoint;
	Point nextDestination;
	
	public Painting targetPainting;
	
	List<Point> path;

	public List<GameObject> visiteurs;

	SteeringBehaviour steering;

	public Vector3 force;
	int indexPath = 0;

	public float distArrive = 2.0f;
	
	// Use this for initialization
	void Start () {

		steering = gameObject.GetComponent<SteeringBehaviour> ();

		visiteurs = new List<GameObject> ();

		targetPoint = FindTheNearestPoint (targetPainting.gameObject);
		startPoint = FindTheNearestPoint (gameObject);
		path = AStar.search(startPoint, targetPoint);
		nextDestination = path [indexPath];
	}
	
	// Update is called once per frame
	void Update () {

		if(isBlock()){
			startPoint = FindTheNearestPoint (gameObject);
			indexPath=0;
			path = AStar.search(startPoint, targetPoint);
			nextDestination = path [indexPath];
		}

		Vector3 position = nextDestination.transform.position;
		
		force = steering.getForce (gameObject, position, visiteurs, visiteurs);
		Vector3 direction = steering.getDirection (gameObject);

		transform.LookAt (new Vector3(direction.x,1.0f,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0.0f,force.z));
		
		if((position - transform.position).sqrMagnitude < distArrive){
			if(indexPath < path.Count-1){
				
				indexPath++;
				nextDestination = path [indexPath];
				
			}
		}
	}
	
	Point FindTheNearestPoint(GameObject go){
		
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

	bool isBlock(){
		Vector3 pos = transform.position;
		Vector3 destPos = nextDestination.transform.position;
		
		if(Physics.Raycast (pos, destPos - pos, Vector3.Distance(pos,destPos))){
			
			return true;
		}
		
		return false;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Agent" && !gameObject.Equals(other.gameObject)) {
			
			if(!visiteurs.Contains(other.gameObject)){
				
				visiteurs.Add (other.gameObject);
			}
		}
		
		if (other.gameObject.tag == "Agent"){
			
			if(!visiteurs.Contains(other.gameObject)){
				
				visiteurs.Add (other.gameObject);
			}
		}
	}
	
	void OnTriggerExit(Collider other) {
		
		if (other.gameObject.tag == "Agent") {
			
			visiteurs.Remove (other.gameObject);
		}
		
		if (other.gameObject.tag == "Player"){
			
			visiteurs.Remove (other.gameObject);
		}
	}
}