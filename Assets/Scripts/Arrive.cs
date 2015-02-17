using UnityEngine;
using System.Collections;

public class Arrive : MonoBehaviour {
	public float moveSpeed;
	public float slowDistance;
	public float arriveDistance;
	public float rotationSpeed;
	public GameObject target;
	public Vector3 direction;
	
	// Use this for initialization
	void Start () {
		moveSpeed = 10;
		rotationSpeed = 5;
		slowDistance = 30;
		arriveDistance = 5;
	}
	
	// Update is called once per frame
	void Update () {
		direction = target.transform.position - this.transform.position;
		this.transform.LookAt(target.transform, Vector3.up);

		float mag = direction.magnitude;
		mag = Mathf.Abs(mag);
		float speed;
		// if it is close enough, don't move
		if (mag < arriveDistance) { return; }
		else if (mag > slowDistance) { speed = moveSpeed; }
		else {
			float x = mag / (slowDistance - arriveDistance);
			speed = moveSpeed * x;
		}
		this.transform.Translate(Vector3.forward * speed * Time.deltaTime);
	}
}
