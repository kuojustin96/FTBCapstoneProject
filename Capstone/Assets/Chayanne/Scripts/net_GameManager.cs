using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ckp
{
    public class net_GameManager : MonoBehaviour
    {
        public static GameSettingsSO.net_Settings gs;

        public static net_GameManager netgm;

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
            if (netgm == null)
                netgm = this;
            else if (netgm != this)
                Destroy(gameObject);

            gs = netgm.GameSettings.Settings;
            //Sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);
        }
    }

}