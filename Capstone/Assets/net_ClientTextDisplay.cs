using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ckp
{
    public class net_ClientTextDisplay : MonoBehaviour
    {

        TextMeshProUGUI tm;

        public net_PlayerTrigger t;


        // Use this for initialization
        void Start()
        {
            tm = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {

            int val = t.numPlayersInTrigger;

            tm.text = val.ToString();
        }

    }
}