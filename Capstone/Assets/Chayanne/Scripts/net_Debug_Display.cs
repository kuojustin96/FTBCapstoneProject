using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

<<<<<<< HEAD:Capstone/Assets/net_ClientTextDisplay.cs
namespace ckp
{
    public class net_ClientTextDisplay : MonoBehaviour
=======
namespace ckp {
    public class net_Debug_Display : MonoBehaviour
>>>>>>> b41ff2756db238cb5b287a0c7b87c6421476f696:Capstone/Assets/Chayanne/Scripts/net_Debug_Display.cs
    {

        TextMeshProUGUI tm;

        public net_Event_MultiplayerTrigger_Trigger t;


<<<<<<< HEAD:Capstone/Assets/net_ClientTextDisplay.cs

=======
>>>>>>> b41ff2756db238cb5b287a0c7b87c6421476f696:Capstone/Assets/Chayanne/Scripts/net_Debug_Display.cs
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

<<<<<<< HEAD:Capstone/Assets/net_ClientTextDisplay.cs
=======

>>>>>>> b41ff2756db238cb5b287a0c7b87c6421476f696:Capstone/Assets/Chayanne/Scripts/net_Debug_Display.cs
    }
}