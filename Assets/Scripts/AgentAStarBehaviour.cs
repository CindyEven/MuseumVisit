using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentAStarBehaviour : MonoBehaviour {

	Point startPoint;
	Point targetPoint;
	Painting targetPainting;
	Painting[] listPainting;
	
	public Vector3 seek;
	float maxSpeed = 3.0f;
	public List<GameObject> visiteurs;
	public List<GameObject> visiteursWithSameDestination;

	int painting = 0;
	int i = 0;
	public List<Point> path = null;
	float speed = 2f;
	Point nextDestination;
	// Use this for initialization
	void Start () {
		listPainting = GameObject.FindObjectsOfType <Painting>();
		painting = (int)Random.Range(0, listPainting.Length-1);
		targetPainting = listPainting [painting];
		targetPoint = FindTheNearestPoint (targetPainting.gameObject);
		startPoint = FindTheNearestPoint (gameObject);
		path = AStarSearchWithLink.search(startPoint, targetPoint);
		nextDestination = path [i];
		visiteurs = new List<GameObject> ();
		visiteursWithSameDestination = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {

		if(Physics.Raycast (transform.position, nextDestination.transform.position - transform.position, Vector3.Distance(transform.position,nextDestination.transform.position))){
			startPoint = FindTheNearestPoint (gameObject);
			i=0;
			path = AStarSearchWithLink.search(startPoint, targetPoint);
			nextDestination = path [i];
		}

		Vector3 position = nextDestination.transform.position;
		/*Vector3 direction = new Vector3(position.x - transform.position.x, 0, position.z - transform.position.z);
		direction = direction.normalized;
		transform.Translate(direction * Time.deltaTime * speed);*/
		seek = (position - transform.position).normalized*maxSpeed;
		seek = seek - (rigidbody.velocity);
		Vector3 coh=Vector3.zero, sep=Vector3.zero, al=Vector3.zero;
		//transform.Translate (mainDirection * 0.01f);
		if (visiteurs.Count != 0) {
			sep = Separation (gameObject);
		}
		if(visiteursWithSameDestination.Count != 0){
			al = Alignment (gameObject);
			coh = Cohesion (gameObject);
		}
		Vector3 force = 1.25f*seek + 1.5f*sep + 0.25f*coh + al;
		Vector3 direction = transform.position + (rigidbody.velocity + (force / rigidbody.mass) * Time.deltaTime) * Time.deltaTime;
		transform.LookAt (new Vector3(direction.x,0,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0,force.z));
		
		if((position - transform.position).sqrMagnitude < 1.5f){
			if(i < path.Count-1){
				i++;
				nextDestination = path [i];
			}else{
				startPoint = FindTheNearestPoint (gameObject);
				painting = (int)Random.Range(0, listPainting.Length-1);
				targetPainting = listPainting [painting];
				targetPoint = FindTheNearestPoint (targetPainting.gameObject);
				i=0;
				path = AStarSearchWithLink.search(startPoint, targetPoint); 
				nextDestination = path [i];
			}
		}
	}
	
	Vector3 Separation(GameObject me){
		Vector3 c = Vector3.zero;
		for (int y = 0; y < visiteurs.Count ; y++) {
			if(visiteursWithSameDestination.Contains(visiteurs[y])){
				c += (((me.transform.position - visiteurs[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(visiteurs[y].transform.position, me.transform.position)));
			}else{
				c += 2.0f*(((me.transform.position - visiteurs[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(visiteurs[y].transform.position, me.transform.position)));
			}
		}
		return c;
	}
	
	Vector3 Alignment(GameObject me){
		Vector3 velocityAverage = Vector3.zero;
		for (int y = 0; y < visiteursWithSameDestination.Count ; y++) {
			velocityAverage += visiteursWithSameDestination[y].rigidbody.velocity;
		}
		velocityAverage = velocityAverage / (visiteursWithSameDestination.Count);
		return (velocityAverage - me.rigidbody.velocity);
	}
	
	Vector3 Cohesion(GameObject me){
		Vector3 gravityCenter = Vector3.zero;
		for (int y = 0; y < visiteursWithSameDestination.Count ; y++) {
			gravityCenter += visiteursWithSameDestination[y].transform.position;
		}
		gravityCenter = gravityCenter / visiteursWithSameDestination.Count;
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
		if (other.gameObject.tag == "Agent") {
			AgentAStarBehaviour a = other.gameObject.GetComponent<AgentAStarBehaviour>();
			if(targetPoint == a.targetPoint || nextDestination == a.nextDestination){
				if(!visiteursWithSameDestination.Contains(other.gameObject)){
					visiteursWithSameDestination.Add(other.gameObject);
				}
			}else{
				visiteursWithSameDestination.Remove(other.gameObject);
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
}

public static class AStarSearch{

	public static List<Point> search(Point start, Point target){

		Point currentPoint = start;
		float currentScoreTotal;
		List<Point> waypoints = new List<Point> ();
		List<Point> possibleWaypoints = new List<Point> ();
		List<Point> alreadyView = new List<Point> ();

		Point[] listPoints = GameObject.FindObjectsOfType <Point>();
		Vector3 currentPos = currentPoint.transform.position;
		Vector3 targetPos = target.transform.position;

		currentPoint.scoreG = 0;
		alreadyView.Add (start);

		if (currentPoint == target) {
			waypoints.Add (currentPoint);
			return waypoints;
		}

		if (IsAvailable(currentPos,targetPos)){

			waypoints.Add(currentPoint);
			waypoints.Add(target);
			return waypoints;
		}

		while(currentPoint != target){
			foreach (Point p in listPoints){

				if(!alreadyView.Contains(p)){
					Vector3 posPoint = p.transform.position;
					if (IsAvailable(currentPos,posPoint) && Vector3.Distance(currentPos,posPoint) < 25.0f){
						if(!possibleWaypoints.Contains(p)) {
							p.parent = currentPoint;
							p.scoreG = currentPoint.scoreG+Vector3.Distance(currentPos, posPoint);
							p.score = p.scoreG+Vector3.Distance(targetPos, posPoint);
							possibleWaypoints.Add(p);
						}else if(p.scoreG > (currentPoint.scoreG+Vector3.Distance(currentPos, posPoint))){
							p.parent = currentPoint;
							p.scoreG = currentPoint.scoreG+Vector3.Distance(currentPos, posPoint);
							p.score = p.scoreG+Vector3.Distance(targetPos, posPoint);
						}
					}
				}
			}

			int index = -1;
			currentScoreTotal = float.PositiveInfinity;
			foreach (Point p in possibleWaypoints){
				if(p.score < currentScoreTotal){
					currentScoreTotal = p.score;
					currentPoint = p;
					currentPos = currentPoint.transform.position;
					index = possibleWaypoints.IndexOf(p);
				}
			}
			alreadyView.Add(possibleWaypoints[index]);
			possibleWaypoints.RemoveAt(index);
			//Debug.Log(currentPoint.name);
		}

		while(currentPoint != start){
			waypoints.Add(currentPoint);
			currentPoint = currentPoint.parent;
		}
		waypoints.Add(currentPoint);
		waypoints.Reverse ();
		return waypoints;
	}

	private static bool IsAvailable(Vector3 A, Vector3 B){
		Vector3 right = new Vector3 (B.x + 1f, B.y, B.z + 1f);
		Vector3 left = new Vector3 (B.x - 1f, B.y, B.z - 1f);
		if (!Physics.Raycast (A, B - A, Vector3.Distance (A, B)+1)) {
			if (!Physics.Raycast (A, right - A, Vector3.Distance (A, B)+1.0f)) {
				if (!Physics.Raycast (A, left - A, Vector3.Distance (A, B)+1.0f)) {
					return true;
				}
			}
		}
		return false;
	}

}

public static class AStarSearchWithLink{
	
	public static List<Point> search(Point start, Point target){
		
		Point currentPoint = start;
		float currentScoreTotal;
		List<Point> waypoints = new List<Point> ();
		List<Point> possibleWaypoints = new List<Point> ();
		List<Point> alreadyView = new List<Point> ();

		Vector3 currentPos = currentPoint.transform.position;
		Vector3 targetPos = target.transform.position;
		
		currentPoint.scoreG = 0;
		alreadyView.Add (start);

		if (currentPoint == target) {
			waypoints.Add (currentPoint);
			return waypoints;
		}

		if (currentPoint.ConnectedTo.Contains(target)){		
			waypoints.Add(currentPoint);
			waypoints.Add(target);
			return waypoints;
		}

		while(currentPoint != target){
			foreach (Point p in currentPoint.ConnectedTo){
				
				if(!alreadyView.Contains(p)){
					Vector3 posPoint = p.transform.position;
					if(!possibleWaypoints.Contains(p)) {
						p.parent = currentPoint;
						p.scoreG = currentPoint.scoreG+Vector3.Distance(currentPos, posPoint);
						p.score = p.scoreG+Vector3.Distance(targetPos, posPoint);
						possibleWaypoints.Add(p);
					}else if(p.scoreG > (currentPoint.scoreG+Vector3.Distance(currentPos, posPoint))){
						p.parent = currentPoint;
						p.scoreG = currentPoint.scoreG+Vector3.Distance(currentPos, posPoint);
						p.score = p.scoreG+Vector3.Distance(targetPos, posPoint);
					}
				}
			}
			
			int index = -1;
			currentScoreTotal = float.PositiveInfinity;
			foreach (Point p in possibleWaypoints){
				if(p.score < currentScoreTotal){
					currentScoreTotal = p.score;
					currentPoint = p;
					currentPos = currentPoint.transform.position;
					index = possibleWaypoints.IndexOf(p);
				}
			}
			alreadyView.Add(possibleWaypoints[index]);
			possibleWaypoints.RemoveAt(index);
			//Debug.Log(currentPoint.name);
		}
		
		while(currentPoint != start){
			waypoints.Add(currentPoint);
			currentPoint = currentPoint.parent;
		}
		waypoints.Add(currentPoint);
		waypoints.Reverse ();
		return waypoints;
	}
	
	private static bool IsAvailable(Vector3 A, Vector3 B){
		Vector3 right = new Vector3 (B.x + 1f, B.y, B.z + 1f);
		Vector3 left = new Vector3 (B.x - 1f, B.y, B.z - 1f);
		if (!Physics.Raycast (A, B - A, Vector3.Distance (A, B)+1)) {
			if (!Physics.Raycast (A, right - A, Vector3.Distance (A, B)+1.0f)) {
				if (!Physics.Raycast (A, left - A, Vector3.Distance (A, B)+1.0f)) {
					return true;
				}
			}
		}
		return false;
	}
	
}
