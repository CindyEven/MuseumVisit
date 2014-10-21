using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class AStar{
	
	public static List<Point> search(Point start, Point target){
		
		Point currentPoint = start;

		List<Point> waypoints = new List<Point> ();
		List<Point> possibleWaypoints = new List<Point> ();
		List<Point> alreadyView = new List<Point> ();
		
		Vector3 currentPos = currentPoint.transform.position;
		Vector3 targetPos = target.transform.position;

		float currentScoreTotal;

		currentPoint.scoreG = 0;
		alreadyView.Add (start);
		
		if (currentPoint == target) {
			waypoints.Add (currentPoint);
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
		}
		
		while(currentPoint != start){
			waypoints.Add(currentPoint);
			currentPoint = currentPoint.parent;
		}

		waypoints.Add(currentPoint);
		waypoints.Reverse ();
		return waypoints;
	}
}