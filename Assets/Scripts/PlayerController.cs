using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Vector3 inputVector;
    private bool grounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
            rb.isKinematic = false;
        else
            rb.isKinematic = true;
    }

    private void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputVector = new Vector3(h, 0, v).normalized;

        // Movement
        if (IsServer)
        {
            MovePlayer(inputVector);
        }
        else
        {
            SubmitMovementServerRpc(new Vector2(h, v));
        }

        // Jump input (DO NOT check grounded here)
        if (Input.GetButtonDown("Jump"))
        {
            RequestJumpServerRpc();
        }
    }

    [ServerRpc]
    private void SubmitMovementServerRpc(Vector2 input)
    {
        Vector3 moveInput = new Vector3(input.x, 0, input.y).normalized;
        MovePlayer(moveInput);
    }

    private void MovePlayer(Vector3 moveInput)
    {
        if (moveInput.sqrMagnitude < 0.001f) return;

        Vector3 targetPos = rb.position + moveInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(targetPos);

        Quaternion targetRot = Quaternion.LookRotation(moveInput, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
    }

    [ServerRpc]
    private void RequestJumpServerRpc()
    {
        if (!grounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!IsServer) return;

        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!IsServer) return;

        if ((groundLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            grounded = false;
        }
    }
}