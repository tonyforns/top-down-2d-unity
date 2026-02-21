using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerTopDownMovement_InputSystem : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private bool normalizeDiagonal = true;

    [SerializeField] private InputActionReference moveAction;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void Update()
    {
        if (moveAction == null) return;

        moveInput = moveAction.action.ReadValue<Vector2>();
        if (normalizeDiagonal && moveInput.sqrMagnitude > 1f)
            moveInput = moveInput.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;

        Debug.Log("ATTACK pressed");

        var forms = GetComponent<PlayerVisualForms>();
        if (forms == null)
        {
            Debug.Log("PlayerVisualForms is NULL");
            return;
        }

        var anim = forms.GetCurrentAnimator();
        Debug.Log(anim != null
            ? $"Animator target: {anim.name} | controller: {anim.runtimeAnimatorController.name}"
            : "Animator is NULL");

        if (anim == null) return;

        anim.SetTrigger("Attack");
    }
}