using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AgentSimpleBehaviour : MonoBehaviour {

	Point startPoint;
	Point targetPoint;
	Point nextDestination;
	
	public Painting targetPainting;
	
	List<Point> path;

	int indexPath = 0;

	public float maxSpeed = 4.0f;
	public float distArrive = 1.5f;
	public float slowDistance = 6.0f;
	
	// Use this for initialization
	void Start () {

		targetPoint = FindTheNearestPoint (targetPainting.gameObject);
		startPoint = FindTheNearestPoint (gameObject);
		path = AStar.search(startPoint, targetPoint);
		nextDestination = path [indexPath];
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 position = nextDestination.transform.position;

		Vector3 seek = Vector3.zero;
		float dist = Vector3.Distance (position, transform.position);
		
		if(dist > slowDistance){
			
			seek = (position - transform.position).normalized*maxSpeed;
		}else{
			
			seek = (position - transform.position).normalized*maxSpeed*(dist/slowDistance);
		}
		
		seek = seek - (rigidbody.velocity);

		Vector3 direction = Vector3.zero;
		direction = Vector3.zero;
		direction = transform.position + (rigidbody.velocity + (seek / rigidbody.mass) * Time.deltaTime) * Time.deltaTime;

		transform.LookAt (new Vector3(direction.x,1.0f,direction.z));
		rigidbody.AddForce(new Vector3(seek.x,0.0f,seek.z));
		
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
}
