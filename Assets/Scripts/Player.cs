using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public KeyCode moveForward;
	public KeyCode moveBackward;
	public KeyCode moveLeft;
	public KeyCode moveRight;

	public float speed;
	public Vector3 direction;
	
	//public GameObject mainCam;

	Vector2 location;
	Vector2 velocity;
	Vector2 acceleration;
	
	// Use this for initialization
	void Start () {
		location = Vector2.zero;
		velocity = Vector2.zero;
		acceleration = Vector2.zero;
		//mainCam = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
		//float velX = 0.0f;
		//float velZ = 0.0f;
		direction = this.transform.forward;
		
		// Check for up and down movement, else float
		if(Input.GetKey(moveForward)){
			this.transform.Translate(direction * speed * Time.deltaTime);
		}
		else if(Input.GetKey(moveBackward)){
			this.transform.Translate(direction * -1f * speed * Time.deltaTime);
		}
		
		// Check for left and right movement
		if(Input.GetKey(moveRight)){
			this.transform.Translate(Vector3.Cross(-direction, Vector3.up) * speed * Time.deltaTime);
		}
		else if(Input.GetKey(moveLeft)){
			this.transform.Translate(Vector3.Cross(direction, Vector3.up) * speed * Time.deltaTime);;
		}

		//transform.position.y = 1;
		this.transform.LookAt(direction);
		//location += velocity;
		//velocity += acceleration;
	}
}
