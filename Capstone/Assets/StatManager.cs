using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stats
{
    SugarCollected = 0,
    SugarStolen = 1,
    TimeStunnedOthers = 2,
    TimeSpentStunned = 3,
    ItemsCraft = 4,
    NumEmotesUsed = 5,
    TimesVisitedBase = 6,
    NumTimesJumped = 7,
}

public class StatManager : MonoBehaviour {

    public static StatManager instance = null;

    public class Stat
    {
        public Stats stat;
    }

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
	}


}
