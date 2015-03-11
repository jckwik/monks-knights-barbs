﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonasteryScript : MonoBehaviour {
	public bool underAttack;
	public GameController gameController;
	public List<GameObject> barray = new List<GameObject> ();

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		underAttack = false;
		barray = gameController.barray;
	}
	
	// Update is called once per frame
	void Update () {
		underAttack = false;
		foreach(GameObject b in barray)
		{
			if(Mathf.Sqrt((b.transform.position.x - this.transform.position.x) * (b.transform.position.x - this.transform.position.x)
			              + (b.transform.position.y - this.transform.position.y) * (b.transform.position.y - this.transform.position.y)) < 25)
			{
				underAttack = true;
			}
		}
	}
}
