using System.Collections;
using UnityEngine;
using Dialogue;

[RequireComponent(typeof(Rigidbody2D))]
public class WarriorBossAI : MonoBehaviour
{
    private enum State { Patrol, Duel, Guard, Attack, Dead }

    [Header("Stats")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private int maxHP = 10;
    [SerializeField] private float moveSpeed = 2.4f;
    [SerializeField] private float enragedMoveSpeed = 2.9f;

    [Header("Detection")]
    [SerializeField] private float visionRadius = 6f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Duel Distance")]
    [SerializeField] private float preferredDistanceMin = 1.4f;
    [SerializeField] private float preferredDistanceMax = 2.2f;
    [SerializeField] private float strafeStrength = 0.6f;
    [SerializeField] private float strafeChangeInterval = 0.8f;

    [Header("Attacks")]
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackCooldown = 1.1f;          
    [SerializeField] private float enragedAttackCooldown = 0.65f;   
    [SerializeField] private float specialChanceEnraged = 0.35f;  

    [Header("Guard")]
    [SerializeField] private int damagePerGuard = 3;
    [SerializeField] private float guardDuration = 3f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string speedParam = "Speed";
    [SerializeField] private string runParam = "Run";
    [SerializeField] private string attack1Trigger = "Attack1";
    [SerializeField] private string attack2Trigger = "Attack2";
    [SerializeField] private string defendTrigger = "Defend";

    [Header("Death Rewards")]
    [SerializeField] private DialogueData deathDialogue;
    [SerializeField] private ItemData swordItem;

    [Header("Patrol (optional)")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointReachDist = 0.2f;

    private Rigidbody2D rb;
    private Transform player;

    private int hp;
    private int nextGuardAt;   
    private bool enraged;
    private bool invulnerable;

    private State state = State.Patrol;
    private int patrolIndex;

    private float lastAttackTime;
    private float strafeSign = 1f;
    private float nextStrafeChangeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (animator == null) animator = GetComponentInChildren<Animator>();

        hp = maxHP;
        nextGuardAt = maxHP - damagePerGuard;
        nextStrafeChangeTime = Time.time + strafeChangeInterval;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (state == State.Dead) return;

        player = FindPlayer();
        if (player == null)
        {
            state = patrolPoints != null && patrolPoints.Length > 0 ? State.Patrol : State.Duel;
            return;
        }
        if (facePlayer && spriteRenderer != null)
        {
            float dx = player.position.x - transform.position.x;
            if (Mathf.Abs(dx) > 0.01f)
                spriteRenderer.flipX = dx < 0f; 
        }

        if (!enraged && hp <= maxHP / 2)
            enraged = true;

        if (state == State.Guard) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time >= lastAttackTime + CurrentAttackCooldown())
        {
            state = State.Attack;
            DoAttack(dist);
            return;
        }

        state = State.Duel;
    }

    private void FixedUpdate()
    {
        if (state == State.Dead || state == State.Guard || player == null)
        {
            SetMove(Vector2.zero);
            return;
        }

        if (state == State.Patrol)
        {
            PatrolMove();
            return;
        }

        if (state == State.Duel)
        {
            DuelMove();
            return;
        }

        if (state == State.Attack)
        {
            SetMove(Vector2.zero);
            return;
        }
    }

    private Transform FindPlayer()
    {
        var hit = Physics2D.OverlapCircle(transform.position, visionRadius, playerLayer);
        return hit != null ? hit.transform : null;
    }

    private void PatrolMove()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) { SetMove(Vector2.zero); return; }

        Vector2 target = patrolPoints[patrolIndex].position;
        Vector2 to = target - (Vector2)transform.position;

        if (to.magnitude <= waypointReachDist)
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

        SetMove(to.normalized * CurrentMoveSpeed());
    }

    private void DuelMove()
    {
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float dist = toPlayer.magnitude;

        if (Time.time >= nextStrafeChangeTime)
        {
            strafeSign = Random.value < 0.5f ? -1f : 1f;
            nextStrafeChangeTime = Time.time + strafeChangeInterval;
        }

        Vector2 dirToPlayer = dist > 0.001f ? (toPlayer / dist) : Vector2.right;

        Vector2 strafe = new Vector2(-dirToPlayer.y, dirToPlayer.x) * (strafeStrength * strafeSign);

        Vector2 desired;

        if (dist > preferredDistanceMax)
        {
            desired = dirToPlayer + strafe;
        }
        else if (dist < preferredDistanceMin)
        {
            desired = -dirToPlayer + strafe;
        }
        else
        {
            desired = strafe;
        }

        desired = desired.normalized * CurrentMoveSpeed();
        SetMove(desired);
    }

    private void DoAttack(float dist)
    {
        lastAttackTime = Time.time;

        if (animator != null)
        {
            bool useSpecial = enraged && Random.value < specialChanceEnraged;

            animator.ResetTrigger(attack1Trigger);
            animator.ResetTrigger(attack2Trigger);

            animator.SetTrigger(useSpecial ? attack2Trigger : attack1Trigger);
        }

        StartCoroutine(ReturnToDuelAfter(0.35f));
    }

    private IEnumerator ReturnToDuelAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (state != State.Dead && state != State.Guard)
            state = State.Duel;
    }

    private float CurrentMoveSpeed() => enraged ? enragedMoveSpeed : moveSpeed;
    private float CurrentAttackCooldown() => enraged ? enragedAttackCooldown : attackCooldown;

    private void SetMove(Vector2 vel)
    {
        rb.linearVelocity = vel;

        if (animator != null)
        {
            float spd = vel.magnitude;
            animator.SetFloat(speedParam, spd);
            animator.SetBool(runParam, spd > 0.05f);
        }
    }


    public void TakeDamage(int amount)
    {
        if (state == State.Dead) return;
        if (invulnerable) return;

        hp -= amount;
        if (hp <= 0)
        {
            Die();
            return;
        }

        if (hp <= nextGuardAt)
        {
            nextGuardAt -= damagePerGuard;
            StartCoroutine(GuardRoutine());
        }
    }

    private IEnumerator GuardRoutine()
    {
        state = State.Guard;
        invulnerable = true;
        SetMove(Vector2.zero);

        if (animator != null)
            animator.SetTrigger(defendTrigger);

        yield return new WaitForSeconds(guardDuration);

        invulnerable = false;
        if (state != State.Dead)
            state = player != null ? State.Duel : State.Patrol;
    }

    private void Die()
{
    state = State.Dead;
    invulnerable = true;
    SetMove(Vector2.zero);

    if (swordItem != null && Inventory.Instance != null)
        Inventory.Instance.AddItem(swordItem, 1);

    if (deathDialogue != null && DialogueManager.Instance != null)
    {
        DialogueManager.Instance.OnDialogueComplete += DestroyAfterDialogue;
        DialogueManager.Instance.StartDialogue(deathDialogue);
    }
    else
    {
        Destroy(gameObject);
    }
        }

        private void DestroyAfterDialogue()
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueComplete -= DestroyAfterDialogue;

            Destroy(gameObject);
        }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, preferredDistanceMin);
        Gizmos.DrawWireSphere(transform.position, preferredDistanceMax);
    }
}