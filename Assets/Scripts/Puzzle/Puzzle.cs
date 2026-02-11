using System;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public Action OnPuzzleSolved;
    [SerializeField] private PuzzleItem[] puzzleItems;

    private void Start()
    {
        foreach (var item in puzzleItems)
        {
            item.OnStateChange += CheckPuzzleSolved;
        }
    }
    private void OnDestroy()
    {
        foreach (var item in puzzleItems)
        {
            item.OnStateChange -= CheckPuzzleSolved;
        }
    }
    private void CheckPuzzleSolved()
    {
        bool result = true;
        foreach (var item in puzzleItems)
        {
            result = result && item.IsInCorrectState();
        }

        if(result)
        {
            PuzzleSolved();
        }
    }

    internal virtual void PuzzleSolved()
    {
        OnPuzzleSolved?.Invoke();
    }
}
