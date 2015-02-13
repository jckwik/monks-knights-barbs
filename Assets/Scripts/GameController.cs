using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public int monastaries;
	public int barbarians;
	public int knights;
	public int monks;

	//Barbarian[] barray;
	//Knight[] karray;
	//Monk[] marray

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
		}
		for (int i = 0; i < knights; i++) {
			//Create knights at random locations
		}
		for (int i = 0; i < monks; i++) {
			//Create monks at random locations
		}
	}
}
