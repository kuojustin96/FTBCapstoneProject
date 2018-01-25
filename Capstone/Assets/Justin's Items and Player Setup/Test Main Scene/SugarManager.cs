    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarManager : MonoBehaviour {

    public static SugarManager instance = null;

    [SerializeField]
    private float timeUntilNewSugar = 10f;
    [SerializeField]
    private int numSugarPool = 200;
    [SerializeField]
    private GameObject sugarPrefab;
    [SerializeField]
    private int numSugarInLvl = 30;
    private List<GameObject> inactiveSugar = new List<GameObject>();
    private List<GameObject> activeSugar = new List<GameObject>();

    [System.Serializable]
    public class SugarSpawnSpot
    {
        public Transform spawn;
        //[HideInInspector]
        public GameObject sugarBeingHeld;
    }

    public SugarSpawnSpot[] spawnSpots;
    public List<SugarSpawnSpot> inactiveSpots = new List<SugarSpawnSpot>();
    public List<SugarSpawnSpot> activeSpots = new List<SugarSpawnSpot>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        SetUpSugar();
    }

    private void SetUpSugar()
    {
        for(int x = 0; x < numSugarPool; x++)
        {
            GameObject sugar = Instantiate(sugarPrefab, Vector3.zero, Quaternion.identity, transform);
            sugar.SetActive(false);
            inactiveSugar.Add(sugar);
        }

        foreach(SugarSpawnSpot s in spawnSpots)
        {
            inactiveSpots.Add(s);
        }

        int count = 0;
        if (spawnSpots.Length < numSugarInLvl)
            numSugarInLvl = spawnSpots.Length;

        while(count < numSugarInLvl)
        {
            int x = Random.Range(0, numSugarInLvl);

            if (!activeSpots.Contains(spawnSpots[x]))
            {

                inactiveSugar[0].transform.position = spawnSpots[x].spawn.position;
                inactiveSugar[0].SetActive(true);
                activeSugar.Add(inactiveSugar[0]);
                spawnSpots[x].sugarBeingHeld = inactiveSugar[0];
                activeSpots.Add(spawnSpots[x]);
                inactiveSpots.Remove(spawnSpots[x]);

                inactiveSugar.Remove(inactiveSugar[0]);

                count++;
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableNewSugar(GameObject sugar)
    {
        foreach(SugarSpawnSpot s in activeSpots)
        {
            if (s.sugarBeingHeld == sugar)
            {
                activeSpots.Remove(s);
                inactiveSpots.Add(s);
                s.sugarBeingHeld = null;
                StartCoroutine(EnableNewSugarCoroutine(s));
                return;
            }
        }
    }

    private IEnumerator EnableNewSugarCoroutine(SugarSpawnSpot s)
    {
        yield return new WaitForSeconds(timeUntilNewSugar);

        int x = Random.Range(0, inactiveSpots.Count);

        if (inactiveSugar.Count == 0)
        {
            GameObject sugar = Instantiate(sugarPrefab, inactiveSpots[x].spawn.position, Quaternion.identity);
            activeSugar.Add(sugar);
            inactiveSpots[x].sugarBeingHeld = sugar;
            activeSpots.Add(inactiveSpots[x]);
            inactiveSpots.Remove(inactiveSpots[x]);
        } else
        {
            inactiveSugar[0].transform.position = inactiveSpots[x].spawn.position;
            inactiveSpots[x].sugarBeingHeld = inactiveSugar[0];
            inactiveSugar[0].SetActive(true);
            activeSugar.Add(inactiveSugar[0]);
            inactiveSugar.Remove(inactiveSugar[0]);
            activeSpots.Add(inactiveSpots[x]);
            inactiveSpots.Remove(inactiveSpots[x]);
        }
    }

    public void DisableSugar(GameObject sugar)
    {
        activeSugar.Remove(sugar);
        inactiveSugar.Add(sugar);
        sugar.SetActive(false);
        sugar.transform.parent = transform;
    }
}
