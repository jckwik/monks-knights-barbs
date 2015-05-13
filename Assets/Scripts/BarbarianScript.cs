using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarbarianScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public float sightRange;
	public GameObject target;
	public GameController gameController;
	public Vector3 direction;
	public Vector3 velocity;
	public float groupingStrength;

	public StateMachine stM;
	public int currentState;
	
	public enum behavior {Seek, Flee, Arrive, Wander, Avoid, Follow};
//	public behavior currentBehavior;
	
	List<GameObject> bInSight = new List<GameObject> ();
	List<GameObject> kInSight = new List<GameObject> ();
	List<GameObject> mInSight = new List<GameObject> ();
	List<GameObject> monasteryInSight = new List<GameObject> ();
	
	public int numBInSight;
	public int numKInSight;
	public int numMInSight;
	public int numMonasteryInSight;
	public bool playerInSight;

	public bool alive;
	public int health;
	public float hitChance;
	public float attackDelay;
	public int fitnessValue;
	public float timeSurvived;
	public int roundKillCount;
	public int chrom;
	public float timeSinceLastDecision;
	public bool aggressive;
	public int decisionKills;

	NavMeshAgent agent;

	/* State Machine: 
	 *  -> States:
	 * 4
     * 0 Wander
     * 1 Seek
     * 2 Flee
     * 3 Follow
     *  -> Change inputs:
     * 5
     * 0 I see unguarded monks
     * 1 I see more knights than friends
     * 2 I see more friends than knights
     * 3 I see an unguarded monastery
     * 4 I'm all alone
	*/

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agent.speed = 10;
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		stM = gameController.bStateM;
		moveSpeed = 15;
		sightRange = 100;
		direction = Vector3.zero;
		velocity = Vector3.zero;
		groupingStrength = 0.1f;
		currentState = 0;
		alive = true;
		hitChance = 40;
		attackDelay = 2;
		timeSurvived = 0;
		roundKillCount = 0;
		health = 1;
		timeSinceLastDecision = 0;
		decisionKills = 0;
		makeNewDecision();
	}
	
	// Update is called once per frame
	void Update () {
		hitChance = 20 + Mathf.Log(fitnessValue);
		float change;
		if (Mathf.Log(fitnessValue) > 255 || Mathf.Log(fitnessValue) < 255) change = 0;
		else change = Mathf.Log(fitnessValue);
		agent.speed = 5 + change;
		sightRange = 25 + change;
		if (!alive)
			return;
		if (health <= 0) 
			alive = !alive;
		timeSurvived += Time.deltaTime;
		attackDelay -= Time.deltaTime;
		timeSinceLastDecision += Time.deltaTime;
		if(attackDelay < 0)
		{
			attackDelay = 0;
		}
		if (timeSinceLastDecision > 20) {
			timeSinceLastDecision = 0;
			checkForStoreBayes();
			findUnitsInSight();
			makeNewDecision();
		}
		findTarget ();
		if (numMInSight > 0 && numKInSight == 0) {
			MakeTrans (0);
		}
		else if (numBInSight == 0 && numKInSight == 0 && numMInSight == 0 && numMonasteryInSight == 0) {
			MakeTrans (4);
		}
		else if (numBInSight > 0 && numKInSight < numBInSight + 2) {
			MakeTrans (2);
		}
		else if (numKInSight > numBInSight + 2) {
			MakeTrans (1);
		}
		if (target != null) {
			// Check target type
			//Unit tarUnit = target.GetComponenet<Unit>();
			MonkScript mScript = target.GetComponent<MonkScript>();
			KnightScript kScript = target.GetComponent<KnightScript>();
			MonasteryScript monScript = target.GetComponent<MonasteryScript>();
			if (mScript != null || kScript != null || monScript != null && aggressive) {// if a monk or a knight or a monastery
				Vector3 targetDist = target.transform.position - this.transform.position;
				// if not a monastery
				if (monScript == null) {
					if (targetDist.magnitude <= 5) {
						//Debug.Log ("Barbarian: In Attack Range");
						if (attackDelay <= 0) {
							//Debug.Log ("Barbarian: Attacking");
							attackDelay = 5 - Mathf.Log(fitnessValue);
							if (Random.Range (1, 100) <= hitChance) {
								//tarUnit.health-=1;
								if (kScript != null) 
								{
									Debug.Log ("Barbarian: Killed a Knight");
									kScript.health-=1;
									if(kScript.health <= 0)
									{
										roundKillCount++;
									}
								}
								else
								{
									Debug.Log ("Barbarian: Killed a Monk");
									mScript.health-=1;
									if(mScript.health <= 0)
									{
										roundKillCount++;
									}
								}
							}
						}
					}
				}
				// if a monastery
				else {
					if (targetDist.magnitude <= 20) {
						if (attackDelay <= 0) {
							//Debug.Log ("Barbarian: Attacking");
							attackDelay = 5 - Mathf.Log(fitnessValue);
							if (Random.Range (1, 100) <= hitChance) {
								Debug.Log ("Barbarian: Attacking Monastery");
								monScript.health -=1;
								if (monScript.health <=0)
								{
									roundKillCount++;
								}
							}
						}
					}
				}
			}

		}
//		switch(currentBehavior)
//		{
//			case behavior.Seek:
//				velocity += gameController.Seek(this.transform.position, target.transform.position, moveSpeed);
//				//Debug.Log("Seeking");
//				break;
//
//			case behavior.Flee:
//				velocity += gameController.Flee(this.transform.position, target.transform.position, moveSpeed);
//				//Debug.Log("Fleeing");
//				break;
//
//			case behavior.Arrive:
//				velocity += gameController.Arrive(this.transform.position, target.transform.position, moveSpeed, 15, 5);
//				//Debug.Log("Arriving");
//				break;
//
//			case behavior.Wander:
//				velocity += gameController.Wander (this.transform.position, moveSpeed, 40, 10);
//				//Debug.Log("Wandering");
//				break;
//
//			case behavior.Follow:
//				//Debug.Log("Following");
//				break;
//
//			case behavior.Avoid:
//				//Debug.Log("Avoiding");
//				break;
//		}

		CallAction ();
		
		Debug.DrawLine (this.transform.position, this.transform.position+velocity, Color.red);
		//velocity *= Time.deltaTime;
		//this.transform.position += velocity;
		agent.SetDestination (this.transform.position + velocity);
		lookAt ();
		velocity = Vector3.zero;
		this.transform.position = new Vector3(this.transform.position.x, 1, this.transform.position.z);
		this.transform.rotation = new Quaternion(0, this.transform.rotation.y, 0, this.transform.rotation.w);
	}

	public void checkForStoreBayes(){
		if (decisionKills > 0 && alive) {
			gameController.bBayes.AddObs (numKInSight, numMInSight, numBInSight, numMonasteryInSight > 0, aggressive);
		} else if (decisionKills == 0 && !alive) {
			gameController.bBayes.AddObs (numKInSight, numMInSight, numBInSight, numMonasteryInSight > 0, !aggressive);
		}
	}

	void makeNewDecision() {
		double chance = gameController.bBayes.CalcBayes (numKInSight, numMInSight, numBInSight, numMonasteryInSight > 0, true) / 
			gameController.bBayes.CalcBayes (numKInSight, numMInSight, numBInSight, numMonasteryInSight > 0, false);
		aggressive = chance >= 1;
		Debug.Log (aggressive);
	}

	void findTarget() {
		try {
			if (target == null) {
				//target = gameController.player;
			}
		}
		catch {
			target = null;
		}
	}
	void findUnitsInSight() {
		// clear the arrays
		bInSight.Clear();
		kInSight.Clear();
		mInSight.Clear();
		monasteryInSight.Clear();
		
		// barbarians in sight
		List<GameObject> barbs = gameController.barray;
		for (int i=0; i<barbs.Count; i++) {
			GameObject b = barbs[i];
			if (b == this.gameObject) continue;
			
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

		//monasteries in sight
		List<GameObject> monastery = gameController.monasteryArray;
		for (int i=0; i<monastery.Count; i++) {
			GameObject mon = monastery[i];
			
			// check if they are in range
			Vector3 diff = mon.transform.position - this.transform.position;
			if (diff.magnitude <= sightRange) {
				monasteryInSight.Add(mon);
			}
		}
		
		numBInSight = bInSight.Count;
		numKInSight = kInSight.Count;
		numMInSight = mInSight.Count;
		numMonasteryInSight = monasteryInSight.Count;
		
	}
	public GameObject getClosestMonk() {
		if (numMInSight <= 0) return null;
		
		GameObject closest = mInSight[0];
		Vector3 dist = this.transform.position - closest.transform.position;
		float record = dist.magnitude;
		
		for (int i=1; i<mInSight.Count; i++) {
			GameObject obj = mInSight[i];
			
			dist = this.transform.position - obj.transform.position;
			float magn = dist.magnitude;
			if (magn < record) {
				closest = obj;
				record = magn;
			}
		}
		return closest;
	}
	public GameObject getClosestKnight() {
		if (numKInSight <= 0) return null;
		
		GameObject closest = kInSight[0];
		Vector3 dist = this.transform.position - closest.transform.position;
		float record = dist.magnitude;
		
		for (int i=1; i<kInSight.Count; i++) {
			GameObject obj = kInSight[i];
			
			dist = this.transform.position - obj.transform.position;
			float magn = dist.magnitude;
			if (magn < record) {
				closest = obj;
				record = magn;
			}
		}
		return closest;
	}
	void lookAt() {
		//direction = target.transform.position - this.transform.position;
		//this.transform.LookAt(velocity, Vector3.up);
	}
	// Use state machine to make a transition and display the input
	public void MakeTrans(int input)
	{
		currentState = stM.MakeTrans (currentState, input);
		//Debug.Log ("Input"+input + ": " +stM.Inputs[input]);
	}

	public void CallAction ()
	{
		// Always group with other barbs
		if (numBInSight > 0) {
			if ((BarbarianScript)bInSight[0].GetComponent("BarbarianScript") != null) {
			BarbarianScript blah = (BarbarianScript)bInSight[0].GetComponent("BarbarianScript");
			if (blah.target != this.gameObject) {
				target = bInSight [0];
				velocity += gameController.Arrive (this.transform.position, target.transform.position, moveSpeed, 20, 5);
				velocity *= groupingStrength;
			}
			}
			else 
				s0Act();
			
		}
		
		switch (currentState)
		{
		case 0:
			s0Act ();
			break;
		case 1:
			s1Act ();
			break;
		case 2:
			s2Act ();
			break;
		case 3:
			s3Act ();
			break;
		default:
			Debug.Log ("BARB: Oops!  Bad state!");
			break;
		}
		return;
	}

	void s0Act ()
	{
		velocity += gameController.Wander (this.transform.position, this.transform.forward, moveSpeed, 40, 20);
		//Debug.Log ("State0: I'm just wandering.");
	}
	void s1Act ()
	{
		//Find a target
		if (numKInSight > 0) {
			target = getClosestKnight();
			if (aggressive) velocity += gameController.Seek (this.transform.position, target.transform.position, moveSpeed);
			else velocity += gameController.Flee (this.transform.position, target.transform.position, moveSpeed);
		}
		else if (numMInSight > 0) {
			target = getClosestMonk();
			if (aggressive) velocity += gameController.Seek (this.transform.position, target.transform.position, moveSpeed);
			else velocity += gameController.Flee (this.transform.position, target.transform.position, moveSpeed);
		} else if (numMonasteryInSight > 0) {
			target = monasteryInSight[0];
			if (aggressive) velocity += gameController.Seek (this.transform.position, target.transform.position, moveSpeed);
			else velocity += gameController.Flee (this.transform.position, target.transform.position, moveSpeed);
		} else 
			MakeTrans (4);
		//Debug.Log ("State1: I'm chasing something!");
	}
	void s2Act ()
	{
		//Find a target
		if (numKInSight > 0) {
			target = getClosestKnight();
			if (aggressive) velocity += gameController.Seek (this.transform.position, target.transform.position, moveSpeed);
			else velocity += gameController.Flee (this.transform.position, target.transform.position, moveSpeed);
		} else
			MakeTrans (4);
		//Debug.Log ("State2: Running away!");
	}
	void s3Act ()
	{
		/*
		//Follow?
		if (numBInSight > 0) {
			target = bInSight [0];
			velocity += gameController.Arrive (this.transform.position, target.transform.position, moveSpeed, 20, 5);
		} else
			MakeTrans (4);
		//Debug.Log ("State3: I'm following others.");
		*/
		MakeTrans (4);
	}
}
