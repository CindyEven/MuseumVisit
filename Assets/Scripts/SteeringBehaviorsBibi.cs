using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringBehaviorsBibi : MonoBehaviour {
	
	public Vector3 seek;
	GameObject[] boids;
	List<GameObject> myBoids;
	public GameObject destination2;
	float distanceMin = 2.0f;
	float alignCoef = 0.12f;
	float cohesionCoef = 0.01f;
	float maxSpeed = 20.0f;
	public GameObject[] destinations;
	int i = 0;
	// Use this for initialization
	void Start () {
		//mainDirection = destination.transform.position - transform.position;
		boids = GameObject.FindGameObjectsWithTag("Player");
		myBoids = new List<GameObject> ();
		// penser à se supprimer de la liste
	}
	
	// Update is called once per frame
	void Update () {
		seek = (destinations[i].transform.position - transform.position).normalized*maxSpeed;
		seek = seek - (rigidbody.velocity);
		Vector3 coh=Vector3.zero, sep=Vector3.zero, al=Vector3.zero;
		//transform.Translate (mainDirection * 0.01f);
		if (myBoids.Count != 0) {
			coh = Cohesion (gameObject);
			sep = Separation (gameObject);
			al = Alignment (gameObject);
		}

		Vector3 force = 1.5f*seek + 1.2f*coh + 5.5f*sep + al;

		if (Vector3.Distance (transform.position, destinations[i].transform.position) > 6f) {
				transform.LookAt (transform.position+(rigidbody.velocity+(force/rigidbody.mass)*Time.deltaTime)*Time.deltaTime);
				rigidbody.AddForce(force);
				} else {
						if (i < destinations.Length - 1) {
								i++;
						} else {
								i = 0;
						}
				}
	}

	Vector3 Separation(GameObject me){
		Vector3 c = Vector3.zero;
		for (int y = 0; y < myBoids.Count ; y++) {
			c += ((me.transform.position - myBoids[y].transform.position).normalized)/Mathf.Abs(Vector3.Distance(myBoids[y].transform.position, me.transform.position));
		}
		return c;
	}

	Vector3 Alignment(GameObject me){
		Vector3 velocityAverage = Vector3.zero;
		for (int y = 0; y < myBoids.Count ; y++) {
			velocityAverage += myBoids[y].rigidbody.velocity;
		}
		velocityAverage = velocityAverage / (myBoids.Count);
		return (velocityAverage - me.rigidbody.velocity);
	}

	Vector3 Cohesion(GameObject me){
		Vector3 gravityCenter = Vector3.zero;
		for (int y = 0; y < myBoids.Count ; y++) {
				gravityCenter += myBoids[y].transform.position;
		}
		gravityCenter = gravityCenter / myBoids.Count;
		return ((((gravityCenter - me.transform.position).normalized * 20.0f)) - me.rigidbody.velocity);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			myBoids.Add (other.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player") {
			myBoids.Remove (other.gameObject);
		}
	}
}
