using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerVisualForms visualForms;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int LastMoveX = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveY = Animator.StringToHash("LastMoveY");
    private static readonly int Attack = Animator.StringToHash("Attack");

    private Vector2 lastDir = Vector2.down;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (visualForms == null) visualForms = GetComponent<PlayerVisualForms>();
    }

    private void Update()
    {
        var anim = visualForms != null ? visualForms.GetCurrentAnimator() : null;
        if (anim == null || rb == null) return;

        Vector2 v = rb.linearVelocity;

        Vector2 dir = v.sqrMagnitude > 0.001f ? v.normalized : lastDir;
        if (v.sqrMagnitude > 0.001f)
            lastDir = Get4Dir(dir);

        Vector2 move4 = Get4Dir(new Vector2(v.x, v.y));

        anim.SetFloat(MoveX, move4.x);
        anim.SetFloat(MoveY, move4.y);
        anim.SetFloat(Speed, v.magnitude);

        anim.SetFloat(LastMoveX, lastDir.x);
        anim.SetFloat(LastMoveY, lastDir.y);
    }

    public Vector2 GetFacing4() => lastDir;

    public void PlayAttack()
    {
        var anim = visualForms != null ? visualForms.GetCurrentAnimator() : null;
        if (anim == null) return;
        anim.SetTrigger(Attack);
    }

    private Vector2 Get4Dir(Vector2 v)
    {
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return new Vector2(Mathf.Sign(v.x), 0f);
        if (Mathf.Abs(v.y) > 0.001f)
            return new Vector2(0f, Mathf.Sign(v.y));
        return Vector2.zero;
    }
}