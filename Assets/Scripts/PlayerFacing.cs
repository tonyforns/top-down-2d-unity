using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    [SerializeField] private PlayerVisualForms visualForms;

    private static readonly int LastMoveX = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveY = Animator.StringToHash("LastMoveY");

    private void Awake()
    {
        if (visualForms == null) visualForms = GetComponent<PlayerVisualForms>();
    }

    public Vector2 GetFacing4()
    {
        var anim = visualForms != null ? visualForms.GetCurrentAnimator() : null;
        if (anim == null) return Vector2.down;

        float x = anim.GetFloat(LastMoveX);
        float y = anim.GetFloat(LastMoveY);

        if (Mathf.Abs(x) > Mathf.Abs(y)) return new Vector2(Mathf.Sign(x), 0);
        if (Mathf.Abs(y) > 0.01f) return new Vector2(0, Mathf.Sign(y));
        return Vector2.down;
    }
}