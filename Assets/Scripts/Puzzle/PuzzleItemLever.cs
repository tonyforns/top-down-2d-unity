using UnityEngine;

public class PuzzleItemLever : PuzzleItem
{
    [SerializeField] private SpriteRenderer leverSpriteRenderer;
    override public void Interact()
    {
        base.Interact();
        leverSpriteRenderer.flipX = !leverSpriteRenderer.flipX;
    }
}
