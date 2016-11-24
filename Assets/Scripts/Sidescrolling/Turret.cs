using UnityEngine;
using System.Collections;
using System;

public class Turret : Character {

    [SerializeField]
    private bool hasDrop;

    [SerializeField]
    private GameObject enemyDrop;

    private ITurretState currentState;

    private Transform enemyShootPoint;

    // Use this for initialization
    public override void Start () {
        base.Start();
        ChangeState(new IdleTurretState());
        enemyShootPoint = shootPoint;
    }

     // Update is called once per frame
     void Update () {
        if (!isDead)
        {
            currentState.Execute();
        }
    }

    public override bool isDead
    {
        get
        {
            return health <= 0;
        }
    }

    public void ChangeState(ITurretState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        //give enemy 
        currentState.Enter(this);
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

    public override void Death()
    {
        Debug.Log("Turret Death");
        Destroy(gameObject);
    }

    public void VerticalFlip(bool UpRotation)
    {
        Vector3 RotationDirectionVector;
        if (facingRight)
        {
            if (UpRotation)
            {
                RotationDirectionVector = new Vector3(0, 0, 90);
            } else
            {
                RotationDirectionVector = new Vector3(0, 0, 270);
            }            
        } else
        {
            if (UpRotation)
            {
                RotationDirectionVector = new Vector3(0, 0, -90);
            } else
            {
                RotationDirectionVector = new Vector3(0, 0, -270);
            }            
        }
        transform.Find("TurretBarrel").eulerAngles = RotationDirectionVector;
    }

    public void DownFlip()
    {
        if (facingRight)
        {
            
        }
        else
        {
            
        }
    }

    public void ResetTurretRotation()
    {
        transform.Find("TurretBarrel").eulerAngles = new Vector3(0, 0, 0);
    }
}
