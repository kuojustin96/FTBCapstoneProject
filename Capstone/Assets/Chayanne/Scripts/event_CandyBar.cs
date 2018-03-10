using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ckp;

[RequireComponent(typeof(net_Event_MultiplayerTrigger))]
public class event_CandyBar : MonoBehaviour {

    public GameObject candyBar;
    private net_Event_MultiplayerTrigger eventTrigger;

	// Use this for initialization
	void Start () {
        eventTrigger = GetComponent<net_Event_MultiplayerTrigger>();
	}
	
	public void TriggerCandyBarEvent()
    {
        candyBar.SetActive(true);
        SugarManager.instance.CmdDropSugar(eventTrigger.numSugarDrops, eventTrigger.sugarDropPos.position);
    }
}
