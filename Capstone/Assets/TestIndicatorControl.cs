using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestIndicatorControl : MonoBehaviour {

    public RectTransform indicator;
    public float yOffset = 20f;
    public Transform target;

    [Range(0, 100)]
    public float m_edgeBuffer = 10;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        //indicator.anchoredPosition = screenPos;
        UpdateTargetIconPosition();
	}

    private void UpdateTargetIconPosition()
    {
        Vector3 newPos = transform.position;
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
        indicator.position = newPos;

        if (newPos.x + m_edgeBuffer >= Screen.width - m_edgeBuffer || newPos.x - m_edgeBuffer <= 0 + m_edgeBuffer
            || newPos.y + m_edgeBuffer >= Screen.height - m_edgeBuffer || newPos.y - m_edgeBuffer <= 0 + m_edgeBuffer)
            indicator.gameObject.SetActive(false);
        else
            indicator.gameObject.SetActive(true);
    }

    public Vector3 Vector3Maxamize(Vector3 vector)
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
