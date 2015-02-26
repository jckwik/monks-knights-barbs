using UnityEngine;
using System.Collections;

public class KnightScript : MonoBehaviour {
	public float radius;
	public float moveSpeed;
	public float rotationSpeed;
	public GameObject target;
	public GameController gameController;
	public Vector3 direction;
	public Vector3 velocity;

	// Use this for initialization
	void Start () {
		GameObject gC = GameObject.Find("Game Controller");
		gameController = (GameController) gC.GetComponent(typeof(GameController));
		moveSpeed = 5;
		direction = Vector3.zero;
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		findTarget ();
		lookAt ();
		velocity += gameController.Arrive(this.transform.position, target.transform.position, moveSpeed, 15, 5);
		
		velocity *= Time.deltaTime;
		this.transform.position += velocity;
		velocity = Vector3.zero;
	}

	void findTarget() {
		if (target == null) {
			target = gameController.player;
		}
	}
	void lookAt() {
		direction = target.transform.position - this.transform.position;
		this.transform.LookAt(direction, Vector3.up);
	}
}
