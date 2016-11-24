using UnityEngine;
using System.Collections;

public class RailPlayer : RailCharacter
{
    [SerializeField]
    private Transform cursor;

    [SerializeField]
    protected GameObject bullet;

    void Start()
    {
        base.Start();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
        if (Input.GetAxisRaw("Fire1") > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //Instantiate bullet in direction of cursor
        //Debug.DrawLine(GetComponent<Transform>().position, cursor.position);
        GameObject tmp = (GameObject)Instantiate(bullet, GetComponent<Transform>().position, Quaternion.Euler(new Vector3(0, 0, -90)));
        Vector2 shootAngle = cursor.position - GetComponent<Transform>().position;
        tmp.GetComponent<Bullet>().Initialize(shootAngle);

        Debug.Log(shootAngle);
        if (Mathf.Abs(shootAngle.x) < 0.5f && Mathf.Abs(shootAngle.y) < 0.5f)
        {
            Debug.Log("here");
            DestroyObject(tmp);
        }

    }

}
