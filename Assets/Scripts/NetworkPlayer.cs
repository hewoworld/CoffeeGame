using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] GameObject[] gameObjectsToDisable;
    [SerializeField] Behaviour[] componentsToDisable;

    // Start is called before the first frame update
    void Start()
    {
        if (IsLocalPlayer) return;
        for (int i = 0; i < gameObjectsToDisable.Length; i++)
        {
            gameObjectsToDisable[i].SetActive(false);
        }

        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
