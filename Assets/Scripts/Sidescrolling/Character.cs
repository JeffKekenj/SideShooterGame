using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Character : MonoBehaviour {

    public Animator myAnimator { get; private set; }

    [SerializeField]
    public Transform shootPoint;

    [SerializeField]
    protected GameObject bullet;

    [SerializeField]
    protected float movementSpeed;

    public Vector3 originalShootPoint;

    //layer mask of what to hit
    public LayerMask layerMask;

    public GameObject Target { get; set; }

    //protected bool facingRight;
    public bool facingRight { get; set; }

    //protected bool isLookingUp = false;

    public bool isLookingUp { get; set; }

    public bool isLookingDown { get; set; }

    [SerializeField]
    public int health;

    protected int maxHealth = 100;

    [SerializeField]
    private List<string> damageSources;

    public bool TakingDamage { get; set; }

    public abstract bool isDead { get; }
    
    // Use this for initialization
    public virtual void Start () {
        facingRight = true;
        myAnimator = GetComponent<Animator>();
        originalShootPoint = shootPoint.localPosition;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public abstract IEnumerator TakeDamage(int damage);

    public abstract void Death();

    public void ChangeDirection(float horizontal)
    {
        if (facingRight && horizontal < 0 || !facingRight && horizontal > 0)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
    }    

    public virtual void Shoot()
    {
        Debug.Log("Character Shoot");
        if (isLookingUp)
        {
            Debug.Log("Looking UP");
            GameObject tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.up);
        } else if (isLookingDown)
        {
            Debug.Log("Looking Down");
            GameObject tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.down);
        }
        else if (facingRight)
        {
            Debug.Log("Facing Right");
            GameObject tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.right);

        }
        else
        {
            Debug.Log("Facing LEFT");
            GameObject tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, 90)));
            tmp.GetComponent<Bullet>().Initialize(Vector2.left);
        }
    }

    public void ResetShootDirectionBooleans()
    {
        isLookingUp = false;
        isLookingDown = false;
    }
    
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            
            if (!isDead)
            {
                DestroyObject(other.gameObject);
            }
            StartCoroutine(TakeDamage(other.GetComponent<Bullet>().bulletDamage));            
        }
    }

    public void SetProneShootPoint()
    {
        shootPoint.localPosition = new Vector2(0.717f, -0.322f);
    }

    public void SetUpwardShootPoint()
    {
        //Make sure this dosen't fuck up any other code
        isLookingUp = true;
        shootPoint.localPosition = new Vector2(-0.004f, 1.219f);
        shootPoint.localRotation = new Quaternion(shootPoint.localRotation.x, shootPoint.localRotation.y, 90f, shootPoint.localRotation.w);
    }

    public void SetDownwardShootPoint()
    {
        isLookingDown = true;
        shootPoint.localPosition = new Vector2(-0.004f, -1.219f);
        shootPoint.localRotation = new Quaternion(shootPoint.localRotation.x, shootPoint.localRotation.y, 90f, shootPoint.localRotation.w);
    }

    public void ResetShootPoint()
    {
        shootPoint.localPosition = originalShootPoint;
    }

    public bool IsFacingPlayer()
    {
        Vector2 shootDir;
        if (facingRight)
        {
            shootDir = Vector2.right;
        }
        else
        {
            shootDir = Vector2.left;
        }
        Debug.DrawRay(shootPoint.position, shootDir);        
        Debug.Log("Raycast " + Physics2D.Raycast(shootPoint.position, shootDir));
        //return Physics2D.Raycast(shootPoint.position, shootDir);
        //return Physics2D.CircleCast(shootPoint.position, shootDir, Mathf.Infinity, layerMask);
        return Physics2D.CircleCast(shootPoint.position, 1f, shootDir, Mathf.Infinity, layerMask);
    }
}
