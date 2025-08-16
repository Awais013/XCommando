using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float smoothTime = 0.1f;
    public float zoomSpeed = 10f;
    public float rotationSpeed = 90f; // Degrees per second

    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;

    private Camera mainCam;

    public Transform followTarget;
    public float followSmoothSpeed = 5f;

    private float currentRotation = 0f;
    [SerializeField] private Vector3 followOffset;

    void Start()
    {
        targetPosition = transform.position;
        mainCam = Camera.main;

        if (followTarget != null)
            followOffset = transform.position - followTarget.position;
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotationInput();
    }

    void LateUpdate()
    {
        if (followTarget != null)
        {
            Vector3 targetPos = followTarget.position + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, followSmoothSpeed * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            inputDir += new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            inputDir += new Vector3(-1, 0, -1);
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputDir += new Vector3(1, 0, -1);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputDir += new Vector3(-1, 0, 1);

        inputDir.Normalize();

        // Rotate input direction with current Y rotation
        inputDir = Quaternion.Euler(0, currentRotation, 0) * inputDir;

        targetPosition += inputDir * moveSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            mainCam.orthographicSize -= scroll * zoomSpeed;
            mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize, 5f, 20f);
        }
    }

    void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            currentRotation -= rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

    public void FocusOnPlayer(Transform playerTransform)
    {
        if (followTarget == playerTransform)
        {
            // Already following → toggle off
            followTarget = null;
            Debug.Log("Camera follow disabled.");
        }
        else
        {
            // Start following
            followTarget = playerTransform;
            followOffset = transform.position - playerTransform.position;
            Debug.Log("Camera now following player.");
        }
    }


}
