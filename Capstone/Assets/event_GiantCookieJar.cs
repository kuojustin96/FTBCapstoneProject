using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;

namespace jkuo
{
    [RequireComponent(typeof(net_Event_MultiplayerTrigger))]
    public class event_GiantCookieJar : NetworkBehaviour
    {

        private net_Event_MultiplayerTrigger eventTrigger;

        // Use this for initialization
        void Start()
        {
            eventTrigger = GetComponent<net_Event_MultiplayerTrigger>();
        }

        public void ReleaseSugar()
        {
            SugarManager.instance.CmdDropSugar(eventTrigger.numSugarDrops, eventTrigger.sugarDropPos.position);
        }
    }
}