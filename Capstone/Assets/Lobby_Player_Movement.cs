using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;

public class Lobby_Player_Movement : NetworkBehaviour
{


    public CinemachineVirtualCameraBase mainCam;
    public CinemachineVirtualCameraBase freeCam;


    public bool offlineTesting = false;

    public Camera cam;
    private Rigidbody rb;
    private bool isFocused = false;
    private CursorLockMode lockMode;

    //movement
    [Header("Movement")]
    public float speed = 50f;
    private Vector3 moveHori;
    private Vector3 moveVert;
    private Vector3 velocity = Vector3.zero;

    //look rotation
    [Header("Player / Camera Rotation")]
    public float lookSensitivity = 3f;
    private bool inFreeLook = false;

    //jump
    [Header("Jumping")]
    public float jumpForce = 1000f;
    public float gravity = 100f;
    public float downwardAcceleration = 1f;
    public LayerMask jumpMask;
    private bool isGrounded;
    private bool canJump = true;
    private Vector3 _jumpForce = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        isFocused = true;
        Cursor.visible = false;

    }

    private void LocalCameraCheck()
    {

    }

    private void UpdateCursorLock()
    {
        if (Input.GetMouseButton(0))
        {
            isFocused = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isFocused = false;
        }

        if (isFocused)
        {
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "Mouse Y";
            lockMode = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_YAxis.m_InputAxisName = "";
            Cursor.visible = true;
            lockMode = CursorLockMode.None;
        }


        Cursor.lockState = lockMode;
    }

    void Update()
    {
        if (isLocalPlayer)// || offlineTesting)
        {
            UpdateCursorLock();

            //Movement

            if (isFocused)
            {
                Movement();

                Rotation();
                RefreshList();

                Camera();



                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    inFreeLook = true;

                }
                else
                {

                    inFreeLook = false;
                }
            }
            //Jump
            Jumping();


        }
    }

    private void Rotation()
    {
        float xRot;
        float yRot;
        xRot = Input.GetAxisRaw("Mouse Y");
        yRot = Input.GetAxisRaw("Mouse X");

        if (!inFreeLook)
        {
            Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        }
    }

    private void Movement()
    {
        moveHori = transform.right * Input.GetAxis("Horizontal");
        moveVert = transform.forward * Input.GetAxis("Vertical");


        velocity = (moveHori + moveVert) * speed;

        if (velocity != Vector3.zero)
        {
            rb.velocity = velocity;
        }
    }


    void Camera()
    {

        if (inFreeLook)
        {
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_BindingMode = CinemachineTransposer.BindingMode.LockToTarget;
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisName = "";
        }
    }

    private void Jumping()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 4.5f, jumpMask))
        {

            isGrounded = true;
            canJump = true;
        }
        else
        {

            isGrounded = false;
            //apply gravity

            rb.AddForce(new Vector3(0f, -gravity, 0f), ForceMode.Force);
            rb.AddForce(Vector3.down * downwardAcceleration, ForceMode.Impulse);

        }

        if (isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                _jumpForce = transform.up * (jumpForce * 10000);
                rb.AddForce(_jumpForce * Time.fixedDeltaTime);
            }
        }
    }

    public void RefreshList()
    {



        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!Network.isServer)
            {
                Debug.Log("sorry you're a client");
                return;
            }
            Debug.Log(isLocalPlayer);
            Prototype.NetworkLobby.LobbyPlayerList._instance.theList.CmdRegenerateList();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (!Network.isServer)
            {
                Debug.Log("sorry you're a client");
                return;
            }
            Debug.Log(isLocalPlayer);
            Prototype.NetworkLobby.LobbyPlayerList._instance.theList.RpcRegenerateList();
        }
    }
}