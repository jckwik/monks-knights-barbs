using UnityEngine;
using System.Collections;

public class Wander : MonoBehaviour {
	public float moveSpeed;
	public float rotationSpeed;
	//public GameObject target;
	public Vector3 direction;
	public float wanderR;
	public float wanderD;
	// Use this for initialization
	void Start () {
		rotationSpeed = 5;
		wanderR = 30.0f; // Radius for our "wander circle"
		wanderD = 100f; // Distance for our "wander circle"
	}
	
	// Update is called once per frame
	void Update () {
		float randomAngle = Random.Range (0, Mathf.PI*2);
		
		Vector3 circleLoc = transform.forward;
		circleLoc *= wanderD;
		circleLoc += transform.position;
		
		
		Vector3 circleOffSet = new Vector3(Mathf.Cos (randomAngle)*wanderR, 0, Mathf.Sin (randomAngle)*wanderR);
		Vector3 target = circleLoc + circleOffSet;

		
		direction = target - this.transform.position;
		this.transform.LookAt(target, Vector3.up);
		this.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
	}
}
