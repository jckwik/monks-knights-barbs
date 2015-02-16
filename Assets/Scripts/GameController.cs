using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public int monastaries;
	public int barbarians;
	public int knights;
	public int monks;

	public MovableObject barbarianFab;
	public MovableObject knightFab;
	public MovableObject monkFab;

	List<MovableObject> barray = new List<MovableObject> ();
	List<MovableObject> karray = new List<MovableObject> ();
	List<MovableObject> marray = new List<MovableObject> ();

	// Use this for initialization
	void Start () {
		Initialize ();
	}

	void Initialize() {

		for (int i = 0; i < monastaries; i++) {
			//Create monastaries
		}
		for (int i = 0; i < barbarians; i++) {
			//Create barbarians at random locations
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
			barray.Add(Instantiate(barbarianFab, pos, Quaternion.identity) as MovableObject);
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
			karray.Add(Instantiate(knightFab, pos, Quaternion.identity) as MovableObject);
		}
		for (int i = 0; i < monks; i++) {
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f));
			marray.Add(Instantiate(monkFab, pos, Quaternion.identity) as MovableObject);
			//Create monks at random locations
		}
	}
}
