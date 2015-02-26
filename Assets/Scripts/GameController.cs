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
	public GameObject monastaryFab;

	List<GameObject> barray = new List<GameObject> ();
	List<GameObject> karray = new List<GameObject> ();
	List<GameObject> marray = new List<GameObject> ();
	List<GameObject> monarray = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	void Initialize() {
		Vector3 playerPos = new Vector3 (0, 1, 0);
		player = (GameObject)Instantiate (playerFab, playerPos, Quaternion.identity);
		for (int i = 0; i < monastaries; i++) {
			//Create monastaries
		}
		for (int i = 0; i < barbarians; i++) {
			//Create barbarians at random locations
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject barb = (GameObject)Instantiate(barbarianFab, pos, Quaternion.identity);
			barray.Add(barb);
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject knight = (GameObject)Instantiate(knightFab, pos, Quaternion.identity);
			karray.Add(knight);
		}
		for (int i = 0; i < monks; i++) {
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject monk = (GameObject)Instantiate(monkFab, pos, Quaternion.identity);
			marray.Add(monk);
			//Create monks at random locations
		}
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
}
