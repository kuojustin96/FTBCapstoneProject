using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuOutfitManager : MonoBehaviour {

    public Transform HatRoot;

    public HatList Hats;

    public int currentHat = 0;


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject chadRoot = GameObject.FindGameObjectWithTag("ChadCustomize");

        if (chadRoot)
        {
            HatRoot = GameObject.FindGameObjectWithTag("ChadCustomize").transform;
            PopulateHats(); 
        }
    }

    void Start()
    {
        //will change later

        SceneManager.sceneLoaded += OnSceneLoaded;

        PopulateHats();
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


    }

    public void ChangeSelection(int change)
    {

        ChangeHat(currentHat + change);

    }

    public void ChangeHat(int index)
    {

        if(index < 0)
        {
            index = Hats.list.Count - 1;
        }
        if(index >= Hats.list.Count)
        {
            index = 0;
        }

        Hats.list[currentHat].SetActive(false);
        currentHat = index;
        Hats.list[currentHat].SetActive(true);

        PlayerGameProfile.instance.UpdatePlayerOutfitSelection(index);

    }
}
