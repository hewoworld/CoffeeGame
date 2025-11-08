using Unity.Netcode;
using UnityEngine;

public class MoveCamera : NetworkBehaviour
{

    public Transform player;

    void Update()
    {
        if (!IsOwner) return;
        transform.position = player.transform.position;
    }
}