using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point : MonoBehaviour {

	public Point[] ConnectedTo;
	public Point parent;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		Debug.DrawLine (transform.position, parent.transform.position);
		foreach(Point connected in ConnectedTo){
			Debug.DrawLine(transform.position,connected.transform.position);
		}
	}
}
