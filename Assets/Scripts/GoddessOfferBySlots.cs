using System;
using UnityEngine;
using Dialogue;

[RequireComponent(typeof(Collider2D))]
public class GoddessOfferBySlots : MonoBehaviour, IInteractable
{
    [Serializable]
    public struct SlotRequirement
    {
        [Min(0)] public int slotIndex;
        public ItemData requiredItem;
        [Min(1)] public int requiredAmount;
    }

    [Header("Requirements (by slot)")]
    [SerializeField] private SlotRequirement[] requirements;

    [Header("Dialogues")]
    [SerializeField] private DialogueData successDialogue;
    [SerializeField] private DialogueData failDialogue;
    [SerializeField] private DialogueData alreadyCompletedDialogue;

    [Header("Behavior")]
    [SerializeField] private bool consumeItemsOnSuccess = true;
    [SerializeField] private bool oneTimeOnly = true;

    [Header("On Success Actions")]
    [SerializeField] private PlayerVisualForms playerVisuals;
    [SerializeField] private int setPlayerFormIndex = 1;
    [SerializeField] private GameObject teleporterToUnlock;

    [Header("UI Prompt")]
    [SerializeField] private string promptText = "Ofrecer";

    private bool completed;

    public void Interact()
    {
        Debug.Log("[GoddessOfferBySlots] Interact() called");

        if (oneTimeOnly && completed)
        {
            Debug.Log("[GoddessOfferBySlots] Already completed");
            PlayDialogue(alreadyCompletedDialogue != null ? alreadyCompletedDialogue : successDialogue);
            return;
        }

        var inv = Inventory.Instance;
        if (inv == null)
        {
            Debug.LogWarning("[GoddessOfferBySlots] Inventory.Instance is NULL");
            return;
        }

        bool ok = CheckRequirements(inv);
        Debug.Log($"[GoddessOfferBySlots] Requirements ok = {ok}");

        if (!ok)
        {
            PlayDialogue(failDialogue);
            return;
        }

        if (consumeItemsOnSuccess)
            Consume(inv);

        completed = true;
        Debug.Log("[GoddessOfferBySlots] SUCCESS");

        if (playerVisuals != null)
        {
            Debug.Log("[GoddessOfferBySlots] Setting player form");
            playerVisuals.SetForm(setPlayerFormIndex);
        }
        else
        {
            Debug.LogWarning("[GoddessOfferBySlots] playerVisuals is NULL");
        }

        if (teleporterToUnlock != null)
        {
            Debug.Log($"[GoddessOfferBySlots] Teleporter ref = {teleporterToUnlock.name} | activeSelf(before)={teleporterToUnlock.activeSelf} | inScene={teleporterToUnlock.scene.IsValid()}");

            teleporterToUnlock.SetActive(true);

            Debug.Log($"[GoddessOfferBySlots] Teleporter activeSelf(after)={teleporterToUnlock.activeSelf}");

            var portal = teleporterToUnlock.GetComponent<PortalToSceneInteractable>();
            if (portal != null)
            {
                portal.SetUnlocked(true);
                Debug.Log("[GoddessOfferBySlots] Portal unlocked = true");
            }
        }
        else
        {
            Debug.LogWarning("[GoddessOfferBySlots] teleporterToUnlock is NULL (no asignado)");
        }
        var closer = FindObjectOfType<CloseGameAfterDialogue>();
        if (closer != null) closer.ArmIfFinalDialogue(successDialogue);
        PlayDialogue(successDialogue);
        
    }

    public string GetPromptText() => promptText;

    private bool CheckRequirements(Inventory inv)
    {
        if (requirements == null || requirements.Length == 0) return true;

        for (int i = 0; i < requirements.Length; i++)
        {
            var req = requirements[i];

            if (req.requiredItem == null || req.requiredAmount <= 0)
                return false;

            var stack = inv.GetSlot(req.slotIndex);

            if (stack.IsEmpty) return false;
            if (stack.Data != req.requiredItem) return false;
            if (stack.Amount < req.requiredAmount) return false;
        }

        return true;
    }

    private void Consume(Inventory inv)
    {
        if (requirements == null) return;

        for (int i = 0; i < requirements.Length; i++)
        {
            var req = requirements[i];
            inv.RemoveAt(req.slotIndex, req.requiredAmount);
        }
    }

    private void PlayDialogue(DialogueData data)
    {
        if (data == null) return;
        if (DialogueManager.Instance == null) return;
        DialogueManager.Instance.StartDialogue(data);
    }
}