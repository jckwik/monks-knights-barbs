using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public int monastaries;
	public int barbarians;
	public int knights;
	public int monks;

	public GameObject barbarianFab;
	public GameObject knightFab;
	public GameObject monkFab;
	public GameObject player;

	List<GameObject> barray = new List<GameObject> ();
	List<GameObject> karray = new List<GameObject> ();
	List<GameObject> marray = new List<GameObject> ();

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
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject barb = (GameObject)Instantiate(barbarianFab, pos, Quaternion.identity);
			barray.Add(barb);
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject knight = (GameObject)Instantiate(knightFab, pos, Quaternion.identity);
			karray.Add(knight);
		}
		for (int i = 0; i < monks; i++) {
			Vector3 pos = new Vector3(Random.Range(-10.0f, 10.0f), 1, Random.Range(-10.0f, 10.0f));
			GameObject monk = (GameObject)Instantiate(monkFab, pos, Quaternion.identity);
			marray.Add(monk);
			//Create monks at random locations
		}
	}
}
