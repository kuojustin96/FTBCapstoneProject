using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour {

    public RectTransform[] indicators;
    public RectTransform baseIndicator;
    public float yOffset = 20f;
    public float minShowDistance = 150f;

    private Transform dropoffPoint;
    private PlayerClass player;
    private List<Transform> playerTransforms = new List<Transform>();

    [Range(0, 100)]
    public float m_edgeBuffer = 10;

    private void Start()
    {
        
    }

    public void UpdatePlayerTransforms(List<PlayerClass> playerList)
    {
        player = GetComponent<playerClassAdd>().player;
        dropoffPoint = player.dropoffPoint.transform;
        playerTransforms.Clear();
        foreach (PlayerClass p in playerList)
        {
            if (p != player)
                playerTransforms.Add(p.playerGO.transform);
        }

        foreach (RectTransform rt in indicators)
            rt.gameObject.SetActive(false);

        for(int x = 0; x < playerTransforms.Count; x++)
        {
            indicators[x].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update () {
        UpdateTargetIconPosition();
	}

    private void UpdateTargetIconPosition()
    {
        //Player Indicators
        for(int x = 0; x < playerTransforms.Count; x++)
        {
            indicators[x].gameObject.SetActive(true);
            float dist = Vector3.Distance(transform.position, playerTransforms[x].position);
            if (dist > minShowDistance)
            {
                Vector3 newPos = playerTransforms[x].position;
                newPos = Camera.main.WorldToViewportPoint(newPos);
                if (newPos.z < 0)
                {
                    newPos.x = 1f - newPos.x;
                    newPos.y = 1f - newPos.y;
                    newPos.z = 0;
                    newPos = Vector3Maxamize(newPos);
                }
                newPos = Camera.main.ViewportToScreenPoint(newPos);
                newPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
                newPos.y += yOffset;
                newPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);
                newPos.z = 0;
                indicators[x].position = newPos;

                if (newPos.x + m_edgeBuffer >= Screen.width - m_edgeBuffer || newPos.x - m_edgeBuffer <= 0 + m_edgeBuffer
                    || newPos.y + m_edgeBuffer >= Screen.height - m_edgeBuffer || newPos.y - m_edgeBuffer <= 0 + m_edgeBuffer)
                    indicators[x].gameObject.SetActive(false);
                else
                    indicators[x].gameObject.SetActive(true);
            } else
            {
                indicators[x].gameObject.SetActive(false);
            }
        }

        //Base Indicator
        if (player != null)
        {
            Vector3 newPos2 = player.dropoffPoint.transform.position;
            newPos2 = Camera.main.WorldToViewportPoint(newPos2);
            if (newPos2.z < 0)
            {
                newPos2.x = 1f - newPos2.x;
                newPos2.y = 1f - newPos2.y;
                newPos2.z = 0;
                newPos2 = Vector3Maxamize(newPos2);
            }
            newPos2 = Camera.main.ViewportToScreenPoint(newPos2);
            newPos2.x = Mathf.Clamp(newPos2.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
            newPos2.y += yOffset;
            newPos2.y = Mathf.Clamp(newPos2.y, m_edgeBuffer, Screen.height - m_edgeBuffer);
            newPos2.z = 0f;
            baseIndicator.position = newPos2;

            if (newPos2.x + m_edgeBuffer >= Screen.width - m_edgeBuffer || newPos2.x - m_edgeBuffer <= 0 + m_edgeBuffer
                || newPos2.y + m_edgeBuffer >= Screen.height - m_edgeBuffer || newPos2.y - m_edgeBuffer <= 0 + m_edgeBuffer)
                baseIndicator.gameObject.SetActive(false);
            else
                baseIndicator.gameObject.SetActive(true);
        }
    }

    private Vector3 Vector3Maxamize(Vector3 vector)
    {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }
}
