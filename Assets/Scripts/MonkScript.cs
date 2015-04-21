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

	public enum behavior {Seek, Flee, Wander, Follow, Avoid};
	public behavior currentBehavior;

	GameObject[] waypoints;

	//int avoidFrameCount;
	public bool leftHit;
	public bool rightHit;
	public bool centerHit;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;

	public bool alive;

	NavMeshAgent agent;

	//Vector3 lastPos;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
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
		leftHit = false;
   		rightHit = false;
   		centerHit = false;
		alive = true;
	}
	
	// Update is called once per frame
	void Update () {
		FindUnitsInSight();
		//agent.SetDestination (target.transform.position);
		DetermineBehavior ();
		switch(currentBehavior)
		{
			case behavior.Seek:
				//velocity += gameController.Seek(this.transform.position, target.transform.position, moveSpeed);
				agent.SetDestination (target.transform.position);
				break;
				
			case behavior.Flee:
				//velocity += gameController.Flee(this.transform.position, target.transform.position, moveSpeed);
				agent.SetDestination (target.transform.position - this.transform.position);
				break;
				
			case behavior.Wander:
				velocity += gameController.Wander (this.transform.position, this.transform.forward, moveSpeed, 20, 10);
				velocity *= Time.deltaTime;
				agent.SetDestination (this.transform.position + velocity);
				break;
				
			case behavior.Follow:
				//Debug.Log("Following");
				FindTarget ();
				agent.SetDestination (target.transform.position);
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
		//Avoid ();
		LookAt ();
		//velocity *= Time.deltaTime;
		//this.transform.position += velocity;
		//this.transform.position += this.transform.forward * moveSpeed * Time.deltaTime;
		//this.transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
		velocity = Vector3.zero;
		//Lock y position and x and z rotation
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
	}	
	
	void FindUnitsInSight() {
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
			if (diff.magnitude <= 2*sightRange/3) {
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
	void FindTarget() {
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

	void LookAt() {
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position), rotationSpeed * Time.deltaTime);
	}

	void DetermineBehavior()
	{
		//If Barbarian is too close
		if(numBInSight > 0)
		{
			//Flee
			currentBehavior = behavior.Flee;

			//Find closest Barbarian
			float closestDist = float.MaxValue;
			foreach(GameObject b in bInSight)
			{
				Vector3 diff = b.transform.position - this.transform.position;
				if(diff.magnitude < closestDist)
				{
					closestDist = diff.magnitude;
					target = b;
				}
			}
			//Check its direction

			//Move opposite

		}
		else{
			//Else check distance too monasteries
			float closestDist = float.MaxValue;
			GameObject closestSafeMon = waypoints[0];
			foreach(GameObject w in waypoints)
			{
				Vector3 diff = w.transform.position - this.transform.position;
				if(diff.magnitude < closestDist)
				{
					closestDist = diff.magnitude;
					target = w;
					if(!(w.GetComponent<MonasteryScript>().underAttack))
					{
						closestSafeMon = w;
					}
				}
			}

			//If too far from closest one
			if(closestDist > 40)
			{
				currentBehavior = behavior.Seek;
				target = closestSafeMon;
			}
			//Else wander around your current monastery
			else{
				currentBehavior = behavior.Wander;
			}

		}
	}
}
