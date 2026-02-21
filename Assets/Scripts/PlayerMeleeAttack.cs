using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Hit Settings")]
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private float attackOffset = 0.7f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int damage = 1;

    [Header("Facing/Animator")]
    [SerializeField] private PlayerVisualForms visualForms;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private bool debugDraw = true;

    private static readonly int LastMoveX = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveY = Animator.StringToHash("LastMoveY");

    private Vector2 lastOrigin;
    private Vector2 lastFacing;

    private void Awake()
    {
        if (visualForms == null) visualForms = GetComponent<PlayerVisualForms>();
    }

    // IMPORTANT: si usas PlayerInput "Send Messages" y tu action se llama "Attack",
    // este método debe llamarse EXACTO "OnAttack".
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        DoAttack();
    }

    private void DoAttack()
    {
        var anim = visualForms != null ? visualForms.GetCurrentAnimator() : null;

        if (anim != null)
            anim.SetTrigger("Attack");

        Vector2 facing = GetFacing4(anim);
        Vector2 origin = (Vector2)transform.position + facing * attackOffset;

        lastFacing = facing;
        lastOrigin = origin;

        if (debugLogs)
        {
            string animInfo = anim != null && anim.runtimeAnimatorController != null
                ? $"{anim.name} | {anim.runtimeAnimatorController.name}"
                : "NULL";
            Debug.Log($"[PlayerAttack] DoAttack() facing={facing} origin={origin} anim={animInfo}");
        }

        // En vez de OverlapCircle (uno solo), usamos OverlapCircleAll para ver qué está tocando.
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, attackRadius, enemyLayer);

        if (debugLogs)
            Debug.Log($"[PlayerAttack] Hits found: {hits.Length} (enemyLayer mask={enemyLayer.value})");

        if (hits.Length == 0) return;

        // Log de lo que golpeamos
        for (int i = 0; i < hits.Length; i++)
        {
            if (debugLogs) Debug.Log($"[PlayerAttack] Hit[{i}] = {hits[i].name} (layer={LayerMask.LayerToName(hits[i].gameObject.layer)})");

            // MUY IMPORTANTE: muchas veces el collider está en un hijo del boss.
            var boss = hits[i].GetComponentInParent<WarriorBossAI>();
            if (boss != null)
            {
                if (debugLogs) Debug.Log("[PlayerAttack] WarriorBossAI found -> applying damage");
                boss.TakeDamage(damage);
                return;
            }
        }

        if (debugLogs)
            Debug.Log("[PlayerAttack] No WarriorBossAI found on hits (check collider hierarchy / enemyLayer)");
    }

    private Vector2 GetFacing4(Animator anim)
    {
        if (anim == null) return Vector2.down;

        float x = anim.GetFloat(LastMoveX);
        float y = anim.GetFloat(LastMoveY);

        if (Mathf.Abs(x) > Mathf.Abs(y)) return new Vector2(Mathf.Sign(x), 0);
        if (Mathf.Abs(y) > 0.01f) return new Vector2(0, Mathf.Sign(y));
        return Vector2.down;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // Si estamos en play y ya atacamos alguna vez, dibuja el último origin real.
        Vector3 center = Application.isPlaying ? (Vector3)lastOrigin : transform.position;
        Gizmos.DrawWireSphere(center, attackRadius);

        // Línea de facing
        if (Application.isPlaying)
        {
            Gizmos.DrawLine(transform.position, (Vector2)transform.position + lastFacing * attackOffset);
        }
    }

    private void Update()
    {
        if (!debugDraw) return;

        // Dibujo en pantalla en play (más fácil de ver que Gizmos)
        if (Application.isPlaying)
        {
            Debug.DrawLine(transform.position, lastOrigin, Color.cyan);
        }
    }
}