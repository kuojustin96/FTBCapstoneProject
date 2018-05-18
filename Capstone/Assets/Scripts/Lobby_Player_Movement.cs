using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;

public class Lobby_Player_Movement : NetworkBehaviour
{
    [Header("Animation")]
    public NetworkAnimator netAnimator;
    Animator anim;
    [SerializeField]
    CinemachineVirtualCameraBase mainCam;

    public bool offlineTesting = false;

    public Camera cam;
    private Rigidbody rb;
    private bool isFocused = false;
    private CursorLockMode lockMode;

    //movement
    [Header("Movement")]
    public float speed = 20.0f;
    public float gravityScale = 1.0f;
    private Vector3 moveHori;
    private Vector3 moveVert;
    private Vector3 velocity = Vector3.zero;
    private Vector3 m_MoveDir = Vector3.zero;
    //look rotation
    [Header("Player / Camera Rotation")]
    public float lookSensitivity = 3f;
    private bool inFreeLook = false;

    //jump
    [Header("Jumping")]
    public float jumpForce = 40.0f;
    private bool isGrounded = true;
    public LayerMask jumpMask;

    CharacterController character;
    bool jumped = false;
    bool canJump;

    void Start()
    {
        character = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        isFocused = true;
        Cursor.visible = false;
        anim = netAnimator.animator;

        if (isLocalPlayer)
        {
            mainCam = Net_Camera_Singleton.instance.GetCamera();
        }

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
        if (isLocalPlayer)
        {
            UpdateCursorLock();

            //Movement
            isFocused = true;
            if (isFocused)
            {

                Rotation();
                Camera();

                if(character.isGrounded && Input.GetKeyDown(KeyCode.Space))
                {
                    jumped = true;
                }

                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    inFreeLook = true;

                }
                else
                {

                    inFreeLook = false;
                }
            }

        }
    }
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            Jumping();
            Movement();
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
            transform.rotation = (transform.rotation * Quaternion.Euler(rotation));
        }
    }

    private void Movement()
    {


        moveHori = transform.right * Input.GetAxis("Horizontal");
        moveVert = transform.forward * Input.GetAxis("Vertical");

        float x = Input.GetAxis("Vertical");
        float y = Input.GetAxis("Horizontal");

        //anim.SetFloat("Vertical", x);
        //anim.SetFloat("Horizontal", y);
        //anim.SetBool("IsGrounded", isGrounded);
        //animateCharacter(x, y, isGrounded);

        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = (moveHori + moveVert) * speed;
        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, character.radius, Vector3.down, out hitInfo,
                           character.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        m_MoveDir.x = desiredMove.x * speed;
        m_MoveDir.z = desiredMove.z * speed;


        if (character.isGrounded)
        {
            //m_MoveDir.y = -m_StickToGroundForce;

            if (jumped)
            {
                jumped = false;
                m_MoveDir.y = jumpForce;

            }
        }
        else
        {
            m_MoveDir += Physics.gravity * gravityScale * Time.fixedDeltaTime;
        }
            character.Move(m_MoveDir * Time.fixedDeltaTime);
        CmdAnimateCharacter(x, y, isGrounded, false);

    }
    public void animateCharacter(float x, float y, bool jump, bool glide)
    {
        //netAnim.animator.SetInteger ("CurrentState",a);
        CmdAnimateCharacter(x, y, jump, glide);
    }


    [Command]
    public void CmdAnimateCharacter(float x, float y, bool jump, bool glide)
    {
        //netAnim.animator.SetInteger ("CurrentState",a);
        RpcAnimateCharacter(x, y, jump, glide);
    }
    [ClientRpc]
    public void RpcAnimateCharacter(float x, float y, bool jump, bool glide)
    {
        anim.SetFloat("Horizontal", x);

        anim.SetFloat("Vertical", y);

        anim.SetBool("IsGrounded", jump);

        anim.SetBool("IsGliding", glide);
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
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_XAxis.m_InputAxisValue = 0.0f;
            mainCam.gameObject.GetComponent<CinemachineFreeLook>().m_XAxis.Value = 0.0f;
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

        }

    }
}