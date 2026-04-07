using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Coffee"))
        {
            GetComponent<NetworkedPlayerAdvanced>().moveSpeed += GetComponent<NetworkedPlayerAdvanced>().moveSpeed;
            Destroy(other.gameObject);
        }
    }
}
