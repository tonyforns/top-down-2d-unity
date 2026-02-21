using System.Collections;
using UnityEngine;

public class PlayerToolSwing : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Transform toolPivot;
    [SerializeField] private SpriteRenderer toolRenderer;

    [Header("Tool Sprites")]
    [SerializeField] private Sprite pickaxeSprite;
    [SerializeField] private Sprite axeSprite;

    [Header("Swing")]
    [SerializeField] private float swingAngle = 80f;
    [SerializeField] private float swingTime = 0.10f;

    [Header("Sorting (optional)")]
    [SerializeField] private bool autoSort = true;
    [SerializeField] private int toolOrderFront = 20;
    [SerializeField] private int toolOrderBack = 10;

    private Coroutine swingRoutine;

    private void Awake()
    {
        if (toolRenderer != null)
            toolRenderer.enabled = false;
    }

    public void PlayPickaxe(Vector2 facing4) => PlayTool(pickaxeSprite, facing4);
    public void PlayAxe(Vector2 facing4) => PlayTool(axeSprite, facing4);

    private void PlayTool(Sprite sprite, Vector2 facing4)
    {
        if (toolPivot == null || toolRenderer == null || sprite == null) return;

        toolRenderer.sprite = sprite;
        toolRenderer.enabled = true;

        float baseAngle = FacingToAngle(facing4);
        toolPivot.localRotation = Quaternion.Euler(0, 0, baseAngle);

        if (autoSort)
        {
            toolRenderer.sortingOrder = (facing4.y > 0) ? toolOrderBack : toolOrderFront;
        }

        if (swingRoutine != null) StopCoroutine(swingRoutine);
        swingRoutine = StartCoroutine(Swing(baseAngle));
    }

    private IEnumerator Swing(float baseAngle)
    {
        float t = 0f;
        float start = baseAngle - swingAngle * 0.5f;
        float end = baseAngle + swingAngle * 0.5f;

        while (t < swingTime)
        {
            t += Time.deltaTime;
            float k = t / swingTime;
            float a = Mathf.Lerp(start, end, k);
            toolPivot.localRotation = Quaternion.Euler(0, 0, a);
            yield return null;
        }

        toolPivot.localRotation = Quaternion.Euler(0, 0, baseAngle);
        toolRenderer.enabled = false;
        swingRoutine = null;
    }

    private float FacingToAngle(Vector2 f)
    {
        if (f.x > 0) return 0f;     
        if (f.x < 0) return 180f;   
        if (f.y > 0) return 90f;    
        return -90f;                
    }
}