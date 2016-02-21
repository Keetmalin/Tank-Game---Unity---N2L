using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MsgObject : MonoBehaviour {

	private String message;
	private DateTime time;
	
	public MsgObject(String msg ,  DateTime time) {
		this.message = msg;
		this.time = time;
	}
	
	public void setMessage(String msg) {
		this.message = msg;
	}
	public void setTime(DateTime time)
	{
		this.time = time;
	}
	public String getMessage()
	{
		return this.message;
	}
	public DateTime getTime()
	{
		return this.time;
	}
}
