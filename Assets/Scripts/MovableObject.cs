using UnityEngine;
using System.Collections;

public class MovableObject : MonoBehaviour {

	Vector2 location;
	Vector2 velocity;
	Vector2 acceleration;
	
	// Use this for initialization
	void Start () {
		//location = Vector2.zero;
		velocity = Vector2.zero;
		acceleration = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		location += velocity;
		velocity += acceleration;
	}
}
