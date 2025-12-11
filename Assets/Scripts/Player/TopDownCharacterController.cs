using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Refs")]
    public Rigidbody2D rb;
    public Camera mainCamera;
    public Transform aimPivot;

    private Vector2 moveInput;
    private Vector2 mouseWorldPos;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!mainCamera) mainCamera = Camera.main;
    }

    // Llamado por PlayerInput (acción "Move")
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Llamado por PlayerInput (acción "Look")
    public void OnLook(InputValue value)
    {
        Vector2 screenPos = value.Get<Vector2>();
        mouseWorldPos = mainCamera.ScreenToWorldPoint(screenPos);
    }

    private void FixedUpdate()
    {
        // Movimiento
        rb.linearVelocity = moveInput * moveSpeed;

        // Apuntado hacia el mouse
        Vector2 lookDir = mouseWorldPos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        if (aimPivot != null)
        {
            aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
