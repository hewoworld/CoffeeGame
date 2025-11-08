using System;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject), typeof(NetworkTransform))]
public class NetworkedPlayerAdvanced : NetworkBehaviour
{
    [Header("References")]
    public Transform playerCam;
    public Transform orientation;

    [Header("Camera & Look")]
    public float sensitivity = 70f;
    public float sensMultiplier = 1f;
    private float xRotation;
    private NetworkVariable<float> serverYaw = new(writePerm: NetworkVariableWritePermission.Server);

    [Header("Movement Settings")]
    public float moveSpeed = 4500f;
    public float maxSpeed = 20f;
    public float counterMovement = 0.175f;
    public float slideForce = 400f;
    public float slideCounterMovement = 0.2f;
    public float jumpForce = 550f;
    public LayerMask whatIsGround;

    [Header("Crouch / Slide")]
    public Vector3 crouchScale = new(1, 0.5f, 1);
    private Vector3 playerScale;
    private bool crouching;
    private bool grounded;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;

    private Rigidbody rb;
    private float x, y;
    private bool jumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerScale = transform.localScale;

        if (!IsOwner)
        {
            if (playerCam) playerCam.gameObject.SetActive(false);
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!IsOwner) return;

        HandleInput();
        HandleLook();

        if (jumping && grounded && readyToJump)
            JumpServerRpc();

        if (Input.GetKeyDown(KeyCode.LeftControl))
            CrouchServerRpc(true);
        if (Input.GetKeyUp(KeyCode.LeftControl))
            CrouchServerRpc(false);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        MoveServerRpc(x, y);
    }

    private void HandleInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jumping = Input.GetButton("Jump");
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime * sensMultiplier;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Calculate yaw and apply to both orientation (for movement) and camera (for view)
        float yaw = orientation.eulerAngles.y + mouseX;

        orientation.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCam.rotation = Quaternion.Euler(xRotation, yaw, 0f);

        if (IsOwner)
            SendYawServerRpc(yaw);
    }


    [ServerRpc]
    private void MoveServerRpc(float inputX, float inputY)
    {
        if (!rb) return;

        Vector3 moveDir = (orientation.forward * inputY + orientation.right * inputX).normalized;
        rb.AddForce(moveDir * moveSpeed * Time.fixedDeltaTime);

        CounterMovement(inputX, inputY);
        serverYaw.Value = orientation.eulerAngles.y;
    }

    [ServerRpc]
    private void JumpServerRpc()
    {
        if (!grounded || !readyToJump) return;
        readyToJump = false;

        rb.AddForce(Vector3.up * jumpForce * 1.5f);
        Vector3 vel = rb.velocity;
        if (vel.y < 0.5f)
            rb.velocity = new Vector3(vel.x, 0, vel.z);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    [ServerRpc]
    private void CrouchServerRpc(bool state)
    {
        crouching = state;
        if (state)
        {
            transform.localScale = crouchScale;
            transform.position -= new Vector3(0, 0.5f, 0);
            if (rb.velocity.magnitude > 0.5f && grounded)
                rb.AddForce(orientation.forward * slideForce);
        }
        else
        {
            transform.localScale = playerScale;
            transform.position += new Vector3(0, 0.5f, 0);
        }
    }

    [ServerRpc]
    private void SendYawServerRpc(float yaw)
    {
        serverYaw.Value = yaw;
    }

    private void LateUpdate()
    {
        // Remote players smoothly rotate based on server data
        if (!IsOwner)
        {
            Quaternion targetRot = Quaternion.Euler(0, serverYaw.Value, 0);
            orientation.rotation = Quaternion.Lerp(orientation.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    private void ResetJump() => readyToJump = true;

    private void CounterMovement(float x, float y)
    {
        if (!grounded) return;

        Vector2 mag = new(rb.velocity.x, rb.velocity.z);
        if (Mathf.Abs(mag.x) > maxSpeed) x = 0;
        if (Mathf.Abs(mag.y) > maxSpeed) y = 0;

        rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * (crouching ? slideCounterMovement : counterMovement));
    }

    private void OnCollisionStay(Collision other)
    {
        if ((whatIsGround.value & (1 << other.gameObject.layer)) == 0) return;
        grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        grounded = false;
    }
}
