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
    public int currentHat;

    [SyncVar]
    public Color theColor;

    // Use this for initialization
    void Start()
    {
        Debug.Log("AM I LOCAL? " + isLocalPlayer);
        
            ChangeHat(currentHat);
            ChangeColor(theColor);
        
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
        Debug.Log("Command!");
        //index = WrapNumber(index);

        //Hats.list[currentHat].SetActive(false);
        //currentHat = index;
        //Hats.list[currentHat].SetActive(true);


        RpcChangeHat(index);
    }

    [ClientRpc]
    void RpcChangeHat(int index)
    {

        Debug.Log("RPC!");
        index = WrapNumber(index);

        Hats.list[currentHat].SetActive(false);
        currentHat = index;
        Hats.list[currentHat].SetActive(true);

        Debug.Log("You have hat " + currentHat);
    }

    public void ChangeHat(int index)
    {
        index = WrapNumber(index);

        Hats.list[currentHat].SetActive(false);
        currentHat = index;
        Hats.list[currentHat].SetActive(true);

    }

    private int WrapNumber(int index)
    {
        if (index < 0)
        {
            index = Hats.list.Count - 1;
        }
        if (index >= Hats.list.Count)
        {
            index = 0;
        }

        return index;
    }

    public void ChangeColor(Color color)
    {
        theColor = color;
        Renderer[] rends = HatRoot.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in rends)
        {
            r.material.color = color;
        }
        Debug.Log("You have hat " + currentHat);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
