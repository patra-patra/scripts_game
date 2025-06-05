using UnityEngine;

public class SimplePlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    [Header("Input")]
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(forwardKey))
            movement += transform.forward;
        if (Input.GetKey(backwardKey))
            movement -= transform.forward;
        if (Input.GetKey(leftKey))
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        if (Input.GetKey(rightKey))
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Применяем движение
        transform.position += movement.normalized * moveSpeed * Time.deltaTime;
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, Screen.height - 120, 300, 100));
        GUILayout.Label("=== Управление ===");
        GUILayout.Label("WASD - движение");
        GUILayout.Label("E - взаимодействие с NPC");
        GUILayout.EndArea();
    }
}