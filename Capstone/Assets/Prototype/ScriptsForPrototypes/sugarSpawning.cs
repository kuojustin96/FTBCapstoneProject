using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum Colors {
	Red = 0,
	Blue = 1,
	Yellow = 2,
	Green = 3,
	Cyan = 4,
	Magenta = 5,
}

public class sugarSpawning : NetworkBehaviour {
    public static GameObject sugarSpawn;    

	public bool Alive;
	public bool enableRandomColors = false;
	public float SpawnTime;
	private float curTime;
	public GameObject sugarPrefab;
	private Dictionary<int, Color> colorDict = new Dictionary<int, Color>();

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

		colorDict.Add (0, Color.red);
		colorDict.Add (1, Color.blue);
		colorDict.Add (2, Color.yellow);
		colorDict.Add (3, Color.green);
		colorDict.Add (4, Color.cyan);
		colorDict.Add (5, Color.magenta);
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

	private void PickRandomColor(GameObject g){
		Material m = g.GetComponent<MeshRenderer> ().material;
		int rand = Random.Range (0, System.Enum.GetValues (typeof(Colors)).Length);
		m.color = colorDict [rand];
	}

	[Command]
	public void CmdSpawn(){
		GameObject sugar = Instantiate(sugarPrefab, this.transform.position, Quaternion.identity,sugarSpawn.transform);
		if (enableRandomColors)
			PickRandomColor (sugar);
		NetworkServer.Spawn (sugar);
		curTime = 0;
		Alive = true;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "sugarPickup")
			Alive = false;
	}
}
