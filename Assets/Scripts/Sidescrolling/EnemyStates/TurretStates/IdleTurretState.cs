using UnityEngine;
using System.Collections;
using System;

public class IdleTurretState : ITurretState {

    private Turret turret;

    private float idleTimer;

    private float idleDuration = 5f;
    
    void ITurretState.Enter(Turret turret)
    {
        this.turret = turret;
    }

    void ITurretState.Execute()
    {
        //Detemine if player is above or below
        if (turret.Target != null)
        {
            turret.ChangeState(new AttackTurretState());
        }

        /*Debug.Log("Turret Idle");
        //Idle();
        if (turret.Target != null)
        {
            if (!turret.IsFacingPlayer())
            {
                turret.Flip();
            } else
            {
                turret.ChangeState(new AttackTurretState());
            }            
        }*/
    }

    void ITurretState.Exit()
    {
    }

    void ITurretState.OnTriggerEnter(Collider2D other)
    {
        if (other.tag == "Bullet")
        {
            turret.Flip();
            turret.ChangeState(new AttackTurretState());
        }
    }

    void Idle()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            turret.Flip();            
        }
    }
}
