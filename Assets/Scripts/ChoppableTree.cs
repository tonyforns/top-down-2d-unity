using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ChoppableTree : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData dropItem;
    [SerializeField] private int dropAmount = 1;
    [SerializeField] private int hitsToChop = 3;
    [SerializeField] private string promptText = "Talar";

    private int hits;

    public void Interact()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var facing = player.GetComponent<PlayerFacing>();
            var tool = player.GetComponent<PlayerToolSwing>();
            if (facing != null && tool != null)
                tool.PlayAxe(facing.GetFacing4());
        }

        hits++;
        if (hits < hitsToChop) return;

        if (dropItem != null && Inventory.Instance != null)
            Inventory.Instance.AddItem(dropItem, dropAmount);

        Destroy(gameObject);
    }

    public string GetPromptText() => promptText;
}