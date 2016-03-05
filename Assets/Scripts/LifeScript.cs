using UnityEngine;
using System.Collections;

public class LifeScript : MonoBehaviour {

	public float lifeTime;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		lifeTime -= Time.deltaTime * 1000;
        if(lifeTime <= 0)
        {
            Destroy(gameObject);
        }
	}
}
