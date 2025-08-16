using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public Camera cam;
    public PlayerManager playerManager;

    private float lastClickTime = 0f;
    private float doubleClickThreshold = 0.25f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    bool isDoubleClick = (Time.time - lastClickTime) <= doubleClickThreshold;
                    lastClickTime = Time.time;

                    PlayerMovement currentPlayer = playerManager.GetCurrentPlayer();
                    PlayerShooting currentShooter = playerManager.GetCurrentShooter();

                    currentPlayer.MoveTo(hit.point, isDoubleClick);

                }
            }
        }
    }
}
