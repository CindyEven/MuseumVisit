using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point : MonoBehaviour {

	public enum Type {
		Passage,
		Observation,
		Attente
	}

	public List<Point> ConnectedTo;
	public Point parent;
	public float scoreG;
	public float score;
	public Type typePoint;

	// Update is called once per frame
	void Update () {
		foreach(Point connected in ConnectedTo){
			Debug.DrawLine(transform.position,connected.transform.position,Color.grey);
		}
	}
}
