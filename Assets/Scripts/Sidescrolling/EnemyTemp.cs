using UnityEngine;
using System.Collections;

public class EnemyTemp : MonoBehaviour {

    [SerializeField]
    private int health;

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;

    float counterTime;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            TakeDamage(other.GetComponent<Bullet>().bulletDamage);
            DestroyObject(other.gameObject);
        }
    }

    void TakeDamage(int damageValue)
    {
        health = health - damageValue;
        myAnimator.SetTrigger("enemyHit");
        if (health <= 0)
        {
            EnemyDeath();
        }
    }

    void EnemyDeath()
    {
        myAnimator.SetTrigger("enemyDeath");
        counterTime = Time.time + 2f;
        StartCoroutine(Immortal());


    }

    private IEnumerator Immortal()
    {
        while (counterTime > Time.time)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
        Destroy(gameObject);
    }
}
