using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace jkuo
{
    [RequireComponent(typeof(Rigidbody))]
    public class net_PlayerController : NetworkBehaviour
    {

        public Camera cam;

        private Rigidbody rb;
        private bool isPaused = false;
        private CursorLockMode lockMode;

        //movement
        public float speed = 20f;
        private Vector3 moveHori;
        private Vector3 moveVert;
        private Vector3 velocity = Vector3.zero;

        //look rotation
        public float lookSensitivity = 3f;
        private float xRot;
        private float yRot;
        private Vector3 rotation = Vector3.zero;

        //jump
        public float jumpForce = 1000f;
        public float gravity = 100f;
        public LayerMask jumpMask;
        private bool isGrounded;
        private Vector3 _jumpForce = Vector3.zero;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            LocalCameraCheck();
            lockMode = CursorLockMode.Locked;

        }

        private void LocalCameraCheck()
        {
            if (!GetComponent<net_PlayerController>().isLocalPlayer)
            {
                cam.GetComponent<Camera>().enabled = false;
                cam.GetComponent<AudioListener>().enabled = false;
            }
        }

        private void UpdateCursorLock()
        {
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

        void Update()
        {
            if (isLocalPlayer)
            {
                UpdateCursorLock();

                //Movement
                moveHori = transform.right * Input.GetAxis("Horizontal");
                moveVert = transform.forward * Input.GetAxis("Vertical");
                velocity = (moveHori + moveVert) * speed;

                //Camera Rotation
                yRot = Input.GetAxisRaw("Mouse X");
                rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
                //cam.transform.RotateAround(transform.position, Vector3.up, lookSensitivity * Time.fixedDeltaTime);

                //float mouseX = Input.GetAxis("Mouse X");
                //float mouseY = -Input.GetAxis("Mouse Y");

                //xRot += mouseX * lookSensitivity * Time.deltaTime;
                //yRot += mouseY * lookSensitivity * Time.deltaTime;

                //Quaternion.Euler(xRot, yRot, 0.0f);

                //Jump
                RaycastHit hit;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 4.5f, jumpMask))
                {
                    isGrounded = true;
                }
                else
                {
                    isGrounded = false;
                    //apply gravity
                    rb.AddForce(new Vector3(0f, -gravity, 0f), ForceMode.Force);
                }

                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                {
                    _jumpForce = transform.up * (jumpForce * 1000);
                    rb.AddForce(_jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
                }
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                //Movement
                if (velocity != Vector3.zero)
                    rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

                //Rotation
                rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
            }
        }
    }
}