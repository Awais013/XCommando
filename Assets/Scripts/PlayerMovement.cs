using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 7f;


    private float currentSpeed = 3f;
    private bool aim = false;

    private Animator animator;
    private Vector3 lastPosition;

    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // we handle rotation manually
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleAnimations();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            aim = !aim;
            animator.SetBool("Aim", aim);
            Debug.Log("Aim toggled: " + aim);
        }
    }

    public bool IsAiming()
    {
        return aim;
    }

    private void HandleRotation()
    {
        if (aim)
        {
            // Rotate toward mouse cursor

            //Create raycast from camera to mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Vector3 lookDir = hit.point - transform.position;
                    lookDir.y = 0;

                    if (lookDir != Vector3.zero)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(lookDir);
                        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 20f * Time.deltaTime);
                    }
                }
            }
        }
        else if (agent.velocity.sqrMagnitude > 0.01f)
        {
            // Rotate in direction of movement
            Vector3 moveDir = agent.velocity.normalized;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 40f * Time.deltaTime);
        }
    }

    public void MoveTo(Vector3 destination, bool sprint)
    {
        currentSpeed = sprint ? runSpeed : walkSpeed;
        agent.speed = currentSpeed;
        agent.SetDestination(destination);
    }

    private void HandleAnimations()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Velocity", speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cover"))
        {
            animator.SetTrigger("TakeCover");
            Debug.Log("Entered cover zone!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cover"))
        {
            animator.SetTrigger("ExitCover");
            Debug.Log("Exited cover zone!");
        }
    }
}
