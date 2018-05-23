using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour {

    public static ObjectPoolManager instance;

    [System.Serializable]
    public class PooledObjects
    {
        public string name;
        public GameObject objToPool;
        public int numToPool = 20;
        public Stack<GameObject> objPool = new Stack<GameObject>();
    }

    public PooledObjects[] ObjectPool;
    private Dictionary<string, PooledObjects> objPoolDict = new Dictionary<string, PooledObjects>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start () {
        PoolObjects();
	}

    public void PoolObjects()
    {
        foreach(PooledObjects p in ObjectPool)
        {
            for(int x = 0; x < p.numToPool; x++)
            {
                GameObject g = Instantiate(p.objToPool);
                g.transform.parent = transform;
                g.SetActive(false);
                p.objPool.Push(g);
            }

            objPoolDict.Add(p.name, p);
        }
    }

    public GameObject SpawnObject(string name, float size = 1f, float returnTime = -1)
    {
        PooledObjects p = objPoolDict[name];
        if (p.objPool.Count > 0)
        {
            GameObject g = p.objPool.Pop();
            g.transform.parent = null;
            g.transform.localScale *= size;
            g.SetActive(true);

            if (returnTime > 0)
                StartCoroutine(ReturnObject(name, g, returnTime));

            return g;
        }
        else
        {
            GameObject g = Instantiate(p.objToPool);
            g.transform.localScale *= size;

            if (returnTime > 0)
                StartCoroutine(ReturnObject(name, g, returnTime));

            return g;
        }
    }

    private IEnumerator ReturnObject(string name, GameObject g, float returnTime)
    {
        float saveTime = Time.time;
        while (Time.time < saveTime + returnTime)
            yield return null;

        RecycleObject(name, g);
    }

    public void RecycleObject(string name, GameObject g, float returnTime = -1)
    {
        if (returnTime > 0)
        {
            StartCoroutine(c_RecycleObject(name, g, returnTime));
        }
        else
        {
            g.SetActive(false);
            g.transform.position = Vector3.zero;
            g.transform.parent = null;
            g.transform.localScale = Vector3.one;
            g.transform.rotation = Quaternion.identity;
            PooledObjects p = objPoolDict[name];
            p.objPool.Push(g);
        }
    }

    private IEnumerator c_RecycleObject(string name, GameObject g, float returnTime)
    {
        float saveTime = Time.time;
        while (Time.time < saveTime + returnTime)
            yield return null;

        g.SetActive(false);
        g.transform.position = Vector3.zero;
        g.transform.parent = null;
        g.transform.localScale = Vector3.one;
        g.transform.rotation = Quaternion.identity;
        PooledObjects p = objPoolDict[name];
        p.objPool.Push(g);
    }
}
