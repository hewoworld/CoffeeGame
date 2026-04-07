using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBullet : MonoBehaviour
{
    [Header("Assignables")]
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    [Header("Stats")]
    [Range(0, 1)]
    public float bounciness;
    public bool useGravity;

    [Header("Damage")]
    public int explosionDamage;
    public float explosionRange;
    public string enemyString;

    [Header("LifeTime")]
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch;

    int collisions;
    PhysicMaterial physic_mat;

    private void Setup()
    {
        physic_mat = new PhysicMaterial();
        physic_mat.bounciness = bounciness;
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        rb.velocity = transform.forward * 10 * Time.deltaTime;

        if (collisions >= maxCollisions) Explode();

        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <= 0) Explode();
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (collision.collider.CompareTag(enemyString) && explodeOnTouch) Explode();
    }

    void Explode()
    {
        if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            Debug.Log("found" + enemies[i].transform.name);
        }
        Destroy(gameObject, 0.05f);
    }
}
