using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonkScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public GameObject target;
	public int targetIndex;
	public GameController gameController;
	public Vector3 direction;
	public Vector3 velocity;

	public Vector3 targetLoc;

	public enum behavior {Seek, Flee, Arrive, Wander, Avoid, Follow};
	public behavior currentBehavior;

	GameObject[] waypoints;

	int avoidFrameCount;

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		moveSpeed = 5;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		currentBehavior = behavior.Follow;
		waypoints = GameObject.FindGameObjectsWithTag ("Waypoint");
		target = waypoints [0];
		targetIndex = 0;
		avoidFrameCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		//DetermineBehavior ();
		findTarget ();
		switch(currentBehavior)
		{
			case behavior.Seek:
				velocity += gameController.Seek(this.transform.position, target.transform.position, moveSpeed);
				break;
				
			case behavior.Flee:
				velocity += gameController.Flee(this.transform.position, target.transform.position, moveSpeed);
				break;
				
			case behavior.Arrive:
				velocity += gameController.Arrive(this.transform.position, target.transform.position, moveSpeed, 15, 5);
				break;
				
			case behavior.Wander:
				velocity += gameController.Wander (this.transform.position, moveSpeed, 40, 10);
				break;
				
			case behavior.Follow:
				Debug.Log("Following");
				//velocity += gameController.Follow (this.transform.position, moveSpeed, target.transform.position);
				direction = target.transform.position - this.transform.position;
				break;
				
			//case behavior.Avoid:
				//Debug.Log("Avoiding");
				//direction += gameController.Avoid (this.transform.position, this.transform.forward, direction, this);
				//break;
		}
		Avoid ();
		//lookAt ();
		velocity *= Time.deltaTime;
		//this.transform.position += velocity;
		this.transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
		//this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
		velocity = Vector3.zero;
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		targetLoc = target.transform.position;
	}	

	void findTarget() {
		//if (target == null) {
		//	target = gameController.player;
		//}
		Vector3 pos = this.transform.position;
		Vector3 targetPos = target.transform.position;
		if (Mathf.Sqrt ((targetPos.x - pos.x) * (targetPos.x - pos.x) + (targetPos.z - pos.z) * (targetPos.z - pos.z)) < 25) {
			targetIndex++;
			if(targetIndex > waypoints.Length - 1)
			{
				target = waypoints[0];
				targetIndex = 0;
			}
			else
			{
				target = waypoints[targetIndex];
			}
			//currentBehavior = behavior.Avoid;
		}
	}

	void Avoid()
	{
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (transform.position, hit.point, Color.red);
				direction += hit.normal * 20;
			}
		}
		
		Vector3 leftR = this.transform.position;
		Vector3 rightR = this.transform.position;
		
		leftR.x -= 2;
		rightR.x += 2;
		
		if (Physics.Raycast (leftR, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (leftR, hit.point, Color.red);
				direction -= hit.normal * 20;
			}
		}
		else if (Physics.Raycast (rightR, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (rightR, hit.point, Color.red);
				direction += hit.normal * 20;
			}
		}
		Quaternion rot = Quaternion.LookRotation (direction);
		
		this.transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime);
		this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
	}

	void DetermineBehavior()
	{
		//Determine when avoiding should stop and following should resume
		/*if (currentBehavior == behavior.Avoid) {
			avoidFrameCount++;
			if(avoidFrameCount >= 180)
			{
				currentBehavior = behavior.Follow;
				avoidFrameCount = 0;
			}
		}*/
	}

	void lookAt() {
		//direction = target.transform.position - this.transform.position;
		this.transform.LookAt(direction, Vector3.up);
	}
}
