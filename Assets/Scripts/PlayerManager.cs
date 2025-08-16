using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerManager : MonoBehaviour
{
    public PlayerMovement[] players;
    public PlayerShooting[] playerShootings;
    public int currentIndex = 0;

    public TextMeshProUGUI nameText;

    public CameraController cameraController;

    void Start()
    {
        SetActivePlayer(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActivePlayer(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetActivePlayer(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetActivePlayer(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetActivePlayer(3);
    }

    void SetActivePlayer(int index)
    {
        if (index < 0 || index >= players.Length) return;

        for (int i = 0; i < players.Length; i++)
        {
            players[i].enabled = (i == index); // Only enable the current one
            playerShootings[i].enabled = (i == index);
        }

        currentIndex = index;
        cameraController.FocusOnPlayer(players[index].transform);

        if (nameText != null)
        {
            nameText.text = "Selected: " + players[index].name;
        }
    }

    public PlayerMovement GetCurrentPlayer()
    {
        return players[currentIndex];
    }

    public PlayerShooting GetCurrentShooter()
    {
        return playerShootings[currentIndex];
    }
}
