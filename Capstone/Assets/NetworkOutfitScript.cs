using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


[System.Serializable]
public struct HatList
{
    public List<GameObject> list;
}

public class NetworkOutfitScript : NetworkBehaviour {

    public Transform HatRoot;

    public HatList Hats;

    [SyncVar]
    public int currentHat;


	// Use this for initialization
	void Start ()
    {
        if (isLocalPlayer)
        {
            CmdChangeHat(currentHat);
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
        RpcChangeHat(index);
    }

    [ClientRpc]
    void RpcChangeHat(int index)
    {


        Hats.list[currentHat].SetActive(false);
        currentHat = index % Hats.list.Count;
        Hats.list[currentHat].SetActive(true);


    }
    // Update is called once per frame
    void Update () {
		
	}
}
