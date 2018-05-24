using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prototype.NetworkLobby;
public class MenuOutfitManager : MonoBehaviour
{

    public Transform HatRoot;

    public HatList Hats;

    public int currentHat = 0;


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!LobbyManager.IsLobbyScene())
        {
            Debug.Log("NOT LOBBY SCENE");
            return;
        }

        TryToPopulate();
    }

    private void TryToPopulate()
    {
        HatRoot = GameObject.FindGameObjectWithTag("ChadCustomize").transform;
        if (HatRoot)
        {
            PopulateHats();
        }
    }

    void Start()
    {
        //will change later

        SceneManager.sceneLoaded += OnSceneLoaded;
        if (!LobbyManager.IsLobbyScene())
        {

            Debug.Log("NOT LOBBY SCENE");
            return;
        }


        TryToPopulate();

        if (!HatRoot)
        {
            Debug.Log("returning");
            return;
        }
        else
        {
            Debug.Log("Hats is " + Hats.list.Count);
            foreach (GameObject hat in Hats.list)
            {
                Debug.Log("disabling");
                hat.SetActive(false);
            }

        }
        int selection = PlayerGameProfile.instance.GetPlayerOutfitSelection();
        Debug.Log("Outfit selection: " + selection);
        ChangeSelection(selection);
    }

    public void PopulateHats()
    {

        Hats.list.Clear();
        foreach (Transform child in HatRoot)
        {
            Hats.list.Add(child.gameObject);
        }

        foreach (GameObject hat in Hats.list)
        {
            hat.SetActive(false);
        }
    }

    public void ChangeSelection(int change)
    {

        ChangeHat(currentHat + change);

    }

    public void ChangeHat(int index)
    {

        if (index < 0)
        {
            index = Hats.list.Count - 1;
        }
        if (index >= Hats.list.Count)
        {
            index = 0;
        }

        Hats.list[currentHat].SetActive(false);
        currentHat = index;
        Hats.list[currentHat].SetActive(true);

        PlayerGameProfile.instance.UpdatePlayerOutfitSelection(index);

    }
}
