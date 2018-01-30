using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace ckp
{
    public class Event_2PlayerDoorOpen : NetworkBehaviour
    {

        //Amount how much sugar to drop
        public int numSugarDrops = 10;

        [Range(1, 4)]
        public int minPlayers = 2;

        //	public Event_ButtonPlayerDetection button1;
        //	public Event_ButtonPlayerDetection button2;
        public bool finishedEvent = false;

        public net_PlayerTrigger[] triggers;


        public UnityEvent completionMethods;


        // Use this for initialization
        void Start()
        {

        }


        public void UpdateButton()
        {
            if (isServer)
            {

                if (finishedEvent)
                    return;

                int numTriggered = 0;
                for(int i = 0; i < triggers.Length; i++)
                {
                    if(triggers[i].IsTriggered())
                    {
                        numTriggered++;
                    }
                }

                if (numTriggered >= minPlayers)
                {

                    finishedEvent = true;
                    completionMethods.Invoke();
                }
            }

        }
    }

}