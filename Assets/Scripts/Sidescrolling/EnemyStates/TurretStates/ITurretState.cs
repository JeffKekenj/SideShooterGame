using UnityEngine;

public interface ITurretState
{
    void Execute();
    void Enter(Turret turret);
    void Exit();
    void OnTriggerEnter(Collider2D other);
}
