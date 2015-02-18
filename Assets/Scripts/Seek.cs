using UnityEngine;
using System.Collections;

public class Seek : MonoBehaviour {
	public float moveSpeed;
	public float rotationSpeed;
	public GameObject target;
	public Vector3 direction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		direction = target.transform.position - this.transform.position;
		this.transform.LookAt(direction, Vector3.up);
		this.transform.Translate(direction * moveSpeed * Time.deltaTime);
	}
}
