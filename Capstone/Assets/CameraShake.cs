using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public static CameraShake instance = null;
    //public ParticleSystem explosion;

    private float shakeAmount = 0f;
    public Camera cam;
    private Vector3 saveCamPos;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        saveCamPos = cam.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DoCameraShake(0.1f, 0.1f);
    }

    public void DoCameraShake(float amt = 0.025f, float length = 0.1f)
    {
        //explosion.transform.position = pos;
        //explosion.Play();
        shakeAmount = amt;
        InvokeRepeating("DoShake", 0, 0.01f);
        Invoke("StopShake", length);
    }

    private void DoShake()
    {
        Debug.Log("DSA");
        if(shakeAmount > 0)
        {
            Vector3 camPos = cam.transform.localPosition;
            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;
            camPos.x += offsetX;
            camPos.y += offsetY;

            cam.transform.localPosition = camPos;
        }
    }

    private void StopShake()
    {
        CancelInvoke("DoShake");
        cam.transform.localPosition = saveCamPos;
    }
}
