using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SugarManager : NetworkBehaviour {

    public static SugarManager instance = null;

    [SerializeField]
    private float timeUntilNewSugar = 10f;
    [SerializeField]
    private int numSugarPool = 200;
    [SerializeField]
    private GameObject sugarPrefab;
    [SerializeField]
    private int numSugarInLvl = 30;
    [SerializeField]
    private float timeBetweenSugarDrop = 0.1f;
    private List<GameObject> inactiveSugar = new List<GameObject>();
    private List<GameObject> activeSugar = new List<GameObject>();

    [System.Serializable]
    public class SugarSpawnSpot
    {
        public Transform spawn;
        [HideInInspector]
        public GameObject sugarBeingHeld;
    }

    public SugarSpawnSpot[] spawnSpots;
    private List<SugarSpawnSpot> inactiveSpots = new List<SugarSpawnSpot>();
    private List<SugarSpawnSpot> activeSpots = new List<SugarSpawnSpot>();

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [Command]
    private void CmdSetUpSugar()
    {
        for(int x = 0; x < numSugarPool; x++)
        {
            GameObject sugar = Instantiate(sugarPrefab, Vector3.zero, Quaternion.identity, transform);
            NetworkServer.Spawn(sugar);
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
        CmdSetUpSugar();
    }

    [Command]
    public void CmdEnableNewSugar(GameObject sugar)
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

    [Command]
    public void CmdDropSugar(int num, Vector3 dropPosition)
    {
        StartCoroutine(DropSugarCoroutine(num, dropPosition));
    }

    private IEnumerator DropSugarCoroutine(int num, Vector3 dropPosition)
    {
        int counter = 0;
        Vector3 topPos = new Vector3(dropPosition.x, dropPosition.y + 1, dropPosition.z);
        while (counter < num)
        {
            GameObject g = inactiveSugar[0];
            g.transform.parent = null;
            //g.transform.localScale = Vector3.zero;
            g.SetActive(true);
            activeSugar.Add(g);
            inactiveSugar.Remove(g);

            Vector3 targetPos = dropPosition + (Random.onUnitSphere * 3f);
            //StartCoroutine(DropSugarMovement(g, targetPos, topPos));
            g.transform.position = targetPos;

            counter++;

            yield return new WaitForSeconds(timeBetweenSugarDrop);
        }
    }

    private IEnumerator DropSugarMovement(GameObject sugar, Vector3 targetPos, Vector3 topPos)
    {
        int counter = 0;
        while(counter < 10)
        {
            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, topPos, Time.deltaTime * 2f);
            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.one * 15, Time.deltaTime * 1f);
            counter++;
            yield return null;
        }

        counter = 0;

        while(counter < 10)
        {
            sugar.transform.position = Vector3.MoveTowards(sugar.transform.position, targetPos, Time.deltaTime * 2f);
            sugar.transform.localScale = Vector3.MoveTowards(sugar.transform.localScale, Vector3.one * 15, Time.deltaTime * 1f);
            counter++;
            yield return null;
        }

        sugar.transform.localScale = Vector3.one * 15;
    }

    private IEnumerator EnableNewSugarCoroutine(SugarSpawnSpot s)
    {
        yield return new WaitForSeconds(timeUntilNewSugar);

        int x = Random.Range(0, inactiveSpots.Count);

        if (inactiveSugar.Count == 0)
        {
            GameObject sugar = Instantiate(sugarPrefab, inactiveSpots[x].spawn.position, Quaternion.identity);
            NetworkServer.Spawn(sugar);
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
