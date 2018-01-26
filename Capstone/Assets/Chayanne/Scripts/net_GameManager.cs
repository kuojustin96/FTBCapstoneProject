using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class net_GameManager : MonoBehaviour {

    public static net_GameManager gm;

    [Header("Spawns")]
    public Transform yellowSpawn;
    public Transform greenSpawn;
    public Transform purpleSpawn;
    public Transform redSpawn;

    [Space(10)]
    public GameSettingsSO GameSettings;

    void Awake()
    {
        if (gm == null)
            gm = this;
        else if (gm != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

        


}
