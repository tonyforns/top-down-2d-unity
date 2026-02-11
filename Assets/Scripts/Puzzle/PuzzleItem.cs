using System;
using UnityEngine;

public class PuzzleItem : MonoBehaviour, IInteractable
{
    public Action OnStateChange;

    [SerializeField] private bool correctState;
    [SerializeField] private bool currentState = false;
    public virtual void Interact()
    {
        currentState = !currentState;
        OnStateChange?.Invoke();
    }
    public bool IsInCorrectState()
    {
        return currentState == correctState;
    }
}
