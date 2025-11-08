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

    private Rigidbody rb;
    private Vector3 inputVector;

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

        // Get player input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        inputVector = new Vector3(h, 0, v).normalized;

        // If host, move immediately (because host is the server)
        if (IsServer)
        {
            MovePlayer(inputVector);
        }
        else
        {
            // If client, send input to server
            SubmitMovementServerRpc(new Vector2(h, v));
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

        Vector3 targetPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        Quaternion targetRot = Quaternion.LookRotation(moveInput, Vector3.up);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, rotationSpeed * Time.deltaTime));
    }
}
