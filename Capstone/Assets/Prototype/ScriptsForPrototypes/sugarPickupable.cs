using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sugarPickupable : MonoBehaviour {

    public LayerMask mask;
    private Transform particles;

	// Use this for initialization
	void Start () {
        particles = transform.GetChild(0);
        CheckForGround();
	}

    public void CheckForGround()
    {
        StartCoroutine(c_CheckForGround());
    }

    private IEnumerator c_CheckForGround()
    {
        bool checking = true;
        RaycastHit hit;
        while (checking)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f, mask))
            {
                checking = false;
                particles.position = hit.point;
            }
            else
            {
                transform.Translate(Vector3.down * 0.5f);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
