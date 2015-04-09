using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public float sightRange;
	public GameObject target;
	public int targetIndex;
	public GameController gameController;
	public Vector3 direction;
	public Vector3 velocity;
	public DecisionTree decisionTree;

	public enum behavior {AttackNearbyBarb, ArriveAtAttackedMonastery, ArriveAtARandomMonastery};
	public behavior currentBehavior;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;
	
	public bool alive;
	public float hitChance;

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		GameObject dT = GameObject.Find("DecisionTree");
		decisionTree = (DecisionTree) dT.GetComponent(typeof(DecisionTree));
		moveSpeed = 5;
		sightRange = 50;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		currentBehavior = behavior.ArriveAtARandomMonastery;
		target = gameController.monasteryArray[Random.Range(0, gameController.monasteryArray.Length - 1)];
		alive = true;
		hitChance = 25;
	}
	
	// Update is called once per frame
	void Update () {
		hitChance = 25 + numKInSight * 15;
		findUnitsInSight();
		//findTarget ();
		currentBehavior = makeDec();
		switch(currentBehavior)
		{
			case behavior.AttackNearbyBarb:
				barbarianNearby();
				velocity += gameController.Seek(this.transform.position, target.transform.position, moveSpeed);
				break;

			case behavior.ArriveAtAttackedMonastery:
				attackedMonastery();
				velocity += gameController.Arrive(this.transform.position, target.transform.position, moveSpeed, 40, 20);
				break;

			case behavior.ArriveAtARandomMonastery:
				velocity += gameController.Arrive(this.transform.position, target.transform.position, moveSpeed, 40, 20);
				break;
		}
		
		velocity *= Time.deltaTime;
		this.transform.position += velocity;
		velocity = Vector3.zero;
		Debug.DrawLine (this.transform.position, target.transform.position, Color.blue);
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		lookAt ();
		this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
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
		direction = Vector3.Normalize(target.transform.position - this.transform.position);
		this.transform.LookAt(target.transform.position, Vector3.up);
	}
	
	behavior makeDec()
	{
		/*
		Node currNode = decisionTree.Root;
		while (currNode.NoPtr != null) // Will never backtrack => don't need recursion
		{
			if (currNode.Test())
				currNode = currNode.YesPtr;   // Recurse on "yes" child
			else
				currNode = currNode.NoPtr;    // or "no" child
		}
		*/
		if(attackedMonastery())
		{
			if(atAttackedMonastery())
			{
				return behavior.AttackNearbyBarb;
			}
			else
			{
				return behavior.ArriveAtAttackedMonastery;
			}
		}
		else
		{
			if(barbarianNearby())
			{
				return behavior.AttackNearbyBarb;
			}
			else
			{
				if(currentBehavior == behavior.ArriveAtARandomMonastery)
				{
					target = gameController.monasteryArray[targetIndex];
					return behavior.ArriveAtARandomMonastery;
				}
				else
				{
					targetIndex = Random.Range(0, gameController.monasteryArray.Length - 1);
					target = gameController.monasteryArray[targetIndex];
				}
				return behavior.ArriveAtARandomMonastery;
			}
		}
	}

	bool attackedMonastery()
	{
		foreach(GameObject m in gameController.monasteryArray)
		{
			MonasteryScript monScript = (MonasteryScript) m.GetComponent(typeof(MonasteryScript));
			if(monScript.underAttack == true)
			{
				target = m;
				return true;
			}
		}
		return false;
	}

	bool atAttackedMonastery()
	{
		foreach(GameObject m in gameController.monasteryArray)
		{
			MonasteryScript monScript = (MonasteryScript) m.GetComponent(typeof(MonasteryScript));
			if(monScript.underAttack == true)
			{
				if(Mathf.Sqrt((m.transform.position.x - this.transform.position.x) * (m.transform.position.x - this.transform.position.x)
				              + (m.transform.position.z - this.transform.position.z) * (m.transform.position.z - this.transform.position.z)) < 40)
				{
					target = m;
					return true;
				}
			}
		}
		return false;
	}

	bool barbarianNearby()
	{
		/*
		foreach(GameObject b in gameController.barray)
		{
			if(Mathf.Sqrt((b.transform.position.x - this.transform.position.x) * (b.transform.position.x - this.transform.position.x)
			              + (b.transform.position.z - this.transform.position.z) * (b.transform.position.z - this.transform.position.z)) < 40)
			{
				target = b;
				return true;
			}
		}*/
		if (bInSight.Count > 0)
		{
			target = bInSight[0];
			return true;
		}
		return false;
	}
}