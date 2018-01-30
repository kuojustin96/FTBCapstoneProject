using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ckp
{
    

    public class net_Event_MultiplayerTrigger_Trigger : NetworkBehaviour
    {
        public int numPlayersInTrigger = 0;

        bool triggered = false;

        net_Event_MultiplayerTrigger eventScript;

        void Start()
        {
            eventScript = GetComponentInParent<net_Event_MultiplayerTrigger>();
        }

        public bool IsTriggered()
        {
            return triggered;
        }

        void OnTriggerEnter(Collider other)
        {
            if(isServer)    
            {
                Debug.Log("Meme");
                net_PlayerScript playerComp = other.gameObject.GetComponent<net_PlayerScript>();
                if (playerComp)
                {
                    numPlayersInTrigger++;
                    triggered = true;
                    eventScript.UpdateButton();
                }
                    return;
            }

        }


        void OnTriggerExit(Collider other)
        {
            if (isServer)
            {
                net_PlayerScript playerComp = other.gameObject.GetComponent<net_PlayerScript>();
                if (playerComp)
                {
                    numPlayersInTrigger--;

                    //Dont turn off trigger unless this is the last applicable player off.
                    if (numPlayersInTrigger < 1)
                    {
                        triggered = false;
                        eventScript.UpdateButton();
                    }
                }

                return;
            }
        }

    }
}