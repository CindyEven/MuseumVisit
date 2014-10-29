using UnityEngine;
using System.Collections;

public class VisitorMouvement : MonoBehaviour {

	public float speed = 6f;					// The speed that the visitor will move at.
	public float sensitivityX = 15F;
	
	private Vector3 movement;                   // The vector to store the direction of the player's movement.
	
	void FixedUpdate ()
	{
		// Store the input axes.
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		// Move the player around the scene.
		Move (h, v);
	}

	void Update ()
	{
		transform.Rotate(0f, Input.GetAxis("Mouse X") * sensitivityX, 0f);
	}
	
	void Move (float h, float v)
	{
		// Set the movement vector based on the axis input.
		movement.Set (h, 0f, v);
		// Normalise the movement vector and make it proportional to the speed per second.
		movement = movement.normalized * speed * Time.deltaTime;
		// Move the player to it's current position plus the movement.
		transform.Translate (movement, Space.Self);
	}

}
