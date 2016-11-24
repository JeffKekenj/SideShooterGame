using UnityEngine;
using System.Collections;
using System;

public class RangedState : IEnemyState
{
    private Enemy enemy;

    private float shootTimer;
    private float shootCoolDown;// = 3f;
    private bool canShoot = true;
    private bool isProne = false;
    private bool isDirectlyAbove = false;

    private Vector3 localShootPoint;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        //enemy.enemyShootPoint.position = new Vector3(enemy.shootPoint.position.x, -2.513724f, enemy.shootPoint.position.z);

        Debug.Log("Ranged State");
        localShootPoint = new Vector3(enemy.shootPoint.position.x, -2.513724f, enemy.shootPoint.position.z);
        //localShootPoint = new Vector3(enemy.shootPoint.position.x, enemy.shootPoint.position.y, enemy.shootPoint.position.z);
        shootCoolDown = UnityEngine.Random.Range(0.35f, 3f);
        //enemy.shootPoint               
        if (enemy.Target != null)
        {
            //First determine if player is above enemy's line of sight
            //eg. player.y > enemy shoot point.y
            if (!isTargetAbove())
            {                
                if (!canHitTarget())
                {
                    Debug.Log("Player is Prone");
                    goProne();
                }
                else
                {
                    Debug.Log("Player is Standing");
                    enemy.ResetShootPoint();
                    isProne = false;
                    enemy.myAnimator.SetBool("isProne", isProne);
                    enemy.isLookingUp = false;
                }   
                Shoot();          
            }            
            else
            {
                Debug.Log("IS ABOVE");
                //Until enemy move to target's position, do nothing
                if (CanShootAbove())
                {
                    enemy.SetUpwardShootPoint();
                    enemy.myAnimator.SetBool("isDirectlyAbove", isDirectlyAbove);
                    enemy.isLookingUp = true;
                    isDirectlyAbove = true;
                    Shoot();
                } else
                {
                    isDirectlyAbove = false;
                    enemy.isLookingUp = false;
                    enemy.ResetShootPoint();
                }
            }
            
        } else if (enemy.Target == null)
        {
            isDirectlyAbove = false;
            isProne = false;
            enemy.isLookingUp = false;
            enemy.ResetShootPoint();
        }

        //AND distance between enemy and player is greater than some value     
        if (!isProne)
        {
            if (!isDirectlyAbove && enemy.Target != null && ((Vector2.Distance(enemy.transform.position, enemy.Target.transform.position)) > 3f))
            {
                Debug.Log("enemy moving");
                enemy.Move();
            }
            else if (enemy.Target == null)
            {
                enemy.ChangeState(new IdleState());
            }
            else
            {
                if (!enemy.isLookingUp)
                {
                    enemy.ChangeState(new EscapingState());
                }
            }
        }
    }

    private void goProne()
    {
        //Set shoot point, change sprite, stop enemy from moving
        //enemy.Attack = true;
        Debug.Log("enemy is Prone");

        enemy.SetProneShootPoint();        
        isProne = true;
        enemy.myAnimator.SetFloat("enemySpeed", 0);
        enemy.myAnimator.SetBool("isProne", isProne);
    }

    private bool CanShootAbove()
    {
        return Mathf.Ceil(enemy.transform.position.x) == Mathf.Ceil(enemy.Target.transform.position.x);
    }

    private bool isTargetAbove()
    {
        return enemy.transform.position.y < enemy.Target.transform.position.y ? true : false;
    }

    //TODO: Depricated: Use IsFacingPlayer() now located in the Character class
    private bool canHitTarget()
    {
        Vector2 shootDir;
        if (enemy.facingRight)
        {
            shootDir = Vector2.right;
        } else
        {
            shootDir = Vector2.left;
        }
        Debug.DrawRay(localShootPoint, shootDir);
        return Physics2D.Raycast(localShootPoint, shootDir, Mathf.Infinity, enemy.layerMask);
    }

    public void Exit()
    {
        //throw new NotImplementedException();
    }

    public void OnTriggerEnter(Collider2D other)
    {
        
    }

    private void Shoot()
    {
        Debug.Log("Shoot");
        shootTimer += Time.deltaTime;
         
        if (shootTimer >= shootCoolDown)
        {
            canShoot = true;
            shootTimer = 0;
        }

        if (canShoot)
        {
            canShoot = false;

            if (isProne)
            {
                Debug.Log("Prone Attack");
                enemy.myAnimator.SetTrigger("enemyProneAttack");
            } else if (isDirectlyAbove)
            {
                Debug.Log("Enemy upward attack");
                enemy.myAnimator.SetTrigger("enemyUpwardAttack");
            }
            else
            {
                // set attack trigger and set bool in code enemy class
                Debug.Log("Normal Attack");
                enemy.myAnimator.SetTrigger("enemyAttack");
            }            
        }
    }
}
