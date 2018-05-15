using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraMove : MonoBehaviour {

    public float speed = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * speed);

        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * speed);

        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * speed);

        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * speed);

        if (Input.GetKey(KeyCode.Q))
            transform.Translate(Vector3.forward * speed);

        if (Input.GetKey(KeyCode.E))
            transform.Translate(Vector3.back * speed);
    }
}
