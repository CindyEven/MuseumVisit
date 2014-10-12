using UnityEngine;
using System.Collections;

public class SteerinBehaviors : MonoBehaviour {

	public GameObject destination;
	public Vector3 mainDirection;
	GameObject[] boids;
	public float distanceMin = 1.0f;
	public float alignCoef = 0.12f;
	public float cohesionCoef = 0.01f;

	// Use this for initialization
	void Start () {
		//mainDirection = destination.transform.position - transform.position;
		boids = GameObject.FindGameObjectsWithTag("Player");
		// penser à se supprimer de la liste
	}
	
	// Update is called once per frame
	void Update () {
		mainDirection = destination.transform.position - transform.position;
		Vector3 v1, v2, v3;
		//transform.Translate (mainDirection * 0.01f);
		v1 = Cohesion (gameObject);
		v2 = Separation (gameObject);
		v3 = Alignment (gameObject);

		Vector3 newDirection = mainDirection + v1 + v2 + v3;

		if (Vector3.Distance (transform.position, destination.transform.position) > 1f) {
			//transform.LookAt (destination.transform.position);
			transform.Translate (newDirection * Time.deltaTime, Space.World);
		}
	}

	Vector3 Separation(GameObject me){
		Vector3 c = Vector3.zero;
		foreach (GameObject boid in boids) {
			if(!boid.Equals(me)){
				if(Vector3.Distance(me.transform.position,boid.transform.position)<distanceMin){
					c -= (boid.transform.position - me.transform.position);
				}
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
		return (gravityCenter - me.transform.position) * cohesionCoef;
	}
}
