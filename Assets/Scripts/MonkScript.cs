using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonkScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public float sightRange;
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
	public bool leftHit;
	public bool rightHit;
	public bool centerHit;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;

	NavMeshAgent agent;
	Rigidbody rb;

	//Vector3 lastPos;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		rb = GetComponent<Rigidbody> ();
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		moveSpeed = 15;
		sightRange = 50;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		currentBehavior = behavior.Follow;
		waypoints = GameObject.FindGameObjectsWithTag ("Monastery");
		target = waypoints [0];
		targetIndex = 0;
		avoidFrameCount = 0;
		leftHit = false;
   		rightHit = false;
   		centerHit = false;
		//lastPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		findUnitsInSight();
		findTarget ();
		agent.SetDestination (target.transform.position);
		//DetermineBehavior ();
		/*
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
				//Debug.Log("Following");
				velocity += gameController.Follow (this.transform.position, moveSpeed, target.transform.position);
				direction = target.transform.position - this.transform.position;
				break;
				
			case behavior.Avoid:
				//Debug.Log("Avoiding");
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, 20)) {
					if(hit.transform != this.transform)
					{
						Debug.DrawLine (transform.position, hit.point, Color.green);
						currentBehavior = behavior.Avoid;
					}
					else
					{
						centerHit = false;
					}
				}
				else
				{
					centerHit = false;
				}
				if(!(leftHit || rightHit || centerHit))
				{
				//Debug.Log("K");
					currentBehavior = behavior.Follow;
				}
				break;
		}
		*/
		//Avoid ();
		lookAt ();
		//velocity *= Time.deltaTime;
		//this.transform.position += velocity;
		//this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
		//this.transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
		//velocity = Vector3.zero;
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		//this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
		//targetLoc = target.transform.position;
		//lastPos = this.transform.position;
	}	
	
	void findUnitsInSight() {
		// clear the arrays
		bInSight.Clear();
		kInSight.Clear();
		mInSight.Clear();
		
		// barbarians in sight
		List<GameObject> barbs = gameController.barray;
		for (int i=0; i<barbs.Count; i++) {
			GameObject b = barbs[i];
			
			// check if they are in range
			Vector3 diff = b.transform.position - this.transform.position;
			if (diff.magnitude <= sightRange) {
				bInSight.Add(b);
			}
		}
		
		// knights in sight
		List<GameObject> knights = gameController.karray;
		for (int i=0; i<knights.Count; i++) {
			GameObject k = knights[i];
			
			// check if they are in range
			Vector3 diff = k.transform.position - this.transform.position;
			if (diff.magnitude <= sightRange) {
				kInSight.Add(k);
			}
		} 
		
		// monks in sight
		List<GameObject> monks = gameController.marray;
		for (int i=0; i<monks.Count; i++) {
			GameObject m = monks[i];
			if (m == this) continue;
			
			// check if they are in range
			Vector3 diff = m.transform.position - this.transform.position;
			if (diff.magnitude <= sightRange) {
				mInSight.Add(m);
			}
		}
		
		numBInSight = bInSight.Count;
		numKInSight = kInSight.Count;
		numMInSight = mInSight.Count;
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
				direction = target.transform.position - this.transform.position;
				targetIndex = 0;
			}
			else
			{
				target = waypoints[targetIndex];
				direction = target.transform.position - this.transform.position;
			}
		}
	}

	void Avoid()
	{
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast (this.transform.position, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (transform.position, hit.point, Color.green);
				//direction += hit.normal * 10;
				currentBehavior = behavior.Avoid;
				centerHit = true;
			}
			else
			{
				centerHit = false;
			}
		}
		else
		{
			centerHit = false;
		}
		Vector3 leftR = this.transform.position;
		Vector3 rightR = this.transform.position;
		
		leftR.x -= 2;
		rightR.x += 2;
		
		if (Physics.Raycast (leftR, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (leftR, hit.point, Color.green);
				direction += hit.normal * 10;
				currentBehavior = behavior.Avoid;
				leftHit = true;
			}
			else
			{
				leftHit = false;
			}
		}
		else
		{
			leftHit = false;
		}
		if (Physics.Raycast (rightR, this.transform.forward, out hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (rightR, hit.point, Color.green);
				direction += hit.normal * 10;
				currentBehavior = behavior.Avoid;
				rightHit = true;
			}
			else
			{
				rightHit = false;
			}
		}
		else
		{
			rightHit = false;
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
		//direction = this.transform.position - lastPos;
		//this.transform.LookAt(agent.rigidbody.velocity, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position), rotationSpeed * Time.deltaTime);
	}
}
