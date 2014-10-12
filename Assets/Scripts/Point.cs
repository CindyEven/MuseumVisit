using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Point : MonoBehaviour {

	[SerializeField]
	public Point[] ConnectedTo;
	[SerializeField]
	public Point parent;
	[SerializeField]
	public Vector3 pointPosition;

	// Use this for initialization
	void Start () {
		pointPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
