using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

    [SerializeField]
    private float bulletSpeed;

    public int bulletDamage;
    //public int bulletDamage { get; set; }

    private Rigidbody2D rb2d;

    private Vector2 direction;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        rb2d.velocity = direction * bulletSpeed;
	}

    public void Initialize(Vector2 direction)
    {
        this.direction = direction;
    }


    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
