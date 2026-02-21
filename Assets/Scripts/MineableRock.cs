using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MineableRock : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData dropItem;
    [SerializeField] private int dropAmount = 1;
    [SerializeField] private int hitsToBreak = 1;
    [SerializeField] private string promptText = "Minar";

    private int hits;

    public void Interact()
    {
        if (dropItem == null) return;
        if (Inventory.Instance == null) return;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var facing = player.GetComponent<PlayerFacing>();
            var tool = player.GetComponent<PlayerToolSwing>();
            if (facing != null && tool != null)
                tool.PlayPickaxe(facing.GetFacing4());
        }
        hits++;
        if (hits < hitsToBreak) return;

        int added = Inventory.Instance.AddItem(dropItem, dropAmount);
        if (added <= 0) return;

        Destroy(gameObject);
    }

    public string GetPromptText() => promptText;
}