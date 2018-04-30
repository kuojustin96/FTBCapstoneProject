using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ckp
{
    public class net_GameManager : MonoBehaviour
    {
        public static GameSettingsSO.net_Settings gs;

        public static net_GameManager instance;

        public static GameManager gm;

        [Header("Spawns")]
        public Transform yellowSpawn;
        public Transform greenSpawn;
        public Transform purpleSpawn;
        public Transform redSpawn;

        [Space(10)]
        public GameSettingsSO GameSettings;

        void Awake()
        {
            //Check if instance already exists
            if (instance == null)
            {
                //if not, set instance to this
                instance = this;

            }
            //If instance already exists and it's not this:
            else if (instance != this)
            {
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            }

            gs = instance.GameSettings.Settings;
            //Sets this to not be destroyed when reloading scene
            //DontDestroyOnLoad(gameObject);
        }
    }

}