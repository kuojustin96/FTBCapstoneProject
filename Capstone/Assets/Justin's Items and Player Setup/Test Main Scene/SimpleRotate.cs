using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {

    private float origYPos;

	// Use this for initialization
	void Start () {
        origYPos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up);
        transform.position = new Vector3(transform.position.x, origYPos + Mathf.PingPong(Time.time, 1f), transform.position.z);
	}
}
