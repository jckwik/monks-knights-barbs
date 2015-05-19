using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
	public GameObject monasteryFab;

	public StateMachine bStateM;
	public BayesBarb bBayes = new BayesBarb();

	public List<GameObject> barray = new List<GameObject> ();
	public List<GameObject> karray = new List<GameObject> ();
	public List<GameObject> marray = new List<GameObject> ();
	public List<GameObject> monasteryArray = new List<GameObject> ();
	public List<GameObject> monasteryUnderAttackArray = new List<GameObject> ();
	public List<string> roundInfo = new List<string>();
	public List<int> chroms = new List<int> ();
	public List<int> fitnessValues = new List<int> ();

	public int roundNumber;
    public int barbRounds = 0;
    public int knightRounds = 0;
    public int tieRounds = 0;
	public int[,] monPlace;
	public float roundTime;

	// Use this for initialization
	void Start () {
		monPlace =  new int[,]{ {0, 0}, {100, 200}, {200, 0}, {-100, 200}, {-200, 0}, {-100, -200}, {100, -200} };
		roundNumber = 0;
		LoadData();
		Initialize ();
	}

	void Update()
	{
		List<GameObject> barrayNew = new List<GameObject> ();
		foreach(GameObject b in barray)
		{
			BarbarianScript bScript = b.GetComponent<BarbarianScript>();
			if(bScript.alive)
			{
				barrayNew.Add(b);
			}
			else {
				roundInfo.Add((bScript.chrom).ToString() + " " + (bScript.timeSurvived).ToString());
                AudioSource.PlayClipAtPoint(bScript.dieSound.clip, b.transform.position);
				Destroy(b);
			}
		}
		barray = barrayNew;

		List<GameObject> karrayNew = new List<GameObject> ();
		foreach(GameObject k in karray)
		{
			KnightScript kScript = k.GetComponent<KnightScript>();
			if(kScript.alive)
			{
				karrayNew.Add(k);
			}
			else {
				roundInfo.Add((kScript.chrom).ToString() + " " + (kScript.timeSurvived).ToString());
                AudioSource.PlayClipAtPoint(kScript.dieSound.clip, k.transform.position);
				Destroy(k);
			}
		}
		karray = karrayNew;

		List<GameObject> marrayNew = new List<GameObject> ();
		foreach(GameObject m in marray)
		{
			MonkScript mScript = m.GetComponent<MonkScript>();
			if(mScript.alive)
			{
				marrayNew.Add(m);
			}
			else {
				roundInfo.Add((mScript.chrom).ToString() + " " + (mScript.timeSurvived * 1.5).ToString());
                AudioSource.PlayClipAtPoint(mScript.dieSound.clip, m.transform.position);
				Destroy(m);
			}
		}
		marray = marrayNew;

		monasteryUnderAttackArray = new List<GameObject> ();
        List<GameObject> newMonArray = new List<GameObject>();
		foreach(GameObject monastery in monasteryArray)
		{
			MonasteryScript monScript = (MonasteryScript) monastery.GetComponent(typeof(MonasteryScript));
			if(monScript.underAttack == true)
			{
				monasteryUnderAttackArray.Add (monastery);
			}
            if (monScript.health <= 0)
            {
                monasteryArray.Remove(monastery);
                Destroy(monastery);
            }
            else newMonArray.Add(monastery);
		}
        monasteryArray = newMonArray;

		roundTime += Time.deltaTime;
		if (roundTime > 120) {
			Debug.Log ("Round exceeded two minutes, restarting");
            tieRounds++;
			EmptyArrays ();
			StoreData ();
			Initialize ();
		}
		else if (barray.Count <= 3) {
			Debug.Log("Barbarians Lost");
            knightRounds++;
			EmptyArrays();
			StoreData();
			Initialize();
		}
		else if (karray.Count <= 3) {
			Debug.Log("Knights and Monks Lost! (Knight Deaths)");
            barbRounds++;
			EmptyArrays();
			StoreData();
			Initialize();
		}
		else if (marray.Count <= 3) {
			Debug.Log("Knights and Monks Lost! (Monk Deaths)");
            barbRounds++;
			EmptyArrays();
			StoreData();
			Initialize();
		}
	}

	void LoadData()
	{
		chroms.Clear();
		fitnessValues.Clear ();
		StreamReader instream = null;
		try{
			instream = new StreamReader("fitnessValues.txt");
			for(int i = 0; i < (monks + knights + barbarians); i++)
			{
				string line = instream.ReadLine();
				string[] tokens = (string[])(line.Split (' '));
				int chrom = int.Parse(tokens[0]);
				int fitness = (int)float.Parse(tokens[1]);
				chroms.Add(chrom);
				fitnessValues.Add(fitness);
			}
			instream.Close();
			//Debug.Log("Success");
		}
		catch{
			//Debug.Log("Failure");
		}
	}
	/*
	void ProcessData()
	{
		for (int i = 0; i < barbarians; i++) {
			Individual p1 = Select ();					// Get 2 parents
			Individual p2 = Select ();
			uint c1 = p1.Chrom;							// Extract their chromosomes
			uint c2 = p2.Chrom;
			
			if (Util.rand.NextDouble () < CROSSOVER_PROB) // Probably do crossover
			{
				uint kidChrom = CrossOver (c1, c2);		// Make new chromosome
				Individual newDude = new Individual (kidChrom, nBits); // Make Individual
				newDude.Mutate ();						// Maybe mutate a bit
				return newDude;							// Send it back
			}
			else
				// No crossover => Pick one of the parents to return unchanged
				return (Util.rand.NextDouble() < 0.5 ? p1 : p2);
		}
		for (int i = 0; i < knights; i++) {
			GameObject p1 = karray[Random.Range(0,knights)];				// Get 2 parents
			GameObject p2 = karray[Random.Range(0,knights)];
			uint c1 = p1.GetComponent<KnightScript>().chrom;							// Extract their chromosomes
			uint c2 = p2.GetComponent<KnightScript>().chrom;
			
			if (Random.Range(0,100)/100.0 < 0.9) // Probably do crossover
			{
				int xOverPt = Util.rand.Next (0, nBits);	// Pick random crossover point
				p1 = (p1 >> xOverPt) << xOverPt;			// Get p1's bits to the left
				p2 = (p2 << (32 - xOverPt)) >> (32 - xOverPt); // p2's to the right
				uint newKidChrom = p1 | p2;						// Or them together// Make new chromosome
				chroms.Add (newKidChrom);

				//Individual newDude = new Individual (kidChrom, nBits); // Make Individual
				//newDude.Mutate ();						// Maybe mutate a bit
				GameObject pResult = (Random.Range(0,100)/100.0 < 0.5 ? p1 : p2);						// Send it back
			}
			else
			{
				// No crossover => Pick one of the parents to return unchanged
				GameObject pResult = (Random.Range(0,100)/100.0 < 0.5 ? p1 : p2);
				//fitnessValues.Add(
			}
		}
		for (int i = 0; i < monks; i++) {
			Individual p1 = Select ();					// Get 2 parents
			Individual p2 = Select ();
			uint c1 = p1.Chrom;							// Extract their chromosomes
			uint c2 = p2.Chrom;
			
			if (Util.rand.NextDouble () < CROSSOVER_PROB) // Probably do crossover
			{
				uint kidChrom = CrossOver (c1, c2);		// Make new chromosome
				Individual newDude = new Individual (kidChrom, nBits); // Make Individual
				newDude.Mutate ();						// Maybe mutate a bit
				return newDude;							// Send it back
			}
			else
				// No crossover => Pick one of the parents to return unchanged
				return (Util.rand.NextDouble() < 0.5 ? p1 : p2);
		}
	}
	*/
	void StoreData()
	{
		StreamWriter outStream = new StreamWriter ("fitnessValues.txt",false);
		foreach (string i in roundInfo) {
			outStream.WriteLine(i);
		}
		outStream.Close ();
		roundInfo.Clear ();

		bBayes.WriteObsTab ("BarbBayes.txt");
	}

	void EmptyArrays()
	{
		Destroy (player);
		foreach(GameObject b in barray)
		{
			if(b != null)
			{
				roundInfo.Add((b.GetComponent<BarbarianScript>().chrom).ToString() + " " + (b.GetComponent<BarbarianScript>().timeSurvived * 1.1).ToString());
				Destroy (b);
			}
		}
		foreach(GameObject k in karray)
		{
			if(k != null)
			{
				roundInfo.Add((k.GetComponent<KnightScript>().chrom).ToString() + " " + (k.GetComponent<KnightScript>().timeSurvived * 1.1).ToString());
				Destroy (k);
			}
		}
		foreach(GameObject m in marray)
		{
			if(m != null)
			{
				roundInfo.Add((m.GetComponent<MonkScript>().chrom).ToString() + " " + (m.GetComponent<MonkScript>().timeSurvived * 1.5).ToString());
				Destroy (m);
			}
		}
		foreach (GameObject mon in monasteryArray) 
		{
			if (mon != null) 
			{
				Destroy (mon);
			}
		}
		monasteryArray.Clear ();
		barray.Clear ();
		marray.Clear ();
		karray.Clear ();
	}

	void Initialize() {
		//Debug.Log ("Running Initialize");
		int creationCount = 0;
		Vector3 playerPos = new Vector3 (Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
		player = (GameObject)Instantiate (playerFab, playerPos, Quaternion.identity);
		for (int i = 0; i < monastaries; i++) {
			GameObject newMon = (GameObject)Instantiate(monasteryFab, new Vector3(monPlace[i, 0], 1, monPlace[i, 1]), Quaternion.identity);
			monasteryArray.Add (newMon);
		}
		
		bStateM = new StateMachine ("BarbarianStateMachine.txt");

		for (int i = 0; i < barbarians; i++) {
			//Create barbarians at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			try{
				//Debug.Log("test barb fit");
				barbarianFab.GetComponent<BarbarianScript>().fitnessValue = fitnessValues[creationCount];
				barbarianFab.GetComponent<BarbarianScript>().chrom = chroms[creationCount];
			}
			catch{
				Debug.Log("test barb fit FAILED");
				barbarianFab.GetComponent<BarbarianScript>().fitnessValue = 10;
				barbarianFab.GetComponent<BarbarianScript>().chrom = 10;
			}
			GameObject barb = (GameObject)Instantiate(barbarianFab, pos, Quaternion.identity);
			barray.Add(barb);
			creationCount++;
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			try
			{
				//Debug.Log("test knight fit");
				knightFab.GetComponent<KnightScript>().fitnessValue = fitnessValues[creationCount];
				knightFab.GetComponent<KnightScript>().chrom = chroms[creationCount];
			}
			catch{
				Debug.Log("test knight fit FAILED");
				knightFab.GetComponent<KnightScript>().fitnessValue = 10;
				knightFab.GetComponent<KnightScript>().chrom = 10;
			}
			GameObject knight = (GameObject)Instantiate(knightFab, pos, Quaternion.identity);
			karray.Add(knight);
			creationCount++;
		}
		for (int i = 0; i < monks; i++) {
			//Create monks at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			try
			{
				//Debug.Log("test monk fit");
				monkFab.GetComponent<MonkScript>().fitnessValue = fitnessValues[creationCount];
				monkFab.GetComponent<MonkScript>().chrom = chroms[creationCount];
			}
			catch{
				Debug.Log("test monk fit FAILED");
				monkFab.GetComponent<MonkScript>().fitnessValue = 10;
				monkFab.GetComponent<MonkScript>().chrom = 10;
			}
			GameObject monk = (GameObject)Instantiate(monkFab, pos, Quaternion.identity);
			marray.Add(monk);
			creationCount++;
		}
		roundNumber++;
		roundTime = 0;
        bBayes = new BayesBarb();
		bBayes.ReadObsTab ("BarbBayes.txt");
		bBayes.BuildStats ();
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
	
	public Vector3 Wander (Vector3 pos, Vector3 forward, float speed, float wanderD, float wanderR) {
		float randomAngle = Random.Range (0, Mathf.PI*2);
		
		Vector3 circleLoc = forward;
		circleLoc *= wanderD;
		circleLoc += pos;
		
		
		Vector3 circleOffSet = new Vector3(Mathf.Cos (randomAngle)*wanderR, 0, Mathf.Sin (randomAngle)*wanderR);
		Vector3 target = circleLoc + circleOffSet;
		
		//Debug.DrawLine (pos, target, Color.green);

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