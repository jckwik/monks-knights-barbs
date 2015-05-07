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

	public enum behavior {AttackNearbyBarb, ArriveAtAttackedMonastery, ArriveAtARandomMonastery};
	public behavior currentBehavior;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;
	public bool playerInSight;
	
	public bool alive;
	public int health;
	public float hitChance;
	public float attackDelay;
	public int fitnessValue;
	public float timeSurvived;
	public int roundKillCount;
	public int chrom;

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		moveSpeed = 15;
		sightRange = 50;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		currentBehavior = behavior.ArriveAtARandomMonastery;
		target = gameController.monasteryArray[Random.Range(0, gameController.monasteryArray.Length - 1)];
		alive = true;
		hitChance = 25;
		attackDelay = 5 - Mathf.Log(fitnessValue);
		timeSurvived = 0;
		roundKillCount = 0;
		health = 1;
	}

	// Update is called once per frame
	void Update () {
		hitChance = 20 + 25 * Mathf.Log(numKInSight);
		moveSpeed = 5 + Mathf.Log(fitnessValue);
		sightRange = 25 + Mathf.Log(fitnessValue);
		if (!alive)
			return;
		if (health <= 0) 
			alive = !alive;
		timeSurvived += Time.deltaTime;
		if(target == null)
		{
			findTarget();
		}
		attackDelay -= Time.deltaTime;
		if(attackDelay < 0)
		{
			attackDelay = 0;
		}
		Vector3 targetDist = target.transform.position - this.transform.position;
		if(targetDist.magnitude <= 5)
		{
			//Debug.Log("KNIGHT: In Attack Range");
			if(attackDelay <= 0)
			{
				//Debug.Log("KNIGHT: Attacking");
				attackDelay = 5 - Mathf.Log(fitnessValue);
				if(Random.Range(1,100) <= hitChance)
				{
					target.GetComponent<BarbarianScript>().health--;
					if(target.GetComponent<BarbarianScript>().health <= 0)
					{
						roundKillCount++;
						target = null;
					}
					//gameController.barray.Remove (target);
					//gameController.roundInfo.Add((target.GetComponent<BarbarianScript>().chrom).ToString() + " " + (target.GetComponent<BarbarianScript>().timeSurvived).ToString());
					//Destroy(target);
				}
			}
		}
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
		try {
			if (target == null) {
				target = gameController.player;
			}
		}
		catch {
			target = gameController.player;
		}
	}
	void findUnitsInSight() {
		// clear the arrays
		bInSight.Clear();
		kInSight.Clear();
		mInSight.Clear();
		playerInSight = false;
		
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

		if (gameController.player != null) {
			Vector3 diffp = gameController.player.transform.position - this.transform.position;
			if (diffp.magnitude <= sightRange) {
				playerInSight = true;
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