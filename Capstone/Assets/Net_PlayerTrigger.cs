using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace ckp
{
    

    public class net_PlayerTrigger : NetworkBehaviour
    {

        public bool colorSpecific;
        net_TeamScript.Team[] teamColors;

        public int numPlayersInTrigger = 0;

        bool triggered = false;

        Event_2PlayerDoorOpen eventScript;

        void Start()
        {
            eventScript = GetComponentInParent<Event_2PlayerDoorOpen>();
        }

        public bool IsTriggered()
        {
            return triggered;
        }

        void OnTriggerEnter(Collider other)
        {
            if(isServer)    
            {
                net_TeamScript team = other.gameObject.GetComponent<net_TeamScript>();
                if (team && IsCorrectPlayer(team))
                {
                    numPlayersInTrigger++;
                    triggered = true;
                    eventScript.UpdateButton();
                }

                    return;
            }

        }

        bool IsCorrectPlayer(net_TeamScript team)
        {
            for (int i = 0; i < teamColors.Length; i++)
            {
                if (teamColors[i] == team.teamColor)
                {
                    return true;
                }

            }
            return false;
        }

        void OnTriggerExit(Collider other)
        {
            if (!isServer)
            {
                net_TeamScript team = other.gameObject.GetComponent<net_TeamScript>();
                if (team && IsCorrectPlayer(team))
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