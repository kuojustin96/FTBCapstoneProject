using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class sugarSpawning : NetworkBehaviour {
    public static GameObject sugarSpawn;    

	public bool Alive;
	public float SpawnTime;
	private float curTime;
	public GameObject sugarPrefab;

    void Awake()
    {
        if (!sugarSpawn)
        {
            sugarSpawn = new GameObject("Sugar");
        }
    }

	// Use this for initialization
	void Start () {
		Alive = false;
        //curTime = 0;
        //SpawnTime = 5;
        curTime = SpawnTime - 1;
	}
	// Update is called once per frame
	void Update () {
		if (!isServer)
			return;
		if (curTime > SpawnTime) {
			CmdSpawn ();
			curTime = 0;
			Alive = true;
		}
		if(!Alive)
		curTime += Time.deltaTime;

	}
	[Command]
	public void CmdSpawn(){
		GameObject sugar = Instantiate(sugarPrefab, this.transform.position, Quaternion.identity,sugarSpawn.transform);
		NetworkServer.Spawn (sugar);
		curTime = 0;
		Alive = true;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "sugarPickup")
			Alive = false;
	}
}
