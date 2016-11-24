using UnityEngine;
using System.Collections;
using System;

public class PatrolState : IEnemyState
{

    private Enemy enemy;

    private float patrolTimer;

    private float patrolDuration = 5f;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        Debug.Log("Patrol State");
        Patrol();
        enemy.Move();

        if (enemy.Target != null)
        {
            enemy.ChangeState(new RangedState());
        }
    }

    public void Exit()
    {       
    }

    public void OnTriggerEnter(Collider2D other)
    {
        if (other.tag == "Edge")
        {
            enemy.Flip();
        }
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (patrolTimer >= patrolDuration)
        {
            enemy.ChangeState(new IdleState());
        }
    }

    public void Enter(Character enemy)
    {
        throw new NotImplementedException();
    }
}
