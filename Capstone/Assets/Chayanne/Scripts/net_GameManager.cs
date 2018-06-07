using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

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
        
        public void SetupPlayerBase(GameObject player)
        {
            LobbyPlayer lPlayer = LobbyManager.s_Singleton.GetLobbyPlayer(player);

            Color theColor = lPlayer.playerColor;

            string theName = lPlayer.playerName;

            //hardcoded from lobbyplayer's colors

            BaseScript red = redSpawn.GetComponent<BaseScript>();
            BaseScript green = greenSpawn.GetComponent<BaseScript>();
            BaseScript magenta = purpleSpawn.GetComponent<BaseScript>();
            BaseScript yellow = yellowSpawn.GetComponent<BaseScript>();

            Debug.Assert(red);
            Debug.Assert(green);
            Debug.Assert(magenta);
            Debug.Assert(yellow);


            if (theColor == Color.red)
            {
                red.UpdateColor(theColor);
                red.UpdateNamePlate(theName);
            }
            if (theColor == Color.green)
            {
                green.UpdateColor(theColor);
                green.UpdateNamePlate(theName);
            }
            if (theColor == Color.magenta)
            {
                magenta.UpdateColor(theColor);
                magenta.UpdateNamePlate(theName);
            }
            if (theColor == Color.yellow)
            {
                yellow.UpdateColor(theColor);
                yellow.UpdateNamePlate(theName);
            }


        }
    }

}