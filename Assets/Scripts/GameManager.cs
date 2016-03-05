using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Assets.AI_old;

public class GameManager : MonoBehaviour {

	public Rigidbody tank1_prefab;
	public Rigidbody tank2_prefab;
	public Rigidbody tank3_prefab;
	public Rigidbody tank4_prefab;
	public Rigidbody tank5_prefab;
	public static Rigidbody[] brickWall = new Rigidbody[25];
	public static Rigidbody[] stoneWall = new Rigidbody[25];
	public static Rigidbody[] water = new Rigidbody[25];
	public static GameObject[] tank = new GameObject[5];
	
	private Boolean player0 = true;
	private Boolean player1 = true;
	private Boolean player2 = true;
	private Boolean player3 = true;
	private Boolean player4 = true;

	private MsgParser parser;
	private ClientClass networkClient;
	
	//will update the GUI at the initiation
	private string[,] map;

	//store the brick objects
	private GameObject[,] brickMap;
	
	//store the directions of the players
	private int[] playerDirections;
	
	//will store details of the five players
	private string[,] playerDetails;
	
	//will store the brick health and coin time
	private string[,] mapHealth;	
	
	//create gameobjects for scoreBoard
	GameObject Entry0;
	GameObject Entry1;
	GameObject Entry2;
	GameObject Entry3;
	GameObject Entry4;
	
	//create AI variable
	//AI aiObject = new AI();
	
	//variable to detect game has started
	private bool gameRunning =true;
	

	// Use this for initialization
	void Start () {	
	
		Entry0 = GameObject.Find("Entry0"); 
		Entry1 = GameObject.Find("Entry1");
		Entry2 = GameObject.Find("Entry2");
		Entry3 = GameObject.Find("Entry3");
		Entry4 = GameObject.Find("Entry4");
	
		parser = new MsgParser();
		//instantiate network client
		networkClient = new ClientClass(parser);
		networkClient.Sender("JOIN#");
		print ("game started");

		//initialize map
		map = new string[Constant.MAP_SIZE, Constant.MAP_SIZE];
		for (int i = 0; i < Constant.MAP_SIZE; i++)
		{
			for (int j = 0; j < Constant.MAP_SIZE; j++)
				map[i, j] = Constant.EMPTY;
		}
		
		//initialize brick map
		brickMap = new GameObject[Constant.MAP_SIZE, Constant.MAP_SIZE];
		
		//initialize player details
		playerDetails = new string[5, 5];
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
				playerDetails[i, j] = "-";
		}
		
		//initialize map health
		mapHealth = new string[Constant.MAP_SIZE, Constant.MAP_SIZE];
		for (int i = 0; i < Constant.MAP_SIZE; i++)
		{
			for (int j = 0; j < Constant.MAP_SIZE; j++)
				mapHealth[i, j] = Constant.EMPTY;
		}
		playerDirections =  new int[5];
		for (int i = 0; i < 5; i++)
		{
			playerDirections[i] = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		KeyboardInput();
		Decoder ();
		
		
	}
	
	void KeyboardInput()
	{
		if (Input.GetKeyUp(KeyCode.UpArrow))
			networkClient.Sender("UP#");     //Tank movement up
		if (Input.GetKeyUp(KeyCode.DownArrow))
			networkClient.Sender("DOWN#");   //Tank movement down
		if (Input.GetKeyUp(KeyCode.LeftArrow))
			networkClient.Sender("LEFT#");   //Tank movement left
		if (Input.GetKeyUp(KeyCode.RightArrow))
			networkClient.Sender("RIGHT#");  //Tank movement right
		if (Input.GetKeyUp(KeyCode.Space))
			networkClient.Sender("SHOOT#");  //Tank shoot
	}

	void Decoder(){

		map = parser.getMap();
		playerDetails = parser.getPlayerDetails();
		updateScoreBoard();
		mapHealth = parser.getMapHealth();
		playerDirections = parser.getPlayerDirections();
		updateMap ();

		
		
		
	}

