using UnityEngine;
using System.Collections;

public class MissileScript : MonoBehaviour {

	private Vector3 tankPosition;
	private Vector3 targetPosition;
	private float moveSpeed;

	public MissileScript(Vector3 tankPosition , float moveSpeed){
		this.tankPosition = tankPosition;
		this.moveSpeed = moveSpeed;
		targetPosition = new Vector3(10 , 0 , 0);
	

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = Vector3.MoveTowards(tankPosition, targetPosition, moveSpeed);

	}
}
