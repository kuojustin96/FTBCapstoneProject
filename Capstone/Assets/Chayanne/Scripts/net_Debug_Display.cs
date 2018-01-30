using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ckp {
    public class net_Debug_Display : MonoBehaviour
    {

        TextMeshProUGUI tm;

        public net_Event_MultiplayerTrigger_Trigger t;


        // Use this for initialization
        void Start()
        {
            tm = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {

            //int val = t.numPlayersInTrigger;

            //tm.text = val.ToString();
        }


    }
}