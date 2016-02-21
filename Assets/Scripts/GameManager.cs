using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

public class GameManager : MonoBehaviour {

	private MsgParser parser;
	private ClientClass networkClient;
	public GameObject brick;

	public void LoadGame(){
		//instantiate message passer
		parser = new MsgParser();
		//instantiate network client
		networkClient = new ClientClass(parser);
		networkClient.Sender("JOIN#");
		Application.LoadLevel(1);
	}
	public void LoadBricks(){
		//print ();
		Instantiate(GameObject.Find("brick"), new Vector3(0,0,0) , Quaternion.identity);
	}
}
