using UnityEngine;
using System.Collections;

public class BarbarianScript : MonoBehaviour {
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

		velocity += gameController.Wander (this.transform.position, moveSpeed, 80, 50);
		
		velocity *= Time.deltaTime;
		direction = velocity.normalized * -1;
		lookAt ();
		this.transform.position += velocity;
		velocity = Vector3.zero;
	
	}
	
	void findTarget() {
		if (target == null) {
			target = gameController.player;
		}
	}
	void lookAt() {
		//direction = this.transform.position;
		this.transform.LookAt(direction, Vector3.up);
	}
}
