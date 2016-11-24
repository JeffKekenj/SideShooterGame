using UnityEngine;
using System.Collections;
using System;

public class EscapingState : IEnemyState
{
    private Enemy enemy;
    private float runDuration;
    private float timeToStopRunning;
    private Transform lastPlayerPosition;
        
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        runDuration = UnityEngine.Random.Range(0.2f, 2f);
        timeToStopRunning = Time.time + runDuration;
        enemy.isEscaping = true;        
     }

    public void Execute()
    {
        Debug.Log("Escape State");
        if (enemy.Target != null)
        {
            lastPlayerPosition = enemy.Target.transform;
        }     
        Escape();
    }

    private void Escape()
    {
        if (Time.time < timeToStopRunning)
        {
            if (enemy.Target == null && !((Vector2.Distance(enemy.transform.position, lastPlayerPosition.position)) <= 3f))
            {
                enemy.Flip();
            } 
            else
            {
                enemy.Move();       
            }
        } else
        {
            enemy.Flip();
            enemy.ChangeState(new IdleState());
        }
    }

    public void Exit()
    {
        enemy.isEscaping = false;
    }

    public void OnTriggerEnter(Collider2D other)
    {
        //throw new NotImplementedException();
    }

}
