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
	//public List<string> roundInfo = new List<string>();
	//public List<int> chroms = new List<int> ();
	public List<int> fitnessValues = new List<int> ();
	public List<int> roundFitnessValues = new List<int> ();
	
	public int roundNumber;
    public int barbRounds = 0;
    public int knightRounds = 0;
    public int tieRounds = 0;
	public int[,] monPlace;
	public float roundTime;

	//GA
	ThreshPop tp;
	static int popSize = 30;				// Population size
	static int chromLeng = 10;              // Number of bits in a chromosome
	static int nChromVals = 1 << chromLeng; // Number of values for that many bits
	uint [] chroms;
	
    public AudioClip bDeath;
    public AudioClip mDeath;
    public AudioClip kDeath;

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
				roundFitnessValues.Add((int)((bScript.timeSurvived + bScript.roundKillCount * 2) * 1.1));
                //bDeath = bScript.dieSound;
                //AudioSource.PlayClipAtPoint(bDeath, b.transform.position);
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
				roundFitnessValues.Add((int)((kScript.timeSurvived + kScript.roundKillCount * 2) * 1.1));
                //kDeath = kScript.dieSound;
                //AudioSource.PlayClipAtPoint(kDeath, k.transform.position);
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
				roundFitnessValues.Add((int)((m.GetComponent<MonkScript>().timeSurvived * 1.5)));
                //mDeath = mScript.dieSound;
                //AudioSource.PlayClipAtPoint(mDeath, m.transform.position);
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
			LoadData();
			Initialize ();
		}
		else if (barray.Count <= 3) {
			Debug.Log("Barbarians Lost");
            knightRounds++;
			EmptyArrays();
			StoreData();
			LoadData();
			Initialize();
		}
		else if (karray.Count <= 3) {
			Debug.Log("Knights and Monks Lost! (Knight Deaths)");
            barbRounds++;
			EmptyArrays();
			StoreData();
			LoadData();
			Initialize();
		}
		else if (marray.Count <= 3) {
			Debug.Log("Knights and Monks Lost! (Monk Deaths)");
            barbRounds++;
			EmptyArrays();
			StoreData();
			LoadData();
			Initialize();
		}
	}

	void LoadData()
	{
		tp = new ThreshPop(chromLeng, popSize, "geneticInfo.txt");
		chroms = new uint[popSize];
		int i = 0;
		while (! tp.AllCheckedOut())
		{
			int fit = Fitness(Gen2Phen(chroms[i]));
			chroms[i] = tp.CheckOut();
			fitnessValues.Add (fit);
			i++;
		}
	}

	void StoreData()
	{
		int i = 0;
		while (! tp.AllCheckedIn())
		{
			int fit = Fitness(Gen2Phen(chroms[i]));	// Determine fitness
			tp.CheckIn(chroms[i], fit);	// CheckIn to next generation
			i++;
		}
		tp.WritePop();
		//Combine chroms and fitnessValues calculated in round
		StreamWriter outStream = new StreamWriter("geneticInfo.txt", false);
		for (i = 0; i < popSize; i++)
		{
			outStream.WriteLine (tp.newP.dudes[i].chrom + " " + roundFitnessValues[i]);
		}
		outStream.Close();
		roundFitnessValues.Clear ();

		bBayes.WriteObsTab ("BarbBayes.txt");
	}

	void EmptyArrays()
	{
		Destroy (player);
		foreach(GameObject b in barray)
		{
			if(b != null)
			{
				roundFitnessValues.Add((int)((b.GetComponent<BarbarianScript>().timeSurvived + b.GetComponent<BarbarianScript>().roundKillCount * 2) * 1.1));
				Destroy (b);
			}
		}
		foreach(GameObject k in karray)
		{
			if(k != null)
			{
				roundFitnessValues.Add((int)((k.GetComponent<KnightScript>().timeSurvived + k.GetComponent<KnightScript>().roundKillCount * 2) * 1.1));
				Destroy (k);
			}
		}
		foreach(GameObject m in marray)
		{
			if(m != null)
			{
				roundFitnessValues.Add((int)((m.GetComponent<MonkScript>().timeSurvived * 1.5)));
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
		fitnessValues.Clear ();
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
				barbarianFab.GetComponent<BarbarianScript>().chrom = (int)chroms[creationCount];
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
				knightFab.GetComponent<KnightScript>().chrom = (int)chroms[creationCount];
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
				monkFab.GetComponent<MonkScript>().chrom = (int)chroms[creationCount];
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
	
		public float Gen2Phen (uint gen)
	{
		float lb = 0.0f;			// Lower bound for threshold range in game
		float ub = 200.0f;			// Upper bound
		float step = (ub - lb) / nChromVals;	// Step size for chrom values
		return (gen * step + lb);
	}
	
	public int Fitness (float phen)
	{
		
		return (int) (phen * 2 + Random.Range(0,400));	// S/N = 1:1
	}

	public class Population
	{
		const double CROSSOVER_PROB = 0.9;	// 90% chance of crossover in BreedDude()
		int popSize;			// Population size
		int nBits;           	// Number of bits per chromosome
		int nChromVals;      	// Number of different chromosome values (2 to the nBits)
		public Individual [] dudes;	// Array of Individuals
		int nDudes = 0;			// Current number of Individuals
		int totFit = 0;      	// Total fitness for all individuals in population
		char[] delim = {' '};	// Used in ReadPop to split input lines
		
		// Constructor sets up an empty population with popN individuals
		//   and chromosome length of cLeng (sets iBits)
		public Population (int popN, int cLeng)
		{
			popSize = popN;
			dudes = new Individual[popSize];
			nDudes = 0;
			nBits = cLeng;
			nChromVals = 1 << nBits;
			totFit = 0;
		}
		
		// Returns true if population is full
		public bool Full
		{
			get { return nDudes == popSize; }
		}
		
		// Fills population with new random chromosomes for generation 0
		public void InitPop()
		{
			for (int i = 0; i < popSize; i++)
			{
				dudes[i] = new Individual ((uint) Random.Range(0,nChromVals), nBits);
			}
			nDudes = popSize;
			totFit = popSize;      // Default fitness for each Individual == 1
		}
		
		// Fills population by reading individuals from a file already opened to inStream
		// Assumes file is correctly formatted with correct number of lines
		public void ReadPop(StreamReader inStream)
		{
			for (int i = 0; i < popSize; i++)
			{
				string line = inStream.ReadLine();		// Read a line
				string [] tokens = line.Split (delim);	// Split into "words"
				uint chr = uint.Parse(tokens[0]);		// Convert words to numbers
				int fit = int.Parse(tokens[1]);
				dudes [i] = new Individual (chr, nBits, fit); // Put Individual in population
				totFit += fit;							// Accumulate total fitness for selection
			}
			nDudes = popSize;							// Show the population full
		}
		
		// Write the population out to a data file that can be read by ReadPop
		public void WritePop(StreamWriter outStream)
		{
			for (int i = 0; i < nDudes; i++)
			{
				outStream.WriteLine (dudes[i]);
			}
		}
		
		// Display the Population on the Console
		public void DisplayPop()
		{
			for (int i = 0; i < nDudes; i++)
			{
				Debug.Log (dudes [i]);
			}
			Debug.Log ("");
		}
		
		// Breed a new Individual using crossover and mutation
		public Individual BreedDude()
		{
			Individual p1 = Select ();					// Get 2 parents
			Individual p2 = Select ();
			uint c1 = p1.Chrom;							// Extract their chromosomes
			uint c2 = p2.Chrom;
			
			if (Random.value < CROSSOVER_PROB) // Probably do crossover
			{
				uint kidChrom = CrossOver (c1, c2);		// Make new chromosome
				Individual newDude = new Individual (kidChrom, nBits); // Make Individual
				newDude.Mutate ();						// Maybe mutate a bit
				return newDude;							// Send it back
			}
			else
				// No crossover => Pick one of the parents to return unchanged
				return (Random.value < 0.5 ? p1 : p2);
		}
		
		// Roulette-wheel selection selects in linear proportion to fitness
		// Uses totFit, which was accumulated when population was filled
		public Individual Select()
		{
			// Roll a random integer from 0 to totFit - 1
			int roll = Random.Range(0,totFit);
			
			// Walk through the population accumulating fitness
			int accum = dudes[0].Fitness;	// Initialize to the first one
			int iSel = 0;
			// until the accumulator passes the rolled value
			while (accum <= roll && iSel < nDudes-1)
			{
				iSel++;
				accum += dudes[iSel].Fitness;
			}
			// Return the Individual where we stopped
			return dudes[iSel];
		}
		
		// Find the best (highest fitness) Individual in the population
		// Used to implement elitism => best of old Pop survives in new
		public Individual BestDude ()
		{
			// Initialize to the first Individual in the array
			int whereBest = 0;			// Initialze to the first one
			int bestFit = dudes[0].Fitness;
			
			// Walk through the rest to get the overall best one
			for (int i = 1; i < nDudes; i++)
				if (dudes [i].Fitness > bestFit)
			{
				whereBest = i;
				bestFit = dudes [i].Fitness;
			}
			return dudes[whereBest];
		}
		
		// Add a new Individual to the population in the next open spot
		public int AddNewInd (Individual newDude)
		{
			int wherePut = -1;			// -1 in case something breaks
			if (Full)
				Debug.Log ("Panic!  Tried to add too many dudes");
			else
			{
				wherePut = nDudes;
				dudes[wherePut] = newDude;
				nDudes++;				// Increment for next time
			}
			return wherePut;			// Return offset in array where it landed
		}
		
		// Get Individual at offset where in the array
		public Individual GetDude (int where)
		{
			return dudes [where];
		}
		
		// Set fitness of Individual at offset where to fitVal
		public void SetFitness (int where, int fitVal)
		{
			dudes[where].Fitness = fitVal;
		}
		
		// Single-point crossover of two parents, returns new kid
		// Uses bit shift tricks to get each parent's contribution on opposite
		// sides of random crossover point
		uint CrossOver(uint p1, uint p2)
		{
			int xOverPt = Random.Range (0, nBits);	// Pick random crossover point
			p1 = (p1 >> xOverPt) << xOverPt;			// Get p1's bits to the left
			p2 = (p2 << (32 - xOverPt)) >> (32 - xOverPt); // p2's to the right
			uint newKid = p1 | p2;						// Or them together
			return newKid;
		}
	}

	public class Individual
	{
		const float MUT_PROB = 0.2f;	// Mutation probability
		int fitness;
		public uint chrom;		// up to 32-bit chromosome with an unsigned integer
		int nBits;		// Number of bits actually used (starting w/ least sig)
		
		// Called by InitPop() with no fitness value
		public Individual (uint newChrom, int nB)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = 1;	// Default fitness must be non-zero
		}
		
		// Overload called by ReadPop() with fitness from last generation
		public Individual (uint newChrom, int nB, int fit)
		{
			chrom = newChrom;
			nBits = nB;
			fitness = fit;
		}
		
		// Getters and a setter
		public uint Chrom
		{
			get { return this.chrom; }
		}
		
		public int Fitness
		{
			get { return this.fitness; }
			set { this.fitness = value; }
		}
		
		// Mutates a random bit MUT_PROB of the time
		public void Mutate ()
		{
			if (Random.value < MUT_PROB)
			{
				int mutPt = Random.Range(0, nBits);	// 0 to nBits - 1
				int mutMask = 1 << mutPt;		// Build mask of 1 at mutation point
				chrom = chrom ^ (uint)mutMask;	// xor the mask, which flips that bit
			}
		}
		
		// Make it easier to write an Individual
		public override string ToString()
		{
			return (chrom + " " + fitness);
		}
	}

	public class ThreshPop
	{
		int popSize;		// Number of Individuals in population
		int chromSize;		// Length of chromosome for each individual
		Population oldP;	// Old population read from file or generated randomly
		public Population newP;	// New population filled  as Individuals get fitness
		string popPath;		// String for data file path name (in Bin/Debug folder)
		int nextCOut = 0;	// Counter for number of Individuals checked out of oldP
		int nextCIn = 0;	// Counter for number of Individuals checked into of newP
		bool isGeneration0 = false;	// Assume there's a data file from a previous run
		
		// Constructor sets up old and new populations
		public ThreshPop (int cSize, int size, string path)
		{
			popSize = size;
			chromSize = cSize;
			popPath = path;
			oldP = new Population (popSize, chromSize);	// Old population for check out
			FillPop();
			newP = new Population (popSize, chromSize);	// New population for check in
		}
		
		// Fill oldP either from data file or from scratch (new, random)
		void FillPop ()
		{
			StreamReader inStream = null;	// Open file if it's there
			try
			{
				inStream = new StreamReader(popPath);
				oldP.ReadPop(inStream);		// File opened so read it
				inStream.Close();
			}
			catch
			{
				oldP.InitPop();			// File didn't open so fill with newbies
				isGeneration0 = true;	// Set flag to show it's generation 0
			}
		}
		
		public void WritePop()
		{
			StreamWriter outStream = new StreamWriter(popPath, false);
			newP.WritePop(outStream);
			outStream.Close();
		}
		
		// Display either oldP (0) or newP (1) on Console window
		public void DisplayPop(int which)
		{
			if (which == 0)
				oldP.DisplayPop();
			else
				newP.DisplayPop();
		}
		
		// Check out an individual to use for a threshold in an NPC
		public uint CheckOut ()
		{
			if (isGeneration0)	// Brand new => don't breed
			{
				Individual dude = oldP.GetDude(nextCOut);
				nextCOut++;
				return dude.Chrom;
			}
			else
			{	// Came from file so breed new one
				Individual newDude;
				if (nextCOut == 0)	// First one needs to be Best (elitism)
					newDude = oldP.BestDude();
				else
					newDude = oldP.BreedDude();	// Rest are bred
				nextCOut++;						// Count it
				return newDude.Chrom;			// Return its chromosome
			}
		}
		
		// Returns true if we've checked out a population's worth
		public bool AllCheckedOut()
		{
			return nextCOut == popSize;
		}
		
		// Check in an individual that has now acquired a fitness value
		public void CheckIn (uint chr, int fit)
		{
			Individual NewDude = new Individual(chr, chromSize, fit);	// Make Individual
			newP.AddNewInd(NewDude);						// Add to newP
			nextCIn++;										// Count it
			//Console.WriteLine("In CheckIn chr fit: " + chr + " " + fit);
		}
		
		// Returns true if newP is full of checked in Individuals
		public bool AllCheckedIn()
		{
			return nextCIn == popSize;
		}
	}
}