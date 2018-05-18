using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public struct HatList
{
    public List<GameObject> list;
}

public class NetworkOutfitScript : NetworkBehaviour
{

    public Transform HatRoot;

    public HatList Hats;

    [SyncVar]
    public int currentHat = 0;

    // Use this for initialization
    void Start()
    {

    }

    public void PopulateHats()
    {
        Hats.list.Clear();
        foreach (Transform child in HatRoot)
        {
            Hats.list.Add(child.gameObject);
        }
    }

    [Command]
    public void CmdChangeHat(int index)
    {
        RpcChangeHat(index);
    }

    [ClientRpc]
    void RpcChangeHat(int index)
    {

        Hats.list[currentHat].SetActive(false);
        currentHat = index % Hats.list.Count;
        Hats.list[currentHat].SetActive(true);

        Debug.Log("You have hat " + currentHat);
    }

    public void ChangeHat(int num, Color pColor, bool doColor = true)
    {
        Hats.list[currentHat].SetActive(false);
        currentHat = num % Hats.list.Count;
        Hats.list[currentHat].SetActive(true);

        if (!doColor)
        {
            return;
        }
        Renderer[] rends = HatRoot.GetComponentsInChildren<Renderer>();

        foreach(Renderer r in rends)
        {
            r.material.color = pColor;
        }
        Debug.Log("You have hat " + currentHat);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
