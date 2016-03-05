using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour {

	public float coinTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		coinTime -= Time.deltaTime * 1000;
        if(coinTime <= 0)
        {
            Destroy(gameObject);
        }
	}
}
