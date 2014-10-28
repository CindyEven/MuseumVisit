using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentBehaviour : MonoBehaviour {

	Point startPoint;
	Point targetPoint;
	Point nextDestination;
//	Point previousDestination;

	Painting targetPainting;
	Painting[] listPainting;

	List<GameObject> visiteurs;
	List<GameObject> visiteursWithSameDestination;

	List<Point> path;

	int indexPainting = 0;
	int indexPath = 0;
	
	SteeringBehaviour steering;

	public float distArrive = 1.5f;
	// Use this for initialization
	void Start () {

		steering = gameObject.GetComponent<SteeringBehaviour> ();

		/*listPainting = GameObject.FindObjectsOfType <Painting>();
		indexPainting = (int)Random.Range(0, listPainting.Length-1);
		targetPainting = listPainting [indexPainting];*/
		targetPainting = RouletteWheelSelection.getAPainting ();
		targetPoint = FindTheNearestPoint (targetPainting.gameObject);
		startPoint = FindTheNearestPoint (gameObject);
		path = AStar.search(startPoint, targetPoint);
		nextDestination = path [indexPath];

		visiteurs = new List<GameObject> ();
		visiteursWithSameDestination = new List<GameObject> ();
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

		Vector3 force = steering.getForce (gameObject, position, visiteurs, visiteursWithSameDestination);
		Vector3 direction = steering.getDirection (gameObject);
		transform.LookAt (new Vector3(direction.x,1.0f,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0.0f,force.z));

		if((position - transform.position).sqrMagnitude < distArrive){
			if(indexPath < path.Count-1){

				indexPath++;
//				previousDestination = nextDestination;
				nextDestination = path [indexPath];
				
			}else{

				indexPath=0;

				/*indexPainting = (int)Random.Range(0, listPainting.Length-1);
				targetPainting = listPainting [indexPainting];*/
				targetPainting = RouletteWheelSelection.getAPainting ();

				startPoint = FindTheNearestPoint (gameObject);
				targetPoint = FindTheNearestPoint (targetPainting.gameObject);

				path = AStar.search(startPoint, targetPoint); 
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

			AgentBehaviour a = other.gameObject.GetComponent<AgentBehaviour>();

			if(targetPoint == a.targetPoint || nextDestination == a.nextDestination){

				if(!visiteursWithSameDestination.Contains(other.gameObject)){

					visiteursWithSameDestination.Add(other.gameObject);
				}
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
			visiteursWithSameDestination.Remove(other.gameObject);
		}

		if (other.gameObject.tag == "Player"){

			visiteurs.Remove (other.gameObject);
		}
	}
}
