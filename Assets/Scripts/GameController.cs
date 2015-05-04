﻿using UnityEngine;
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

	public StateMachine bStateM;

	public List<GameObject> barray = new List<GameObject> ();
	public List<GameObject> karray = new List<GameObject> ();
	public List<GameObject> marray = new List<GameObject> ();
	public GameObject[] monasteryArray;
	public List<string> roundInfo = new List<string>();
	public List<int> chroms = new List<int> ();
	public List<int> fitnessValues = new List<int> ();

	public int roundNumber;

	// Use this for initialization
	void Start () {
		roundNumber = 0;
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
		if (barray.Count <= 3) {
			EmptyArrays();
			StoreData();
			Initialize();
		}
		else if (karray.Count <= 3) {
			EmptyArrays();
			StoreData();
			Initialize();
		}
		else if (marray.Count <= 3) {
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
				int fitness = int.Parse(tokens[0]);
				chroms.Add(chrom);
				fitnessValues.Add(fitness);
			}
			instream.Close();
		}
		catch{

		}
	}

	void StoreData()
	{
		StreamWriter outStream = new StreamWriter ("fitnessValues.txt",false);
		foreach (string i in roundInfo) {
			outStream.WriteLine(i);
		}
		outStream.Close ();
		roundInfo.Clear ();
	}

	void EmptyArrays()
	{
		Destroy (player);
		foreach(GameObject b in barray)
		{
			if(b != null)
			{
				roundInfo.Add((b.GetComponent<BarbarianScript>().chrom).ToString() + " " + (b.GetComponent<BarbarianScript>().timeSurvived * 1.5).ToString());
				Destroy (b);
			}
		}
		foreach(GameObject k in karray)
		{
			if(k != null)
			{
				roundInfo.Add((k.GetComponent<KnightScript>().chrom).ToString() + " " + (k.GetComponent<KnightScript>().timeSurvived * 1.5).ToString());
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
		barray.Clear ();
		marray.Clear ();
		karray.Clear ();
	}

	void Initialize() {
		Debug.Log ("Running Initialize");
		int creationCount = 0;
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
			barb.GetComponent<BarbarianScript>().fitnessValue = fitnessValues[creationCount];
			barb.GetComponent<BarbarianScript>().chrom = chroms[creationCount];
			barray.Add(barb);
			creationCount++;
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			GameObject knight = (GameObject)Instantiate(knightFab, pos, Quaternion.identity);
			knight.GetComponent<KnightScript>().fitnessValue = fitnessValues[creationCount];
			knight.GetComponent<KnightScript>().chrom = chroms[creationCount];
			karray.Add(knight);
			creationCount++;
		}
		for (int i = 0; i < monks; i++) {
			//Create monks at random locations
			Vector3 pos = new Vector3(Random.Range(-225.0f, 225.0f), 1, Random.Range(-225.0f, 225.0f));
			GameObject monk = (GameObject)Instantiate(monkFab, pos, Quaternion.identity);
			monk.GetComponent<MonkScript>().fitnessValue = fitnessValues[creationCount];
			monk.GetComponent<MonkScript>().chrom = chroms[creationCount];
			marray.Add(monk);
			creationCount++;
		}
		monasteryArray = GameObject.FindGameObjectsWithTag("Monastery");
		roundNumber++;
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