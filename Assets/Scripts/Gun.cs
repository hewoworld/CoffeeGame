using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gunPoint;
    [SerializeField] float bulletSpeed = 10f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject boolet = Instantiate(bullet, gunPoint.position, gunPoint.rotation);

            Rigidbody rb = boolet.GetComponent<Rigidbody>();

            rb.isKinematic = false; // force it off
            rb.AddForce(gunPoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}