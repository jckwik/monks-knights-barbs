using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public int monastaries;
	public int barbarians;
	public int knights;
	public int monks;

	public GameObject barbarianFab;
	public GameObject knightFab;
	public GameObject monkFab;
	public GameObject player;
	public GameObject playerFab;

	public StateMachine bStateM;

	public List<GameObject> barray = new List<GameObject> ();
	public List<GameObject> karray = new List<GameObject> ();
	public List<GameObject> marray = new List<GameObject> ();
	public GameObject[] monasteryArray;
	
	// Use this for initialization
	void Start () {
		Initialize ();
	}

	void Update()
	{
		List<GameObject> barrayNew = new List<GameObject> ();
		foreach(GameObject b in barray)
		{
			if(b != null)
			{
				barrayNew.Add(b);
			}
		}
		barray = barrayNew;

		List<GameObject> karrayNew = new List<GameObject> ();
		foreach(GameObject k in karray)
		{
			if(k != null)
			{
				karrayNew.Add(k);
			}
		}
		karray = karrayNew;

		List<GameObject> marrayNew = new List<GameObject> ();
		foreach(GameObject m in marray)
		{
			if(m != null)
			{
				marrayNew.Add(m);
			}
		}
		marray = marrayNew;
	}

	void Initialize() {
		Debug.Log ("Running Initialize");

		Vector3 playerPos = new Vector3 (0, 1, 0);
		player = (GameObject)Instantiate (playerFab, playerPos, Quaternion.identity);
		for (int i = 0; i < monastaries; i++) {
			//Create monastaries
		}

		bStateM = new StateMachine ("BarbarianStateMachine.txt");

		for (int i = 0; i < barbarians; i++) {
			//Create barbarians at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			GameObject barb = (GameObject)Instantiate(barbarianFab, pos, Quaternion.identity);
			barray.Add(barb);
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			GameObject knight = (GameObject)Instantiate(knightFab, pos, Quaternion.identity);
			karray.Add(knight);
		}
		for (int i = 0; i < monks; i++) {
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			GameObject monk = (GameObject)Instantiate(monkFab, pos, Quaternion.identity);
			marray.Add(monk);
			//Create monks at random locations
		}
		monasteryArray = GameObject.FindGameObjectsWithTag("Monastery");
	}
	
	public Vector3 Seek (Vector3 pos, Vector3 targetPos, float speed)
	{
		//find dv, desired velocity
		Vector3 dv = targetPos - pos;		
		dv = dv.normalized * speed; 	//scale by maxSpeed
		dv.y = 0;								// only steer in the x/z plane
		return dv;
	}
	
	public Vector3 Flee (Vector3 pos, Vector3 targetPos, float speed) {
		Vector3 dv = pos - targetPos;		
		dv = dv.normalized * speed; 	//scale by maxSpeed
		dv.y = 0;								// only steer in the x/z plane
		return dv;
	}
	
	public Vector3 Arrive (Vector3 pos, Vector3 targetPos, float speed, float slowDistance, float arriveDistance) {
		Vector3 dv = targetPos - pos;
		float mag = dv.magnitude;
		float moveSpeed;
		if (mag < arriveDistance) { return Vector3.zero; }
		else if (mag > slowDistance) { moveSpeed = speed; }
		else {
			float x = mag / (slowDistance - arriveDistance);
			moveSpeed = speed * x;
		}
		dv = dv.normalized * moveSpeed; 	//scale by maxSpeed
		dv.y = 0;								// only steer in the x/z plane
		return dv;		
	}
	
	public Vector3 Wander (Vector3 pos, float speed, float wanderD, float wanderR) {
		float randomAngle = Random.Range (0, Mathf.PI*2);
		
		Vector3 circleLoc = transform.forward;
		circleLoc *= wanderD;
		circleLoc += transform.position;
		
		
		Vector3 circleOffSet = new Vector3(Mathf.Cos (randomAngle)*wanderR, 0, Mathf.Sin (randomAngle)*wanderR);
		Vector3 target = circleLoc + circleOffSet;

		return Seek( pos, target, speed);	
	}

	public Vector3 Follow (Vector3 pos, float speed, Vector3 targetPos) {
		//float randomAngle = Random.Range (0, Mathf.PI*2);
		
		//Vector3 circleLoc = transform.forward;
		//circleLoc *= wanderD;
		//circleLoc += transform.position;
		
		
		//Vector3 circleOffSet = new Vector3(Mathf.Cos (randomAngle)*wanderR, 0, Mathf.Sin (randomAngle)*wanderR);
		//Vector3 target = circleLoc + circleOffSet;


		return Seek( pos, targetPos, speed);	
	}
	/*
	public Vector3 Avoid (Vector3 pos, Vector3 forw, Vector3 dir, GameObject monk) {

		RaycastHit hit;
		if (Physics.Raycast (pos, forw, hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (transform.position, hit.point, Color.red);
				dir += hit.normal * 20;
			}
		}
		
		Vector3 leftR = monk.transform.position;
		Vector3 rightR = monk.transform.position;
		
		leftR.x -= 2;
		rightR.x += 2;
		
		if (Physics.Raycast (leftR, this.transform.forward, hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (leftR, hit.point, Color.red);
				dir += hit.normal * 20;
			}
		}
		if (Physics.Raycast (rightR, this.transform.forward, hit, 20)) {
			if(hit.transform != this.transform)
			{
				Debug.DrawLine (rightR, hit.point, Color.red);
				dir += hit.normal * 20;
			}
		}
		var rot = Quaternion.LookRotation (dir);
		
		monk.transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime);
		monk.transform.position += this.transform.forward * 20 * Time.deltaTime;

		return dir;
	}
	*/
}