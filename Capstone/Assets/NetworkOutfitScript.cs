using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

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

    public GameObject clothing;


    // Use this for initialization
    void Start()
    {
        PopulateHats();
        Debug.Log("AM I LOCAL? " + isLocalPlayer);
        ChangeHat(currentHat);
        //ChangeColor(theColor);

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
        foreach (GameObject hat in Hats.list)
        {
            hat.SetActive(false);
        }

        Debug.Log("RPC!");
        index = WrapNumber(index);

        Hats.list[currentHat].SetActive(false);
        currentHat = index;
        Hats.list[currentHat].SetActive(true);

        Debug.Log("You have hat " + currentHat);
    }

    public void ChangeHat(int index)
    {
        Debug.Log("We have " + Hats.list.Count);
        if (Hats.list.Count == 0)
        {
            return;
        }

        foreach (GameObject hat in Hats.list)
        {
            hat.SetActive(false);
        }

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



}
