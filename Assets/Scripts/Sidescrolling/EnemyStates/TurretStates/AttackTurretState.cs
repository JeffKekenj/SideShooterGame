using UnityEngine;
using System.Collections;
using System;

public class AttackTurretState : ITurretState {

    private Turret turret;
    private float shootTimer;
    private float shootCoolDown;
    private bool canShoot = true;
    
    private float PlayerHeightSubTurrentHeight;

    void ITurretState.Enter(Turret turret)
    {
        this.turret = turret;
    }

    void ITurretState.Execute()
    {
        shootCoolDown = UnityEngine.Random.Range(1f, 2f);
        if (turret.Target != null)
        {
            //TurretBase turret.transform.position.y            
            //PlayerHeightSubTurrentHeight = Mathf.Floor(turret.Target.transform.position.y - turret.transform.Find("TurretBase").transform.position.y);
            PlayerHeightSubTurrentHeight = Mathf.Floor(turret.Target.transform.position.y - turret.transform.position.y);
            //if the  absolute vertical difference value is greater than 1, player is either above OR below the turret
            //Top and Bottom
            if (Mathf.Abs(PlayerHeightSubTurrentHeight) > 1)
            {
                if (PlayerHeightSubTurrentHeight > 0)
                {
                    //Above
                    Debug.Log("Above");
                    turret.SetUpwardShootPoint();
                    turret.isLookingUp = true;
                    turret.VerticalFlip(true);
                    //turret.isLookingUp = true;
                }
                else
                {
                    //Below
                    turret.SetDownwardShootPoint();
                    turret.VerticalFlip(false);
                    Debug.Log("Below");
                }
            } else //Left and Right
            {
                turret.ResetTurretRotation();
                turret.ResetShootPoint();
                turret.ResetShootDirectionBooleans();
                if (!turret.IsFacingPlayer())
                {
                   turret.Flip(); 
                } 
            }
            //Target found, shootpoint updated, now just shoot
            Shoot();
        } else
        {
            //Shoot for a bit before going back to idle
            //TODO: THAT CODE



            //Exit Attacking if no target found
            turret.ChangeState(new IdleTurretState());
        }
    }

    void ITurretState.Exit()
    {
        
    }

    void ITurretState.OnTriggerEnter(Collider2D other)
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
            
            Debug.Log("Normal Attack");
            turret.myAnimator.SetTrigger("enemyAttack");
        }
    }

}
