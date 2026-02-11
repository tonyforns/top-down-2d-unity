using System.Collections;
using UnityEngine;

public class PuzzleDoor : Puzzle
{
    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private GameObject doorUp;
    [SerializeField] private GameObject doorDown;
    [SerializeField] private float openDistance = 1f;
    [SerializeField] private float openDuration = 1f;

    internal override void PuzzleSolved()
    {
        base.PuzzleSolved();
        doorCollider.enabled = false;
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        SoundSystem.Instance.PlaySound(SoundModelSO.SoundName.DoorOpen, transform.position);
        Vector3 doorUpStart = doorUp.transform.position;
        Vector3 doorDownStart = doorDown.transform.position;
        Vector3 doorUpEnd = doorUpStart + Vector3.up * openDistance;
        Vector3 doorDownEnd = doorDownStart + Vector3.down * openDistance;

        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / openDuration;

            doorUp.transform.position = Vector3.Lerp(doorUpStart, doorUpEnd, t);
            doorDown.transform.position = Vector3.Lerp(doorDownStart, doorDownEnd, t);

            yield return null;
        }

        doorUp.transform.position = doorUpEnd;
        doorDown.transform.position = doorDownEnd;
        Destroy(this);
    }
}
