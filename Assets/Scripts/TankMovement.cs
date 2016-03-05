using UnityEngine;
using System.Collections;

public class TankMovement : MonoBehaviour {
	
	public float moveSpeed;
	public int direction;
	private Vector3 input;
	public float angle;
	public GameObject deathParticles;
	public GameObject brick;
	private Vector3 targetPosition;
	private float x;
	private float y;
	private float z;

		
	// Use this for initialization
	void Start () {

		angle =0;
		x = -05;
		y = 0;
		z = -15;
		targetPosition = new Vector3(x, y, z);
		//Instantiate(brick, targetPosition , Quaternion.identity);
		//GameManager gm= new GameManager();
		//gm.LoadBricks ();

	}
	
	// Update is called once per frame
	void Update () {

		//input = currentPosition;
		if (Input.GetKey("up")){
			//print(angle);
			if (direction != 0){
				if (angle ==0 ){
					angle = 270;
				}else if ( angle == 90){
					angle = 180;
				}else if ( angle == 180){
					angle = 90;
				}else{

					angle = 0;
					//input = new Vector3(0, 0, -1);
				}
			

				GetComponent<Rigidbody>().transform.Rotate(0,angle,0);
				angle = 270;
			}
			if (direction == 0){
				z+=10;
			}
			direction=0;
			targetPosition.Set(x,y,z);


		}
		
		if (Input.GetKey("down")){
			//print(angle);
			if (direction != 2){
				if (angle ==0 ){
					angle = 90;
				}else if ( angle == 90){
					angle = 0;
				}else if ( angle == 180){
					angle = 270;
				}else{
					angle = 180;
				}

				GetComponent<Rigidbody>().transform.Rotate(0,angle,0);
				angle = 90;
			}
			if (direction == 2){
				z-=10;
			}
			direction=2;
			targetPosition.Set(x,y,z);

		}
		if (Input.GetKey("right")){
			//print(angle);
			if (direction != 1){
				if (angle ==0 ){
					angle = 0;
				}else if ( angle == 90){
					angle = 270;
				}else if ( angle == 180){
					angle = 180;
				}else{
					angle = 90;
				}

				
				GetComponent<Rigidbody>().transform.Rotate(0,angle,0);
				angle =0;
			}
			if (direction == 1){
				x+=10;
			}
			direction=1;
			targetPosition.Set(x,y,z);

		}
		if (Input.GetKey("left")){
			//print(angle);
			if (direction != 3 ){
				if (angle ==0 ){
					angle = 180;
				}else if ( angle == 90){
					angle = 90;
				}else if ( angle == 180){
					angle = 0;
				}else{
					angle = 270;
				}
				direction=3;
				
				GetComponent<Rigidbody>().transform.Rotate(0,angle,0);
				angle  =180;
			}
			if (direction == 3){
				x-=10;
			}
			direction=3;
			targetPosition.Set(x,y,z);

		}
		//input =  new Vector3(Input.GetAxisRaw("Horizontal"),0 , Input.GetAxisRaw("Vertical"));
			
		//GetComponent<Rigidbody>().AddForce(input * moveSpeed);
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed);

		if (Input.GetKey("space")){

			new MissileScript(transform.position , moveSpeed);
			
		}
				
	}

	void OnCollisionEnter(Collision other){
		if (other.transform.tag == "water"){
			Instantiate(deathParticles, transform.position , Quaternion.identity);
		}
	}
	void upMethod(){

	}
	void downMethod(){
		
	}
	void leftMethod(){
		
	}
	void rightMethod(){
		
	}
}