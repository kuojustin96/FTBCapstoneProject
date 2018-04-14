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

    public Dictionary<Stats, int> statTracker { get; protected set; }

    public TextAsset priorityText;
    public TextAsset nonpriorityText;

    public float minTickerTime = 10f;
    public float maxTickerTime = 30f;
    private Coroutine tickerTimeCoroutine;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        statTracker = new Dictionary<Stats, int>();
	}

    void Start()
    {
        ResetTickerTimer();
    }

    public void UpdateStat(Stats stat)
    {
        statTracker[stat] += 1;
    }

    public void ResetTickerTimer()
    {
        float timeUntilTicker = Random.Range(minTickerTime, maxTickerTime);

        if(tickerTimeCoroutine != null)
            StopCoroutine(tickerTimeCoroutine);

        tickerTimeCoroutine = StartCoroutine(TickerTimer(timeUntilTicker));
    }

    private IEnumerator TickerTimer(float timeUntilTicker)
    {
        float saveTime = Time.time;
        while (Time.time < saveTime + timeUntilTicker)
            yield return null;

        //Enable a ticker message
    }
}