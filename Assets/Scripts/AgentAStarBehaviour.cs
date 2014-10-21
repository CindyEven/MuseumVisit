using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentAStarBehaviour : MonoBehaviour {

	Point startPoint;
	Point targetPoint;

	Painting targetPainting;
	Painting[] listPainting;

	public Painting destination;
	public Vector3 seek;
	float maxSpeed = 3.0f;
	public List<GameObject> visiteurs;
	public List<GameObject> visiteursWithSameDestination;

	int painting = 0;
	int i = 0;
	public List<Point> path = null;
	float speed = 2f;
	Point nextDestination;
	Point previousDestination;
	int time = 0;
	// Use this for initialization
	void Start () {
		listPainting = GameObject.FindObjectsOfType <Painting>();
		painting = (int)Random.Range(0, listPainting.Length-1);
		targetPainting = listPainting [painting];
		//targetPoint = FindTheNearestPoint (targetPainting.gameObject);*
		targetPoint = FindTheNearestPoint (destination.gameObject);
		startPoint = FindTheNearestPoint (gameObject);
		path = AStar.search(startPoint, targetPoint);
		nextDestination = path [i];
		visiteurs = new List<GameObject> ();
		visiteursWithSameDestination = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		time++;
		if(Physics.Raycast (transform.position, nextDestination.transform.position - transform.position, Vector3.Distance(transform.position,nextDestination.transform.position))){
			startPoint = FindTheNearestPoint (gameObject);
			i=0;
			path = AStar.search(startPoint, targetPoint);
			nextDestination = path [i];
		}
		/*Point tmp = Follow ();
		if(tmp.gameObject.name != nextDestination.gameObject.name){
			Debug.Log("Follow");
			previousDestination = nextDestination;
			nextDestination = tmp;
			i++;
		}*/
		Vector3 position = nextDestination.transform.position;
		seek = (position - transform.position).normalized*maxSpeed;
		seek = seek - (rigidbody.velocity);
		Vector3 coh=Vector3.zero, sep=Vector3.zero, al=Vector3.zero, avoid=Vector3.zero;
		if (visiteurs.Count != 0) {
			sep = Separation (gameObject);
		}
		if(visiteursWithSameDestination.Count != 0){
			al = Alignment (gameObject);
			coh = Cohesion (gameObject);
		}
		Vector3 force = seek + sep + coh + al;

		Vector3 direction = transform.position + (rigidbody.velocity + (force / rigidbody.mass) * Time.deltaTime) * Time.deltaTime;

		/*avoid = Avoidance (transform.position, direction);
		if(avoid != Vector3.zero){
			force += avoid;
			direction = transform.position + (rigidbody.velocity + (force / rigidbody.mass) * Time.deltaTime) * Time.deltaTime;
		}*/

		transform.LookAt (new Vector3(direction.x,1.0f,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0,force.z));
		
		if((position - transform.position).sqrMagnitude < 6f){
			if(i < path.Count-1){
				i++;
				previousDestination = nextDestination;
				nextDestination = path [i];
				time =0;
			}else{
				/*startPoint = FindTheNearestPoint (gameObject);
				painting = (int)Random.Range(0, listPainting.Length-1);
				targetPainting = listPainting [painting];
				targetPoint = FindTheNearestPoint (targetPainting.gameObject);
				i=0;
				path = AStarSearchWithLink.search(startPoint, targetPoint); 
				nextDestination = path [i];*/
			}
		}
	}
	
	Vector3 Separation(GameObject me){
		Vector3 c = Vector3.zero;
		for (int y = 0; y < visiteurs.Count ; y++) {
			if(!Physics.Raycast (me.transform.position, visiteurs[y].transform.position - me.transform.position, Vector3.Distance(visiteurs[y].transform.position, me.transform.position))){
				if(visiteursWithSameDestination.Contains(visiteurs[y])){
					c += (((me.transform.position - visiteurs[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(visiteurs[y].transform.position, me.transform.position)));
				}else{
					c += (((me.transform.position - visiteurs[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(visiteurs[y].transform.position, me.transform.position)));
				}
			}
		}
		return c;
	}
	
	Vector3 Alignment(GameObject me){
		Vector3 velocityAverage = Vector3.zero;
		for (int y = 0; y < visiteursWithSameDestination.Count ; y++) {
			if(!Physics.Raycast (me.transform.position, visiteursWithSameDestination[y].transform.position - me.transform.position, Vector3.Distance(visiteursWithSameDestination[y].transform.position, me.transform.position))){
				velocityAverage += visiteursWithSameDestination[y].rigidbody.velocity;
			}
		}
		velocityAverage = velocityAverage / (visiteursWithSameDestination.Count);
		return (velocityAverage - me.rigidbody.velocity);
	}
	
	Vector3 Cohesion(GameObject me){
		Vector3 gravityCenter = Vector3.zero;
		int i = 0;
		for (int y = 0; y < visiteursWithSameDestination.Count ; y++) {
			if(!Physics.Raycast (me.transform.position, visiteursWithSameDestination[y].transform.position - me.transform.position, Vector3.Distance(visiteursWithSameDestination[y].transform.position, me.transform.position))){
				gravityCenter += visiteursWithSameDestination[y].transform.position;
				i++;
			}
		}
		gravityCenter = gravityCenter / i;
		return ((((gravityCenter - me.transform.position).normalized * 6.0f)) - me.rigidbody.velocity);
	}
	
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Agent" && !gameObject.Equals(other.gameObject)) {
			if(!visiteurs.Contains(other.gameObject)){
				visiteurs.Add (other.gameObject);
			}
			AgentAStarBehaviour a = other.gameObject.GetComponent<AgentAStarBehaviour>();
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

	void OnTriggerStay(Collider other){
		/*if (other.gameObject.tag == "Agent") {
			AgentAStarBehaviour a = other.gameObject.GetComponent<AgentAStarBehaviour>();
			if(targetPoint == a.targetPoint || nextDestination == a.nextDestination){
				if(!visiteursWithSameDestination.Contains(other.gameObject)){
					visiteursWithSameDestination.Add(other.gameObject);
				}
			}else{
				visiteursWithSameDestination.Remove(other.gameObject);
			}
		}*/
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

	Vector3 Avoidance(Vector3 A, Vector3 B){
		Vector3 force = Vector3.zero;
		Vector3 right = new Vector3 (B.x + 1f, B.y, B.z + 1f);
		Vector3 left = new Vector3 (B.x - 1f, B.y, B.z - 1f);
		if (Physics.Raycast (A, B - A, 1.0f)) {
			force += 0.2f*((A - B).normalized)/Vector3.Distance(B, A);
		}
		if (Physics.Raycast (A, right - A, 1.0f)) {
			force += 0.2f*((A - right).normalized)/Vector3.Distance(right, A);
		}
		if (Physics.Raycast (A, left - A, 1.0f)) {
			force += 0.2f*((A - left).normalized)/Vector3.Distance(left, A);
		}
		return force;
	}

	Point Follow (){
		for (int y = 0; y < visiteursWithSameDestination.Count ; y++) {
			AgentAStarBehaviour a = visiteursWithSameDestination[y].GetComponent<AgentAStarBehaviour>();
			if(nextDestination == a.previousDestination &&!Physics.Raycast (transform.position, a.previousDestination.transform.position - transform.position, Vector3.Distance(a.previousDestination.transform.position, transform.position))){
				return a.nextDestination;
			}
		}
		return nextDestination;
	}
}
