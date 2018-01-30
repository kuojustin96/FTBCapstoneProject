using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

    public int eventsToSpawn = 4;
    public float minWaitDuration = 10f;
    public float maxWaitDuration = 20f;
    private int counter = 0;

    public List<GameObject> events = new List<GameObject>();
    private List<GameObject> activeEvents = new List<GameObject>();

    public List<GameObject> eventSpawnSpots = new List<GameObject>();
    private List<GameObject> activeSpawnSpots = new List<GameObject>();

	// Use this for initialization
	void Start () {
        foreach(GameObject g in events)
        {
            g.SetActive(false);
        }

        //Fail safe
        int testCase;
        if (events.Count < eventSpawnSpots.Count)
            testCase = events.Count;
        else
            testCase = eventSpawnSpots.Count;

        if (eventsToSpawn > testCase)
            eventsToSpawn = testCase;


        for(int x = 0; x < eventsToSpawn; x++)
        {
            int randSpot = Random.Range(0, eventSpawnSpots.Count);
            int randEvent = Random.Range(0, events.Count);

            events[randEvent].transform.position = eventSpawnSpots[randSpot].transform.position;
            events[randEvent].SetActive(true);
            activeEvents.Add(events[randEvent]);
            events.Remove(events[randEvent]);
            activeSpawnSpots.Add(eventSpawnSpots[randSpot]);
            eventSpawnSpots.Remove(eventSpawnSpots[randSpot]);
        }

        StartCoroutine(activateEvent());
    }

    private IEnumerator activateEvent()
    {
        float waitTime = Random.Range(minWaitDuration, maxWaitDuration);
        yield return new WaitForSeconds(waitTime);

        //Activate an event in the activeEvents list
        //activeEvents[counter].activateEvent       or something
        Debug.Log("Activated an event!");
        counter++;

        if (counter < eventsToSpawn)
            StartCoroutine(activateEvent());
        else
            Debug.Log("Finished");
    }
}
