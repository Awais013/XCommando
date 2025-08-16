using UnityEngine;

public class TargetControl : MonoBehaviour
{
    public GameObject targetPrefab; // Assign the prefab in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Instantiate(targetPrefab, new Vector3(8, 2, -1), Quaternion.identity); // Default pos
            Destroy(gameObject);
        }
    }
}
