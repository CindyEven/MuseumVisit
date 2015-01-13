using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SteeringBehaviour : MonoBehaviour {

	Vector3 force;
	Vector3 direction;

	static float seekCoef = 3.0f;
	static float sepCoef = 2.5f;
	static float cohCoef = 1.0f;
	static float alCoef = 1.0f;

	public float maxSpeed = 4.0f;
	public float slowDistance = 4.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public Vector3 getForce(GameObject me, Vector3 pos, List<GameObject> visiteurs, List<GameObject> visiteursWithSameDestination){

		force = Vector3.zero;

		force += seekCoef * Seek (me, pos);

		if(visiteurs.Count > 0){

			force -= sepCoef * Separation(me,visiteurs);
		}

		if(visiteursWithSameDestination.Count > 0){
			
			force += cohCoef * Cohesion(me,visiteursWithSameDestination);
			force += alCoef * Alignment(me,visiteursWithSameDestination);
		}

		return force;
	}

	public Vector3 getDirection(GameObject me){

		direction = Vector3.zero;
		direction = me.transform.position + (me.rigidbody.velocity + (force / me.rigidbody.mass) * Time.deltaTime) * Time.deltaTime;
		return direction;
	}

	Vector3 Seek(GameObject me, Vector3 pos){
		Vector3 seek = Vector3.zero;
		float dist = Vector3.Distance (pos, me.transform.position);

		if(dist > slowDistance){

			seek = (pos - transform.position).normalized*maxSpeed;
		}else{

			seek = (pos - transform.position).normalized*maxSpeed*(dist/slowDistance);
		}

		seek = seek - (rigidbody.velocity);
		return seek;
	}

	Vector3 Separation(GameObject me,List<GameObject> visiteurs){

		Vector3 c = Vector3.zero;
		Vector3 f = Vector3.zero;

		for (int y = 0; y < visiteurs.Count ; y++) {

			if(isVisible(me,visiteurs[y])){

				f = (visiteurs[y].transform.position - me.transform.position ).normalized;
				c += f/Vector3.Distance(visiteurs[y].transform.position, me.transform.position);
			}
		}

		return c;
	}
	
	Vector3 Alignment(GameObject me, List<GameObject> visiteurs){

		Vector3 velocityAverage = Vector3.zero;
		int nbTarget = 0;

		for (int y = 0; y < visiteurs.Count ; y++) {

			if(isVisible(me,visiteurs[y])){

				velocityAverage += visiteurs[y].rigidbody.velocity;
				nbTarget++;
			}
		}
		if (nbTarget != 0) {
			velocityAverage = velocityAverage / nbTarget;
			return (velocityAverage - me.rigidbody.velocity);
		} else {
			return velocityAverage;
		}
	}
	
	Vector3 Cohesion(GameObject me,List<GameObject> visiteurs){

		Vector3 gravityCenter = Vector3.zero;
		int nbTarget = 0;

		for (int y = 0; y < visiteurs.Count ; y++) {

			if(isVisible(me,visiteurs[y])){

				gravityCenter += visiteurs[y].transform.position;
				nbTarget++;
			}
		}
		if (nbTarget != 0){
			gravityCenter = gravityCenter / nbTarget;
			return ((((gravityCenter - me.transform.position).normalized * maxSpeed)) - me.rigidbody.velocity);
		}else{
			return gravityCenter;
		}
	}

	bool isVisible(GameObject me, GameObject target){

		Vector3 myPosition = me.transform.position;
		Vector3 targetPosition = target.transform.position;
		float dist = Vector3.Distance (targetPosition, myPosition);

		if(!Physics.Raycast (myPosition, targetPosition - myPosition, dist)){

			return true;
		}

		return false;
	}

	public static void updateSteeringCoeff(float seek, float sep, float coh, float al){
		seekCoef = seek;
		sepCoef = sep;
		cohCoef = coh;
		alCoef = al;
	}
	
}
