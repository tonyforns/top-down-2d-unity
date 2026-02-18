using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    [SerializeField] float linearSpeed = 1f;
    Rigidbody2D rb2D;

    protected virtual void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        
    }

    protected void Move(Vector2 direction)
    {
        rb2D.position += direction * linearSpeed * Time.deltaTime;
    }
}
