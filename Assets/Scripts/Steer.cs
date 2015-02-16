// The Steer component has a collection of functions
// that return forces for steering 

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class Steer : MonoBehaviour
{
	Vector3 dv = Vector3.zero; 	// desired velocity, used in calculations
	CharacterController characterController;
	float maxSpeed = 2.0f;

	void Start ()
	{
		GameObject main = GameObject.Find("MainGO");
		characterController = gameObject.GetComponent<CharacterController> ();	
	}

	public Vector3 Seek (Vector3 targetPos)
	{
		//find dv, desired velocity
		dv = targetPos - transform.position;		
		dv = dv.normalized * maxSpeed; 	//scale by maxSpeed
		dv -= characterController.velocity;
		dv.y = 0;
		return dv;
	}
	
	public Vector3 Flee(Vector3 targetPos)
	{
		dv = transform.position - targetPos;		
		dv = dv.normalized * maxSpeed; 	//scale by maxSpeed
		dv -= characterController.velocity;
		dv.y = 0;								// only steer in the x/z plane
		return dv;
	}
	
	public Vector3 Wander()
	{
		float wanderR = 80.0f;         // Radius for our "wander circle"
		float wanderD = 100f;         // Distance for our "wander circle"
		
		
		float randomAngle = Random.Range (0, Mathf.PI*2);
		
		Vector3 circleLoc = transform.forward;
		circleLoc *= wanderD;
		circleLoc += transform.position;
		
		
		Vector3 circleOffSet = new Vector3(Mathf.Cos (randomAngle)*wanderR, 0, Mathf.Sin (randomAngle)*wanderR);
		Vector3 target = circleLoc + circleOffSet;
		
		return Seek(target);
	}
}
