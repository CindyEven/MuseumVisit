using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AgentBFSBehaviour : MonoBehaviour {
	
	public Point startPoint;
	public Point[] posPainting;

	public Vector3 seek;
	float maxSpeed = 6.0f;
	List<GameObject> visiteurs;

	Point targetPoint;
	int painting = 0;
	int i = 0;
	Point[] path = null;
	float speed = 2f;

	// Use this for initialization
	void Start () {
		painting = (int)Random.Range(0, 6);
		targetPoint = posPainting [painting];
		path = BreadthFirstSearch.search(startPoint, targetPoint).ToArray(); 
		visiteurs = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = path[i].transform.position;
		/*Vector3 direction = new Vector3(position.x - transform.position.x, 0, position.z - transform.position.z);
		direction = direction.normalized;
		transform.Translate(direction * Time.deltaTime * speed);*/
		seek = (position - transform.position).normalized*maxSpeed;
		seek = seek - (rigidbody.velocity);
		Vector3 coh=Vector3.zero, sep=Vector3.zero, al=Vector3.zero;
		//transform.Translate (mainDirection * 0.01f);
		if (visiteurs.Count != 0) {
			coh = Cohesion (gameObject);
			sep = Separation (gameObject);
			al = Alignment (gameObject);
		}
		
		Vector3 force = 1.5f*seek + 10.0f*sep + al + coh;
		Vector3 direction = transform.position + (rigidbody.velocity + (force / rigidbody.mass) * Time.deltaTime) * Time.deltaTime;
		transform.LookAt (new Vector3(direction.x,0,direction.z));
		rigidbody.AddForce(new Vector3(force.x,0,force.z));

		if((position - transform.position).sqrMagnitude < 5f){
			if(i < path.Length-1){
				i++;
			}else{
				startPoint = path[i];
				painting = (int)Random.Range(0, 6);
				targetPoint = posPainting [painting];
				i=0;
				path = BreadthFirstSearch.search(startPoint, targetPoint).ToArray(); 
			}
		}
	}

	Vector3 Separation(GameObject me){
		Vector3 c = Vector3.zero;
		for (int y = 0; y < visiteurs.Count ; y++) {
			c += ((me.transform.position - visiteurs[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(visiteurs[y].transform.position, me.transform.position));
		}
		return c;
	}
	
	Vector3 Alignment(GameObject me){
		Vector3 velocityAverage = Vector3.zero;
		for (int y = 0; y < visiteurs.Count ; y++) {
			velocityAverage += visiteurs[y].rigidbody.velocity;
		}
		velocityAverage = velocityAverage / (visiteurs.Count);
		return (velocityAverage - me.rigidbody.velocity);
	}
	
	Vector3 Cohesion(GameObject me){
		Vector3 gravityCenter = Vector3.zero;
		for (int y = 0; y < visiteurs.Count ; y++) {
			gravityCenter += visiteurs[y].transform.position;
		}
		gravityCenter = gravityCenter / visiteurs.Count;
		return ((((gravityCenter - me.transform.position).normalized * 6.0f)) - me.rigidbody.velocity);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Agent") {
			visiteurs.Add (other.gameObject);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Agent") {
			visiteurs.Remove (other.gameObject);
		}
	}

}

public static class BreadthFirstSearch
{
	public static List<Point> search(Point start, Point target)
	{
		Point targetFind = null;

		List<Point> waypoints = new List<Point> ();
		List<Point> alreadyView = new List<Point> ();
		start.parent = null;
		alreadyView.Add (start);

		if (start == target) {
			waypoints.Add (start);
			return waypoints;
		}

		Queue<Point> queue = new Queue<Point> ();
		Point[] children = start.ConnectedTo;

		foreach (Point p in children) {
			queue.Enqueue (p);
			alreadyView.Add (p);
			p.parent = start;
		}

		while (queue.Count > 0) {
			Point tmp = queue.Dequeue();

			if(tmp == target){
				targetFind = tmp;
				break;
			}

			children = tmp.ConnectedTo;

			foreach (Point p in children) {
				if(!alreadyView.Contains(p)){
					queue.Enqueue (p);
					alreadyView.Add (p);
					p.parent = tmp;
				}
			}
		}

		Point parent = targetFind.parent;
		while (parent != null) {
			waypoints.Add (targetFind);
			targetFind = parent;
			parent = targetFind.parent;
		}

		waypoints.Add (targetFind);
		waypoints.Reverse ();
		return waypoints;
	}

}