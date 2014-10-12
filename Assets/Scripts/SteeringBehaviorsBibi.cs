using UnityEngine;
using System.Collections;

public class SteeringBehaviorsBibi : MonoBehaviour {
	
	public Vector3 seek;
	GameObject[] boids;
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
		// penser à se supprimer de la liste
	}
	
	// Update is called once per frame
	void Update () {
		seek = (destinations[i].transform.position - transform.position).normalized*maxSpeed;
		seek = seek - rigidbody.velocity;
		Vector3 coh, sep, v3;
		//transform.Translate (mainDirection * 0.01f);
		coh = Cohesion (gameObject);
		sep = Separation (gameObject);
		v3 = Alignment (gameObject);

		Vector3 force = 1.5f*seek + coh + sep ;

		if (Vector3.Distance (transform.position, destinations [i].transform.position) > 4f) {
						transform.LookAt (destinations[i].transform.position);
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
		foreach (GameObject boid in boids) {
			if(!boid.Equals(me)){
				c -= (boid.transform.position - me.transform.position).normalized * 20.0f/Vector3.Distance(boid.transform.position, me.transform.position);
			}
		}
		return c;
	}

	Vector3 Alignment(GameObject me){
		Vector3 velocityAverage = Vector3.zero;
		foreach (GameObject boid in boids) {
			if(!boid.Equals(me)){
				velocityAverage += boid.rigidbody.velocity;
			}
		}
		velocityAverage = velocityAverage / (boids.Length - 1);
		return (velocityAverage - me.rigidbody.velocity) * alignCoef;
	}

	Vector3 Cohesion(GameObject me){
		Vector3 gravityCenter = Vector3.zero;
		foreach (GameObject boid in boids) {
			if(!boid.Equals(me)){
				gravityCenter += boid.transform.position;
			}
		}
		gravityCenter = gravityCenter / (boids.Length - 1);
		return ((((gravityCenter - me.transform.position).normalized*20.0f)) - me.rigidbody.velocity)*Vector3.Distance(gravityCenter, me.transform.position);
	}
}
