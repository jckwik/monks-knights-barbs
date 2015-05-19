using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	public KeyCode moveForward = KeyCode.W;
	public KeyCode moveBackward = KeyCode.S;
	public KeyCode moveLeft = KeyCode.A;
	public KeyCode moveRight = KeyCode.D;
	public KeyCode turnLeft = KeyCode.LeftArrow;
	public KeyCode turnRight = KeyCode.RightArrow;

	public float speed = 5.0f;
	public float turnSpeed = 20.0f;
	public Vector3 direction;

	Camera cam;
    bool moving = true;

    AudioSource walk;

	//public GameObject mainCam;

	//Vector2 location;
	//Vector2 velocity;
	//Vector2 acceleration;
	
	// Use this for initialization
	void Start () {
		//location = Vector2.zero;
		//velocity = Vector2.zero;
		//acceleration = Vector2.zero;
        walk = gameObject.GetComponent<AudioSource>();
		cam = Camera.main;
		direction = new Vector3 (1, 0, 0);
		//cam = (Camera)GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
        moving = false;
		//float velX = 0.0f;
		//float velZ = 0.0f;
		//direction = new Vector3(this.transform.rotation.y, 0, this.transform.rotation.w);
		
		// Check for up and down movement, else float
		if(Input.GetKey(moveForward)){
			this.transform.Translate(Vector3.forward * speed * Time.deltaTime, cam.transform);
            moving = true;
		}
		else if(Input.GetKey(moveBackward)){
			this.transform.Translate(Vector3.forward * -1f * speed * Time.deltaTime, cam.transform);
            moving = true;
		}
		
		// Check for left and right movement
		if(Input.GetKey(moveRight)){
			this.transform.Translate(Vector3.Cross(-Vector3.forward, Vector3.up) * speed * Time.deltaTime, cam.transform);
            moving = true;
		}
		else if(Input.GetKey(moveLeft)){
			this.transform.Translate(Vector3.Cross(Vector3.forward, Vector3.up) * speed * Time.deltaTime, cam.transform);
            moving = true;
		}

		if (Input.GetKey (turnLeft)) {
			this.transform.Rotate (Vector3.down * Time.deltaTime * 100);
			//Debug.Log("Turning Left");
		}
		else if (Input.GetKey (turnRight)) {
			this.transform.Rotate (Vector3.up * Time.deltaTime * 100);
			//Debug.Log("Turning Right");
		}

		//this.transform.LookAt(direction);
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
		//location += velocity;
		//velocity += acceleration;

        if (moving && !walk.isPlaying)
        {
            walk.Play();
        }
        else if (!moving && walk.isPlaying)
        {
            walk.Stop();
        }
	}
}
