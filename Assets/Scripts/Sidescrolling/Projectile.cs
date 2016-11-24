using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    Rigidbody2D rb2d;

    [SerializeField]
    private float bulletSpeed;

    // Use this for initialization
    void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddForce(new Vector2(1, 0) * bulletSpeed, ForceMode2D.Impulse);
    }
}
