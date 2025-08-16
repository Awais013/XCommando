using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public PlayerMovement playerMovementRef;
    private Animator Animator;

    public GameObject hitPoint;
    public GameObject flash;

    public Transform firePoint;

    private void Start()
    {
        Animator = playerMovementRef.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerMovementRef.IsAiming())
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }
    }

    void Shoot()
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

                rb.AddForce(forceDirection * 500f); // Adjust force multiplier as needed
            }

            EnemyVision enemy = hit.collider.GetComponent<EnemyVision>();
            if (enemy != null) {
                enemy.TakeDamage(1);
            }
        }
    }
}
