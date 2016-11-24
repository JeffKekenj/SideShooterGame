using UnityEngine;
using System.Collections;

public class RailCharacter : MonoBehaviour {

    private Rigidbody2D rb2d;

    [SerializeField]
    private float movementSpeed;

    // Use this for initialization
    public virtual void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        HandleMovement(horizontal, vertical);
    }

    void HandleMovement(float horizontal, float vertical)
    {
        rb2d.velocity = new Vector2(horizontal * movementSpeed, vertical * movementSpeed);
    }
}
