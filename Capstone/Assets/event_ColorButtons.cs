using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ckp;

namespace jkuo
{
    [RequireComponent(typeof(net_Event_MultiplayerTrigger))]
    public class event_ColorButtons : NetworkBehaviour
    {

        public int numRounds = 3;

        private int roundCounter = 0;
        private net_Event_MultiplayerTrigger_Trigger[] buttons;
        public SyncListInt activeButtons = new SyncListInt();
        private net_Event_MultiplayerTrigger eventTrigger;

        private Color saveColor;

        // Use this for initialization
        void Start()
        {
            eventTrigger = GetComponent<net_Event_MultiplayerTrigger>();
            buttons = eventTrigger.triggers;
            saveColor = buttons[0].GetComponent<MeshRenderer>().material.color;

            ChooseRandomButtons();
        }

        //Causes crashes for some reason, only in editor
        public override void OnStartClient()
        {
            base.OnStartClient();
            activeButtons.Callback += OnListChanged;
        }

        //Chooses X buttons at random to color green
        //Green = triggers players must hit to activate
        private void ChooseRandomButtons()
        {
            if (isServer)
            {

                for (int x = 0; x < eventTrigger.minPlayers; x++)
                {
                    bool choosingButton = true;
                    while (choosingButton)
                    {
                        int y = Random.Range(0, buttons.Length);
                        if (!activeButtons.Contains(y))
                        {
                            activeButtons.Add(y);
                            activeButtons.Dirty(activeButtons.Count - 1);
                            choosingButton = false;
                        }
                    }
                }
            }

            //foreach (int i in activeButtons)
            //{
            //    buttons[i].GetComponent<MeshRenderer>().material.color = Color.green;
            //}
        }

        //Debug function
        public void AddToList()
        {
            Debug.Log("TESTING");
            buttons[0].GetComponent<MeshRenderer>().material.color = Color.cyan;
            activeButtons.Add(0);
        }

        public void CheckForCompletion()
        {
            Debug.Log("checking...");
            if (isServer)
            {
                Debug.Log("There are " + activeButtons.Count + " active buttons");
                for (int x = 0; x < activeButtons.Count; x++)
                {
                    Debug.Log(buttons[activeButtons[x]]);
                    if (!buttons[activeButtons[x]].IsTriggered())
                    {
                        Debug.Log("Not all buttons triggered");
                        return;
                    }
                }

                //All correct buttons are pressed, remove them.
                int listSize = activeButtons.Count;
                for (int x = 0; x < listSize; x++)
                {
                    //int y = activeButtons[0];
                    //activeButtons.Dirty(0);
                    activeButtons.RemoveAt(0);
                    //buttons[y].GetComponent<MeshRenderer>().material.color = Color.grey;
                }
            }


            if (roundCounter < numRounds)
            {
                Debug.Log("Next Round");
                roundCounter++;
                ChooseRandomButtons();
            }
            else
            {
                //Drop sugar
                Debug.Log("Color Button Event Complete!");
            }
        }

        private void OnListChanged(SyncListInt.Operation op, int index)
        {
            Debug.Log("List Changed " + op);
            Debug.Log("number is" +  activeButtons[index]);

            int buttonToDeactivate = activeButtons[index];

            if (isServer)
            {
                if (op == SyncList<int>.Operation.OP_ADD)
                {
                    //CmdPaint(true, index);
                    RpcChangeColor(true, buttonToDeactivate);
                }
                else if (op == SyncList<int>.Operation.OP_REMOVEAT)
                {
                    //CmdPaint(false, index);
                    RpcChangeColor(false, buttonToDeactivate);
                }
            }
            //activeButtons.Dirty(index);
        }

        [ClientRpc]
        private void RpcChangeColor(bool hasColor, int index)
        {
            if (hasColor)
            {
                buttons[index].GetComponent<MeshRenderer>().material.color = Color.green;
                //activeButtons.Dirty(index);
            }
            else
            {
                buttons[index].GetComponent<MeshRenderer>().material.color = saveColor;
                //activeButtons.Dirty(index);
            }
        }

        [Command]
        private void CmdPaint(bool hasColor, int index)
        {
            Debug.Log(connectionToClient);
            NetworkIdentity objNetId = buttons[index].GetComponent<NetworkIdentity>();        // get the object's network ID
            //objNetId.AssignClientAuthority(connectionToClient);    // assign authority to the player who is changing the color
            //RpcPaint(obj, col);                                    // usse a Client RPC function to "paint" the object on all clients
            RpcChangeColor(hasColor, index);
            //objNetId.RemoveClientAuthority(connectionToClient);    // remove the authority from the player who changed the color
        }

    }
}