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

	public List<GameObject> barray = new List<GameObject> ();
	public List<GameObject> karray = new List<GameObject> ();
	public List<GameObject> marray = new List<GameObject> ();
	public List<GameObject> monarray = new List<GameObject> ();

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
		GameObject mon = (GameObject)Instantiate (monastaryFab, new Vector3(-20, 1, -20), Quaternion.identity);
		monarray.Add (mon);
		mon = (GameObject)Instantiate (monastaryFab, new Vector3(20, 1, -20), Quaternion.identity);
		monarray.Add (mon);
		mon = (GameObject)Instantiate (monastaryFab, new Vector3(20, 1, 20), Quaternion.identity);
		monarray.Add (mon);
		mon = (GameObject)Instantiate (monastaryFab, new Vector3(-20, 1, 20), Quaternion.identity);
		monarray.Add (mon);
		
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
	
	public Vector3 PathFollow(List<GameObject> path, Vector3 pos, float speed, Vector3 direction) {
		int curWP = -1;
		float pathRadius = 5;
		// Predict the movement
		Vector3 predict = direction.normalized * speed * 2;
		Vector3 predictLoc = predict + pos;
		
		// Find noraml to the path from the predicted location;
		Vector3 normal;
		Vector3 target = Vector3.zero;
		float worldRecord = 10000000;
		
		// Loop through all the points in the path;
		int pathSize = path.Count;
		for (int i=0; i < pathSize; i++) {
			// Look at a line segment
			Vector3 a = path[i%pathSize].transform.position;
			Vector3 b = path[(i+1)%pathSize].transform.position;
			// Get the normal point to that line
			Vector3 normalPoint = GetNormal(predictLoc, a, b);

			// Chec if normal is on line segment
			Vector3 dir = b -a;
			//Fi it's not within the line segment, consider the normal to just be the end of the line segment (point b)
			// if (da + db > line.mag() +1)
			if (normalPoint.x < Mathf.Min(a.x, b.x) || normalPoint.x > Mathf.Max(a.x, b.x) ||
			    normalPoint.y < Mathf.Min(a.y, b.y) || normalPoint.y > Mathf.Max(a.y, b.y)) {

				normalPoint = b + Vector3.zero;
				// if we're at the end we really want the next line segment for looking ahead
				a = (Vector3)(path[(i+1)%pathSize].transform.position);
				b = (Vector3)(path[(i+2)%pathSize].transform.position);
				dir = b - a;
			}

			// How far away are we from the path?
			Vector3 dVec = predictLoc - normalPoint;
			float d = dVec.magnitude;
			// Did we beat the worldRecord and find the closest line segment?
			if (d<worldRecord) {
				normal = normalPoint;
				curWP = i % pathSize;

				// Look at the direction of the line segment so we can seek a little bit ahead of the normal
				dir = dir.normalized;
				dir *= speed;
				target = Vector3.zero + normal; // **NOT SURE IF THIS WORKS **
				target += dir;
			}
		}
		
		if (worldRecord > pathRadius) {
			return Seek (pos, target, speed);
		}
		else {
			return direction.normalized * speed;
		}
	}

	public Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c) {
		Vector3 side1 = b-a;
		Vector3 side2 = c-a;

		// Cross the vectors to get a perpendicular vector, then normalize it.
		return Vector3.Cross(side1, side2).normalized;
	}
}
