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


    //
    public float timer = 5.0f;


    // Use this for initialization
    void Start()
    {
        PopulateHats();
        //Debug.Log("AM I LOCAL? " + isLocalPlayer);
            
        //ChangeColor(theColor);

    }

    void Update()
    {
        //ugly hack to sync player clothing. Repeatedly calls CmdChangeHat for the first second the client sees their player
        if (isLocalPlayer)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                return;
            }

            
            int hatColor = PlayerGameProfile.instance.GetPlayerOutfitSelection();
            CmdChangeHat(hatColor);

        }
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
        //index = WrapNumber(index);

        //Hats.list[currentHat].SetActive(false);
        //currentHat = index;
        //Hats.list[currentHat].SetActive(true);


        RpcChangeHat(index);
    }

    [ClientRpc]
    void RpcChangeHat(int index)
    {

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

    public void ChangeHat(int index)
    {
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
