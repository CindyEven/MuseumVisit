using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		List<Point> children = start.ConnectedTo;
		
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
