using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100;
    float health;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(health == 0)
        {
            transform.position = Vector3.zero;
            health = maxHealth;
        }
        print(health);
    }

    public void DealDamage(float damage)
    {
        health -= damage;
    }
}
