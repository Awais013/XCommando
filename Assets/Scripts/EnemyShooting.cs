using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    private Animator Animator;

    public GameObject hitPoint;
    public GameObject flash;

    public Transform firePoint;

    private int shootDelay;

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }
    public void Shoot()
    {
        RaycastHit hit;
        Animator.SetTrigger("Shoot");
        Destroy(Instantiate(flash, firePoint.position, Quaternion.identity), 0.5f);

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 100))
        {
            Destroy(Instantiate(hitPoint, hit.point, Quaternion.identity), 1.5f);
            // Apply force if the hit object has a Rigidbody
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log(hit.collider.name);
                Vector3 forceDirection = hit.point - firePoint.position;
                forceDirection.Normalize();

                rb.AddForce(forceDirection * 5f); // Adjust force multiplier as needed
            }

            PlayerMovement enemy = hit.collider.GetComponent<PlayerMovement>();
            if (enemy != null)
            {
                Debug.Log("Player got hit !");
            }
        }
    }
}
