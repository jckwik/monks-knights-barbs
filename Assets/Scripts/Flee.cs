using UnityEngine;
using System.Collections;

public class Flee : MonoBehaviour {
	public float moveSpeed;
	public float rotationSpeed;
	public GameObject target;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
		rotationSpeed = 5;
	}
	
	// Update is called once per frame
	void Update () {
		direction = this.transform.position - target.transform.position;
		this.transform.LookAt(target.transform, Vector3.up);
		this.transform.Translate(Vector3.forward * -moveSpeed * Time.deltaTime);
	}
}
