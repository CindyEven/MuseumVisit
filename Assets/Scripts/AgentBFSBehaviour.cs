using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AgentBFSBehaviour : MonoBehaviour {
	
	public Point startPoint;
	public Point[] posPainting;

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
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = path[i].transform.position;
		Vector3 direction = new Vector3(position.x - transform.position.x, 0, position.z - transform.position.z);
		direction = direction.normalized;
		transform.Translate(direction * Time.deltaTime * speed);
		if((position - transform.position).sqrMagnitude < 0.2f){
			if(i	<	path.Length-1){
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