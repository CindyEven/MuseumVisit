using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point : MonoBehaviour {

	public List<Point> ConnectedTo;
	public Point parent;
	public float scoreG;
	public float score;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		foreach(Point connected in ConnectedTo){
			Debug.DrawLine(transform.position,connected.transform.position);
		}
	}
}
