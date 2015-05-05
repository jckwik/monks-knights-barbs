using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonasteryScript : MonoBehaviour {
	public bool underAttack;
	public GameController gameController;
	public List<GameObject> barray = new List<GameObject> ();

	public int health;
	public bool alive;

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		underAttack = false;
		barray = gameController.barray;
		alive = true;
		health = 100;
	}
	
	// Update is called once per frame
	void Update () {
		underAttack = false;
		if (!alive)
			return;
		if (health <= 0) 
			alive = !alive;
		barray = gameController.barray;
		foreach(GameObject b in barray)
		{
			if(Mathf.Sqrt((b.transform.position.x - this.transform.position.x) * (b.transform.position.x - this.transform.position.x)
			              + (b.transform.position.z - this.transform.position.z) * (b.transform.position.z - this.transform.position.z)) < 25)
			{
				underAttack = true;
			}
		}
	}
}
