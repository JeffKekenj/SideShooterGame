using UnityEngine;
using System.Collections;
using System;

public class Enemy : Character {

    [SerializeField]
    private bool hasDrop;

    [SerializeField]
    private GameObject enemyDrop;

    private IEnemyState currentState;
    
    public bool Attack { get; set; }

    public bool isEscaping { get; set; }

    public Transform enemyShootPoint;

    public override bool isDead
    {
        get
        {
            return health <= 0;
        }
    }

    // Use this for initialization
    public override void Start () {
        base.Start();
        ChangeState(new IdleState());
        Attack = false;
        TakingDamage = false;
        isEscaping = false;
        enemyShootPoint = shootPoint;   
    }

    // Update is called once per frame
    void Update () {
        if (!isDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }
            if (!isEscaping)
            {                
                LookAtTarget();
            }
        }
	}

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        //give enemy 
        currentState.Enter(this);
    }

    public void Move()
    {
        //if not attacking
        if (!Attack && !TakingDamage)
        {
            myAnimator.SetFloat("enemySpeed", 1);
            transform.Translate((GetDirection() * movementSpeed * Time.deltaTime), Space.World);
        }
    }

    public Vector2 GetDirection()
    {
        return facingRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Physics2D.IgnoreCollision(other.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && facingRight || xDir >0 && !facingRight)
            {
                Flip();
            }
        }
    }

    public override IEnumerator TakeDamage(int damage)
    {
        health -= damage;

        if (!isDead)
        {            
            myAnimator.SetTrigger("enemyHit");
        }
        else
        {
            myAnimator.SetTrigger("enemyDeath");
            yield return null;
        }
    }

    public bool isfacingRight()
    {
        return facingRight;
    }


    public override void Death()
    {
        //Drop ammo/health, etc
        if (hasDrop)
        {
            Instantiate(enemyDrop, transform.position, Quaternion.identity);
        }
        
        if (this != null)
        {
            Destroy(gameObject);
        }        
        //do animator stuff
        //myAnimator.SetTrigger   
        //myAnimator.ResetTrigger("enemyDeath");  
        //transform.position = enemySpawnpoint.position;
        //Instantiate(this, enemySpawnpoint.position, Quaternion.identity);
    }
}