	void updateMap()
	{
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				

				Vector3 position = new Vector3(-45f + 10 * i, 5f, 45f - 10 * j);
				int direction= 0 ;
				

				switch(map[i, j]){

					
					//case Constant.EMPTY:
					//	Instantiate(Resources.Load("Empty"), position, Quaternion.identity);
					//	break;

					case Constant.STONE:
						Instantiate(Resources.Load("Stone"), position, Quaternion.identity);
						break;

					case Constant.BRICK:
						instantiateBrick(i,i,position);						
						break;

					case Constant.WATER:
						position = new Vector3(-45f + 10 * i, 2f, 45f - 10 * j);
						Instantiate(Resources.Load("Water"), position, Quaternion.identity);
						break;

					case Constant.PLAYER_0:
						//print("PLAYER_0");
						direction = playerDirections[0];
						position = new Vector3(-45f + 10 * i, 0f, 45f - 10 * j);
						if (player0){
							createTank(0, position, direction);
						}			
						else{
							//print (tank[0]);
							tank[0].transform.position = position;
							rotateTank(0,direction);
						}
						break;

					case Constant.PLAYER_1:
						//print("PLAYER_1");
						direction = playerDirections[1];
						position = new Vector3(-45f + 10 * i, 0f, 45f - 10 * j);
						
						if (player1){
							createTank(1, position, direction);
						}	
						else{
							tank[1].transform.position = position;
							rotateTank(1,direction);
						}
						break;

					case Constant.PLAYER_2:
						//print("PLAYER_2");
						direction = playerDirections[2];
						position = new Vector3(-45f + 10 * i, 0f, 45f - 10 * j);
						
						if (player2){
							createTank(2, position, direction);
						}	
						else{
							tank[2].transform.position = position;
							rotateTank(2,direction);
						}
						break;

					case Constant.PLAYER_3:
						//print("PLAYER_3");
						direction = playerDirections[3];
						position = new Vector3(-45f + 10 * i, 0f, 45f - 10 * j);
						
						if (player3){
							createTank(3, position, direction);
						}	
						else{
							tank[3].transform.position = position;
							rotateTank(3,direction);
						}
						break;

					case Constant.PLAYER_4:
						//print("PLAYER_4");
						direction = playerDirections[4];
						position = new Vector3(-45f + 10 * i, 0f, 45f - 10 * j);
						if (player4){
							createTank(4, position, direction);
						}	 
						
						else{
							tank[4].transform.position = position;
							rotateTank(4,direction);
						}
						
						break;
					
					case Constant.LIFE:
						String healthValue = 	mapHealth[i,j];
						GameObject life = Instantiate(Resources.Load("Life"), position, Quaternion.identity) as GameObject;
						life.GetComponent<LifeScript>().lifeTime = (float) Int32.Parse(healthValue);
						break;

					case Constant.COIN:
						String coinValue = mapHealth[i,j];
						GameObject coin = Instantiate(Resources.Load("Coin"), position, Quaternion.identity) as GameObject;
						coin.GetComponent<CoinScript>().coinTime = (float) Int32.Parse(coinValue);
						break;
					/*
					default:
						Instantiate(Resources.Load("Empty"), position, Quaternion.identity);
						break;*/
				}
				

			}
		}
		
		tankMovements();
	}

	void createTank(int tankID, Vector3 position, int direction)
	{
		switch (tankID)
		{
		case 0:
		{
			GameObject g = Instantiate(Resources.Load("Tank0"), position, Quaternion.identity) as GameObject;
			tank[0] = g;
			
			//tank[0] = Instantiate(Resources.Load("Tank0"), position, Quaternion.identity) as Rigidbody;
			player0 = false;
			//print (tank[0]);
			break;
		}
		case 1:
		{
			GameObject g = Instantiate(Resources.Load("Tank1"), position, Quaternion.identity) as GameObject;
			tank[1] = g;
			player1 = false;
			break;
		}
		case 2:
		{
			GameObject g = Instantiate(Resources.Load("Tank2"), position, Quaternion.identity) as GameObject;
			tank[2] = g;
			//tank[2] = Instantiate(Resources.Load("Tank2"), position, Quaternion.identity) as Rigidbody;
			player2 = false;
			break;
		}
		case 3:
		{
			GameObject g = Instantiate(Resources.Load("Tank3"), position, Quaternion.identity) as GameObject;
			tank[3] = g;
			//tank[3] = Instantiate(Resources.Load("Tank3"), position, Quaternion.identity) as Rigidbody;
			player3 = false;
			break;
		}
		case 4:
		{
			GameObject g = Instantiate(Resources.Load("Tank4"), position, Quaternion.identity) as GameObject;
			tank[4] = g;
			//tank[4] = Instantiate(Resources.Load("Tank4"), position, Quaternion.identity) as Rigidbody;
			player4 = false;
			break;
		}
		default:
		{
			break;
		}
		}
		rotateTank(tankID, direction);     
	}
	
	void rotateTank(int tankID, int direction)
	{
		Vector3 rotation;
		switch (direction)
		{
		case 0:
		{
			rotation = new Vector3(0f, -90f, 0f);
			break;
		}
		case 1:
		{
			rotation = new Vector3(0f, 0f, 0f);
			break;
		}
		case 2:
		{
			rotation = new Vector3(0f, 90f, 0f);
			break;
		}
		case 3:
		{
			rotation = new Vector3(0f, 180f, 0f);
			break;
		}
		default:
		{
			rotation = new Vector3(0f, -90f, 0f);
			break;
		}
		}
		tank[tankID].transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
	}


	void updateScoreBoard(){
		
		Entry0.transform.Find("Direction").GetComponent<Text>().text = playerDetails[0,0];
		Entry0.transform.Find("Shot").GetComponent<Text>().text = playerDetails[0,1];
		Entry0.transform.Find("Health").GetComponent<Text>().text = playerDetails[0,2];
		Entry0.transform.Find("Coins").GetComponent<Text>().text = playerDetails[0,3];
		Entry0.transform.Find("Points").GetComponent<Text>().text = playerDetails[0,4];
		
		Entry1.transform.Find("Direction").GetComponent<Text>().text = playerDetails[1,0];
		Entry1.transform.Find("Shot").GetComponent<Text>().text = playerDetails[1,1];
		Entry1.transform.Find("Health").GetComponent<Text>().text = playerDetails[1,2];
		Entry1.transform.Find("Coins").GetComponent<Text>().text = playerDetails[1,3];
		Entry1.transform.Find("Points").GetComponent<Text>().text = playerDetails[1,4];
		
		Entry2.transform.Find("Direction").GetComponent<Text>().text = playerDetails[2,0];
		Entry2.transform.Find("Shot").GetComponent<Text>().text = playerDetails[2,1];
		Entry2.transform.Find("Health").GetComponent<Text>().text = playerDetails[2,2];
		Entry2.transform.Find("Coins").GetComponent<Text>().text = playerDetails[2,3];
		Entry2.transform.Find("Points").GetComponent<Text>().text = playerDetails[2,4];
		
		Entry3.transform.Find("Direction").GetComponent<Text>().text = playerDetails[3,0];
		Entry3.transform.Find("Shot").GetComponent<Text>().text = playerDetails[3,1];
		Entry3.transform.Find("Health").GetComponent<Text>().text = playerDetails[3,2];
		Entry3.transform.Find("Coins").GetComponent<Text>().text = playerDetails[3,3];
		Entry3.transform.Find("Points").GetComponent<Text>().text = playerDetails[3,4];
		
		Entry4.transform.Find("Direction").GetComponent<Text>().text = playerDetails[4,0];
		Entry4.transform.Find("Shot").GetComponent<Text>().text = playerDetails[4,1];
		Entry4.transform.Find("Health").GetComponent<Text>().text = playerDetails[4,2];
		Entry4.transform.Find("Coins").GetComponent<Text>().text = playerDetails[4,3];
		Entry4.transform.Find("Points").GetComponent<Text>().text = playerDetails[4,4];
		
	}
	
	
	void instantiateBrick(int i, int j , Vector3 position ){
		String value = mapHealth[i,j];
		//print(value);
		if ( value.Equals("") || mapHealth[i,j].Equals("0") ){
			brickMap[i,j] = Instantiate(Resources.Load("Brick"), position, Quaternion.identity) as GameObject;
			//Instantiate(Resources.Load("Brick"), position, Quaternion.identity);
		}
		else if ( value.Equals("1")){
			print("75%");
			Destroy(brickMap[i,j]);
			brickMap[i,j] = Instantiate(Resources.Load("Brick75"), position, Quaternion.identity) as GameObject;
		}
		else if ( value.Equals("2")){
			Destroy(brickMap[i,j]);
			brickMap[i,j] = Instantiate(Resources.Load("Brick50"), position, Quaternion.identity) as GameObject;
		}
		else if ( value.Equals("3")){
			Destroy(brickMap[i,j]);
			brickMap[i,j] = Instantiate(Resources.Load("Brick25"), position, Quaternion.identity) as GameObject;
		}
		else if ( value.Equals("4")){
			Destroy(brickMap[i,j]);
			brickMap[i,j] = Instantiate(Resources.Load("Brick0"), position, Quaternion.identity) as GameObject;
		}
	}
	
	void tankMovements(){
		
		string playerName = MsgParser.playerName;
		int x=0;
		int y=0;
		for (int i = 0; i < Constant.MAP_SIZE; i++)
		{
			for (int j = 0; j < Constant.MAP_SIZE; j++){
				string temp = map[i,j];
				if (temp.Equals(playerName)){
					x=i;
					y=j;
					break;
				}
			}
				
		}
		char[] charArray = playerName.ToCharArray();

		int playerCount = 0;
		//print (playerName);
		
		//shooting at players
		for (int i = 0; i < Constant.MAP_SIZE; i++)
		{
			string temp2 = map[x,i];
			if ((temp2.Equals(Constant.PLAYER_0) || temp2.Equals(Constant.PLAYER_1) || temp2.Equals(Constant.PLAYER_2) || 
				temp2.Equals(Constant.PLAYER_3) || temp2.Equals(Constant.PLAYER_4)) && !temp2.Equals(playerName)){
					
					//print (i);
					if ( i < y ){
						if (playerDirections[playerCount] != 3){
							networkClient.Sender("LEFT#");
						}
						else{
							networkClient.Sender("SHOOT#");
						}
					}
					if ( i > y ){
						//print("yes");
						if (playerDirections[playerCount] != 1){
							networkClient.Sender("RIGHT#");
						}
						else{
							networkClient.Sender("SHOOT#");
						}
					}
			}
			temp2 = map[i,y];
			if((temp2.Equals(Constant.PLAYER_0) || temp2.Equals(Constant.PLAYER_1) || temp2.Equals(Constant.PLAYER_2) || 
				temp2.Equals(Constant.PLAYER_3) || temp2.Equals(Constant.PLAYER_4)) && !temp2.Equals(playerName)){
				
				if ( i < x ){
						if (playerDirections[playerCount] != 0){
							networkClient.Sender("UP#");
						}
						else{
							networkClient.Sender("SHOOT#");
						}
					}
					if ( i > x ){
						//print("yes");
						if (playerDirections[playerCount] != 2){
							networkClient.Sender("DOWN#");
						}
						else{
							networkClient.Sender("SHOOT#");
						}
					}
			}
			
			//random movements
			else{
				if (x == 0 && y ==0){
					
				}
			}
				
		}
		
		
		
		
	}
	
	
	
	
	
	
	
	
	
	
	
	
}