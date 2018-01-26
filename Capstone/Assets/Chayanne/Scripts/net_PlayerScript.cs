#define DEBUG_MODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace ckProtoyType
{
    public class net_PlayerScript : NetworkBehaviour
    {

        
        [Header("Gameobjects")]
        public GameObject projectile;
        public Transform gunBarrelPos;
        public GameObject gunObj;


        [SyncVar]
        public string playerName;

        //Local Player profile stuff
        public float mouseSensitivity = 500.0f;

        [Space(10)]
        public float moveSpeedMult = 1.0f;
        public float jumpHeightMult = 1.0f;
        float bodySlamStunDurMult = 1.0f;

        float clampAngle = 80.0f;
        private float rotY = 0.0f; // rotation around the up/y axis
        private float rotX = 0.0f; // rotation around the right/x axis

        [Space(10)]
        [SerializeField]
        int sugarCount;
        CursorLockMode lockMode;

        bool isPaused = false;
        bool doVanityCam = false;
        Rigidbody rb;


        //Derived from singleton;
        float moveSpeed;
        float jumpHeight;


        void InitializeGameSettings()
        {

            GameSettingsSO gs = net_GameManager.gm.GameSettings;

            moveSpeed = gs.Settings.baseMoveSpeed;
            jumpHeight = gs.Settings.baseJumpHeight;


        }


        void Start()
        {

            rb = GetComponent<Rigidbody>();

            LocalCameraCheck();
            SetupName();

            //Initialize cursor lock
            lockMode = CursorLockMode.Locked;

            ////Init position.
            //Vector3 rot = transform.localRotation.eulerAngles;
            //rotY = rot.y;
            //rotX = rot.x;
        }

        private void SetupName()
        {
            GetComponentInChildren<TextMeshPro>().text = playerName;
        }

        public void SetName(string theName)
        {
            playerName = theName;
        }

        void LocalCameraCheck()
        {

            Camera cam = GetComponentInChildren<Camera>();

            if (!GetComponent<net_PlayerScript>().isLocalPlayer)
            {
                cam.GetComponent<Camera>().enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
            }

        }

        void UpdateCursorLock()
        {
#if (DEBUG_MODE)
            if (Input.GetMouseButton(0))
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
            if (isLocalPlayer)
            {

                VanityCameraUpdate();

                UpdateCursorLock();
                ActionUpdate();
                MouseRotation();
                MovementUpdate();
            }


        }

        void VanityCameraUpdate()
        {

            if (Input.GetMouseButton(2))
            {
                doVanityCam = true;
            }
            else
            {
                doVanityCam = false;
            }

        }

        void MovementUpdate()
        {



            if (Input.GetKey(KeyCode.W))
            {
                rb.AddForce(transform.forward * moveSpeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.right * -moveSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(transform.forward * -moveSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.right * moveSpeed);
            }

        }

        void ActionUpdate()
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(0, 2000, 0);
            }
            if (Input.GetButtonDown("Fire2"))
            {
                CmdSpawnBullet();
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

            if (!doVanityCam)
            {
                Quaternion localRotationPlayer = Quaternion.Euler(0.0f, rotY, 0.0f);
                transform.rotation = localRotationPlayer;
            }

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
        void CmdSpawnBullet()
        {

            //Need to do some magic bullshit to make this smooth. ie: Make the bullet simulate physics clientside from where the shot is fired.
            GameObject clone;
            clone = Instantiate(projectile, gunBarrelPos.position, gunBarrelPos.transform.rotation);
            NetworkServer.Spawn(clone);
            clone.GetComponent<Rigidbody>().AddRelativeForce(0, 0, 50000);
        }
    }
}