using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator2D : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;

    [Header("Input (opcional si ya tenés tu sistema)")]
    public InputActionReference moveAction;   // Vector2
    public InputActionReference lookAction;   // Vector2 (mouse screen pos)
    public InputActionReference fireAction;   // Button (opcional)

    [Header("Tuning")]
    public float deadZone = 0.1f;

    Vector2 _lastLookDir = Vector2.down;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (animator == null) return;

        Vector2 move = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        Vector2 lookScreen = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        bool isFiring = fireAction != null && fireAction.action.IsPressed();

        bool isMoving = move.sqrMagnitude > deadZone * deadZone;
        animator.SetBool("IsMoving", isMoving);

        // 1) Determinar hacia dónde "mira"
        Vector2 lookDir = _lastLookDir;

        if (isFiring)
        {
            // Mira hacia el mouse
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(lookScreen.x, lookScreen.y, 0f));
            Vector2 dir = (mouseWorld - transform.position);
            dir.Normalize();

            if (dir.sqrMagnitude > 0.0001f)
                lookDir = dir;
        }
        else if (isMoving)
        {
            // Si no dispara, mira hacia donde camina
            lookDir = move.normalized;
        }

        _lastLookDir = lookDir;

        // 2) Convertir lookDir a Dir int (0=S,1=N,2=E,3=W)
        int dirInt = DirFromVector(lookDir);
        animator.SetInteger("Direction", dirInt);
    }

    int DirFromVector(Vector2 v)
    {
        // Elegimos el eje dominante (tipo Zelda/Isaac)
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return v.x >= 0 ? 2 : 3; // E : W
        else
            return v.y >= 0 ? 1 : 0; // N : S
    }
}