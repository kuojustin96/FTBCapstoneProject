using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ckp
{


    [CustomEditor(typeof(net_Event_MultiplayerTrigger))]
    public class Inspector_TriggerHelper : Editor
    {

        public override void OnInspectorGUI()
        {
            CreateTriggerButton();
            Validate();
            DrawDefaultInspector();

        }

        void CreateTriggerButton()
        {
            net_Event_MultiplayerTrigger ts = (net_Event_MultiplayerTrigger)target;

            if (GUILayout.Button("Create Trigger"))
            {
                if (ts.triggers.Length != 0)
                {
                    int index = 0;
                    while (index < ts.triggers.Length)
                    {
                        if (ts.triggers[index] == null)
                        {
                            GameObject obj = ts.CreateTrigger();
                            UnityEditor.Selection.activeTransform = obj.transform;
                            ts.triggers[index] = obj.GetComponent<net_Event_MultiplayerTrigger_Trigger>();
                            return;
                        }
                        index++;

                    }

                    EditorUtility.DisplayDialog("Warning", "Make the trigger array bigger please.", "Got it.");
                }
                else
                {
                    EditorUtility.DisplayDialog("Warning", "Make the trigger array bigger please.", "Got it.");
                }
            }

        }

        void Validate()
        {
            net_Event_MultiplayerTrigger ts = (net_Event_MultiplayerTrigger)target;

            if(ts.mode == ckp.net_Event_MultiplayerTrigger.net_TriggerMode.MultipleTriggers)
            {
                //if(ts.triggers.Length > 4)
                //{
                //    Debug.LogError("Impossible Trigger Detected.",ts.gameObject);
                //    GUILayout.Label("Too many triggers. Will never trigger.");
                //}
                if (ts.triggers.Length < 4)
                {
                    if (ts.minPlayers < ts.triggers.Length - 1)
                    {
                        ts.minPlayers = ts.triggers.Length - 1;
                    }
                }
            }

            if (ts.mode == ckp.net_Event_MultiplayerTrigger.net_TriggerMode.MultipleTriggers)
            {
                if (ts.minPlayers < 2)
                    ts.minPlayers = 2;
            }

        }

    }
}