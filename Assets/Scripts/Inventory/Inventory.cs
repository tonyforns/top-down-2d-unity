using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contenedor de ítems con slots fijos, respaldado por un Dictionary (índice de slot → ItemStack).
/// Solo se almacenan slots no vacíos. Singleton accesible desde jugador, UI y pickups.
/// </summary>
public class Inventory : Singleton<Inventory>
{
    [Header("Configuración")]
    [SerializeField] private int slotCount = 20;

    /// <summary>Se invoca cuando cambia cualquier slot (añadir, quitar, mover).</summary>
    public event Action OnInventoryChanged;

    private Dictionary<int, ItemStack> _slots;

    public int SlotCount => _slots != null ? slotCount : 0;

    private void Awake()
    {
        base.Awake();
        _slots = new Dictionary<int, ItemStack>();
    }

    /// <summary>Devuelve una copia del stack en el slot (no modificar directamente).</summary>
    public ItemStack GetSlot(int index)
    {
        if (_slots == null || index < 0 || index >= slotCount)
            return default;
        return _slots.TryGetValue(index, out var stack) ? stack : default;
    }

    /// <summary>Cantidad total de un tipo de ítem en el inventario.</summary>
    public int GetTotalCount(ItemData data)
    {
        if (data == null || _slots == null) return 0;
        int total = 0;
        foreach (var stack in _slots.Values)
        {
            if (stack.Data == data)
                total += stack.Amount;
        }
        return total;
    }

    /// <summary>True si hay al menos 'amount' del ítem.</summary>
    public bool HasItem(ItemData data, int amount = 1) => GetTotalCount(data) >= amount;

    /// <summary>Cantidad de ese ítem que cabría añadir sin modificar el inventario (para comprobar espacio).</summary>
    public int CountSpaceFor(ItemData data, int amount = 1)
    {
        if (data == null || amount <= 0 || _slots == null) return 0;
        int remaining = amount;

        if (data.IsStackable)
        {
            foreach (var stack in _slots.Values)
            {
                if (remaining <= 0) break;
                if (stack.Data != data) continue;
                remaining -= Math.Min(remaining, stack.SpaceLeft);
            }
        }

        int emptySlots = 0;
        for (int i = 0; i < slotCount; i++)
            if (!_slots.ContainsKey(i)) emptySlots++;

        if (data.IsStackable)
            remaining -= Math.Min(remaining, emptySlots * data.MaxStackSize);
        else
            remaining -= Math.Min(remaining, emptySlots);

        return amount - Math.Max(0, remaining);
    }

    /// <summary>Añade ítem al inventario. Devuelve la cantidad que se pudo añadir.</summary>
    public int AddItem(ItemData data, int amount = 1)
    {
        if (data == null || amount <= 0 || _slots == null) return 0;
        int remaining = amount;

        if (data.IsStackable)
        {
            var keys = new List<int>(_slots.Keys);
            foreach (int i in keys)
            {
                if (remaining <= 0) break;
                if (!_slots.TryGetValue(i, out var stack) || stack.Data != data) continue;
                int add = Math.Min(remaining, stack.SpaceLeft);
                if (add <= 0) continue;
                stack.Amount += add;
                _slots[i] = stack;
                remaining -= add;
            }
        }

        for (int i = 0; i < slotCount && remaining > 0; i++)
        {
            if (_slots.ContainsKey(i)) continue;
            int add = data.IsStackable ? Math.Min(remaining, data.MaxStackSize) : 1;
            _slots[i] = new ItemStack(data, add);
            remaining -= add;
        }

        int added = amount - remaining;
        if (added > 0)
            OnInventoryChanged?.Invoke();
        return added;
    }

    /// <summary>Quita 'amount' del ítem. Devuelve la cantidad que se pudo quitar.</summary>
    public int RemoveItem(ItemData data, int amount = 1)
    {
        if (data == null || amount <= 0 || _slots == null) return 0;
        int remaining = amount;
        var keysToUpdate = new List<int>();

        foreach (var kvp in _slots)
        {
            if (remaining <= 0) break;
            if (kvp.Value.Data != data) continue;
            int remove = Math.Min(remaining, kvp.Value.Amount);
            remaining -= remove;
            var newStack = new ItemStack(data, kvp.Value.Amount - remove);
            if (newStack.IsEmpty)
                keysToUpdate.Add(kvp.Key);
            else
                _slots[kvp.Key] = newStack;
        }

        foreach (int key in keysToUpdate)
            _slots.Remove(key);

        int removed = amount - remaining;
        if (removed > 0)
            OnInventoryChanged?.Invoke();
        return removed;
    }

    /// <summary>Quita cantidad de un slot concreto. Devuelve la cantidad quitada.</summary>
    public int RemoveAt(int slotIndex, int amount = 1)
    {
        if (_slots == null || slotIndex < 0 || slotIndex >= slotCount || amount <= 0)
            return 0;
        if (!_slots.TryGetValue(slotIndex, out var stack))
            return 0;
        int remove = Math.Min(amount, stack.Amount);
        stack.Amount -= remove;
        if (stack.Amount <= 0)
            _slots.Remove(slotIndex);
        else
            _slots[slotIndex] = stack;
        if (remove > 0)
            OnInventoryChanged?.Invoke();
        return remove;
    }

    /// <summary>Intercambia el contenido de dos slots.</summary>
    public void SwapSlots(int indexA, int indexB)
    {
        if (_slots == null || indexA < 0 || indexA >= slotCount || indexB < 0 || indexB >= slotCount)
            return;
        _slots.TryGetValue(indexA, out var stackA);
        _slots.TryGetValue(indexB, out var stackB);

        if (stackA.IsEmpty && stackB.IsEmpty) return;

        if (stackB.IsEmpty)
            _slots.Remove(indexA);
        else
            _slots[indexA] = stackB;

        if (stackA.IsEmpty)
            _slots.Remove(indexB);
        else
            _slots[indexB] = stackA;

        OnInventoryChanged?.Invoke();
    }
}
