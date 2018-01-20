#define DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;


public class NetMoveTest : NetworkBehaviour {

    public bool isSinglePlayer;

    public GameObject projectile;
	public Transform gunBarrelPos;
	public GameObject player;
    public GameObject gunObj;

    [SyncVar]
    public string playerName;

    public float moveSpeed = 100.0f;
    public float mouseSensitivity = 500.0f;

    float clampAngle = 80.0f;
	private float rotY = 0.0f; // rotation around the up/y axis
	private float rotX = 0.0f; // rotation around the right/x axis


    CursorLockMode lockMode;

    bool isPaused = false;

	// Use this for initialization
	void Start () {

        GetComponentInChildren<TextMeshPro>().text = playerName;

        //Init position.
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;

        //Initialize cursor lock
        lockMode = CursorLockMode.Locked;
	}

    public void SetName(string theName)
    {
        playerName = theName;
    }

    void UpdateCursorLock()
    {
        #if (DEBUG_MODE)
            if(Input.GetMouseButton(0))
            {
                isPaused = false;
                Cursor.visible = false;
                lockMode = CursorLockMode.Locked;
            }
#endif
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPaused = !isPaused;

                if (isPaused)
                {
                    lockMode = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                    lockMode = CursorLockMode.Locked;
                }
            }


        Cursor.lockState = lockMode;
    }

	// Update is called once per frame


    void Update()

    {
        if (isLocalPlayer || isSinglePlayer)
        {
            UpdateCursorLock();
            ActionUpdate();
            MouseRotation();
            MovementUpdate();
        }


    }

    void MovementUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, player.transform);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.right * -moveSpeed * Time.fixedDeltaTime, player.transform);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * -moveSpeed * Time.fixedDeltaTime, player.transform);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.fixedDeltaTime, player.transform);
        }
    }

    void ActionUpdate()
    {
        if (Input.GetButtonDown("Jump"))
        {
            gameObject.GetComponent<Rigidbody>().AddForce(0, 1000, 0);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!isSinglePlayer)
            {
                CmdSpawnBullet();
            }
            else
            {
                ShootLocal();
            }
        }
    }

    void MouseRotation()
    {
        if (isPaused)
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotationPlayer = Quaternion.Euler(0.0f, rotY, 0.0f);
        player.transform.rotation = localRotationPlayer;

        Quaternion localRotationGun = Quaternion.Euler(rotX, rotY, 0.0f);
        gunObj.transform.rotation = localRotationGun;

    }


    void ShootLocal()
    {

        GameObject clone;
        clone = Instantiate(projectile, gunBarrelPos.position, gunBarrelPos.transform.rotation);
        clone.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 50000);

    }


    [Command] 
	void CmdSpawnBullet(){

        //Need to do some magic bullshit to make this smooth. ie: Make the bullet simulate physics clientside from where the shot is fired.
		GameObject clone;
		clone = Instantiate (projectile, gunBarrelPos.position, gunBarrelPos.transform.rotation) ;
		NetworkServer.Spawn (clone);
		clone.GetComponent<Rigidbody>().AddRelativeForce (0, 0, 50000);
	}
}
