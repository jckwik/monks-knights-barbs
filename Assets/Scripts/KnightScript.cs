using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public float sightRange;
	public GameObject target;
	public GameController gameController;
	public Vector3 direction;
	public Vector3 velocity;

	public enum behavior {Seek, Flee, Arrive, Wander, Avoid, Follow};
	public behavior currentBehavior;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;
	

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		moveSpeed = 5;
		sightRange = 50;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		currentBehavior = behavior.Arrive;
	}
	
	// Update is called once per frame
	void Update () {
		findUnitsInSight();
		findTarget ();
		lookAt ();
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
				break;

			case behavior.Avoid:
				break;
		}
		
		velocity *= Time.deltaTime;
		this.transform.position += velocity;
		velocity = Vector3.zero;
	}

	void findTarget() {
		if (target == null) {
			target = gameController.player;
		}
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
			if (k == this) continue;
			
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
	void lookAt() {
		direction = target.transform.position - this.transform.position;
		this.transform.LookAt(direction, Vector3.up);
	}
}