using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Player : Character {

    //Handle Camera Shaking
    public float camShakeAmt = 0.1f;

    public Image damageScreen;

    bool damaged = false;
    Color damagedColor = new Color(0, 0, 0, 0.5f);
    float smoothColor = 5f;

    private AudioManager audioManager;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    GameObject mainCam;
    CameraShake camShake;

    [SerializeField]
    private Stat healthStat;

    [SerializeField]
    private Stat ammoStat;

    Rigidbody2D rb2d;
    
    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private Transform groundPoint;
        
    [SerializeField]
    private float groundRadius;

    [SerializeField]
    private LayerMask whatIsGround;

    [SerializeField]
    private LayerMask whatIsWall;

    //Wall Sliding
    private bool wallSliding = false;
    private float wallSlideSpeedMax = 3f;

    [SerializeField]
    private Transform wallPoint;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    int wallDirX;

    public float wallStickTime = .25f;
    float timeToWallUnstick;
       
    //SLague playerInput for camera smoothing
    [HideInInspector]
    public Vector2 playerInput;
        
    private bool isGrounded = false;
    private bool isProne = false;

    private BoxCollider2D bxCollider;

    private Vector2 playerBoxColliderSize;
    private Vector2 playerBoxColliderOffset;

    private FireMode weaponFire;

    FireMode simpleGun;
    FireMode shotgun;
    FireMode gattlingGun;


    [SerializeField]
    private float valueToDelete;

    //WeaponSystem
    //https://www.davevoyles.com/2014/12/16/creating-inventory-system-unity-using-c/

    // Holds the weapons in our inventory
    private Boolean[] weaponInventory;
    // Which weapon is the player currently using?
    private int currentWeaponIndex = 0;

    public enum BulletPresetType
    {
        Simple = 0,
        Shotgun = 1,
        GatlingGun = 2
    }

    private class FireMode
    {
        public int damage;
        public float timeBetweenShots;
        public int ammo;
        public BulletPresetType weapon;
        public float weaponShakeAmt;
        public string weaponSound;
        public int maxAmmo = 100;

        public FireMode(int damage, float timeBetweenShots, int ammo, BulletPresetType weapon, float weaponShakeAmt, string sound)
        {
            this.damage = damage;
            this.timeBetweenShots = timeBetweenShots;
            this.ammo = ammo;
            this.weapon = weapon;
            this.weaponShakeAmt = weaponShakeAmt;
            this.weaponSound = sound;
        }
    }

    public void updateAmmo(int TypeofPickup, int Value)
    {
        BulletTypeChanger(TypeofPickup);
        weaponFire.ammo += Value;

        if (weaponFire.ammo >= weaponFire.maxAmmo)
        {
            weaponFire.ammo = weaponFire.maxAmmo;
        }
        //reset to current weapon
        BulletTypeChanger(currentWeaponIndex);
        setAmmoStat();
    }

    public void updateHealth(int value)
    {
        health += value;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        healthStatUpdate();
    }


    private void BulletTypeChanger(int currentWeapon)
    {
        switch (currentWeapon)
        {
            case (int)BulletPresetType.Simple:
                weaponFire = simpleGun;                      
                break;
            case (int)BulletPresetType.Shotgun:
                weaponFire = shotgun;
                break;
            case (int)BulletPresetType.GatlingGun:
                weaponFire = gattlingGun;
                break;
        }
    }

    //Shooting Vars
    private float counterToShootTime;

    public override bool isDead
    {
        get
        {
            return health <= 0;
        }
    }

    // Use this for initialization
    public override void Start () {
        //base.StartCoroutine();
        base.Start();

        if (mainCam == null)
        {
            mainCam = GameObject.FindGameObjectWithTag("Camera");
        }

        camShake = mainCam.GetComponent<CameraShake>();

        if (camShake == null)
        {
            print("No Camera shake ");
        }

        rb2d = GetComponent<Rigidbody2D>();

        bxCollider = GetComponent<BoxCollider2D>();

        playerBoxColliderSize = bxCollider.size;
        playerBoxColliderOffset = bxCollider.offset;
        
        counterToShootTime = Time.time;

        weaponInventory = new bool[Enum.GetValues(typeof(BulletPresetType)).Length];
        //weaponInventory[0] = true; // The first item in the BulletPresetType, is currently in my inventory
        weaponInventory[(int)BulletPresetType.Simple] = true;
        weaponInventory[(int)BulletPresetType.Shotgun] = true;
        weaponInventory[(int)BulletPresetType.GatlingGun] = true;
               
        /*weaponFire = new FireMode();
        BulletTypeChanger(currentWeaponIndex);*/

        simpleGun = new FireMode(10, 0.3f, 1, BulletPresetType.Simple, 0.01f, "simple");
        shotgun = new FireMode(20, 1f, 50, BulletPresetType.Shotgun, 0.1f, "shotgun");
        gattlingGun = new FireMode(5, 0.1f, 50, BulletPresetType.GatlingGun, 0.05f, "gattling");
                  
        //initialize weapons
        foreach (BulletPresetType weapon in Enum.GetValues(typeof(BulletPresetType)))
        {
            BulletTypeChanger((int)weapon);
        }

        weaponFire.ammo += 50;

        //Always start with Simple
        BulletTypeChanger(currentWeaponIndex);

        //TODO: Move this up to Enemy
        healthStat.Initialize();
        ammoStat.Initialize();
        setAmmoStat();

        //Setup Audio Manager
        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No Audio Manager found!");
        }
    }

    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");

        if (!isDead)
        {
            HandleInput(vertical);
        }       

        if (damaged)
        {
            damageScreen.color = damagedColor;
        } else
        {
            damageScreen.color = Color.Lerp(damageScreen.color, Color.clear, smoothColor * Time.deltaTime);
        }
        damaged = false;


        //WallJump();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isDead)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");            

            isGrounded = IsGrounded();
            //Check to see if needed
            /*setGrounded(isGrounded);
            myAnimator.SetFloat("verticalSpeed", rb2d.velocity.y);*/

            HandleMovement(horizontal);
            ChangeDirection(horizontal);
            CheckJump();
            WallJump();
        }
    }

    void WallJump()
    {
        //Wall Jump stuff
        if ((Input.GetButton("Left") || Input.GetButton("Right")) && IsWall() && rb2d.velocity.y < 0)
        {
            if (Input.GetButton("Left"))
            {
                wallDirX = -1;
            }
            else if (Input.GetButton("Right"))
            {
                wallDirX = 1;
            }
            wallSliding = true;
            if (rb2d.velocity.y < -wallSlideSpeedMax)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, -wallSlideSpeedMax);
            }

            if (timeToWallUnstick > 0)
            {
                rb2d.velocity = new Vector2(Vector2.zero.x, rb2d.velocity.y);
                if ((Input.GetButton("Right") && wallDirX != 1) || (Input.GetButton("Left") && wallDirX != -1))
                {
                    if (Input.GetButton("Right") || Input.GetButtonDown("Left"))
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
        else
        {
            wallSliding = false;
        }
    }


    //if is grounded, reset boxcollider
    void ResetCollider()
    {
        if (isGrounded && !Input.GetKeyDown(KeyCode.Space))
        {
            bxCollider.size = playerBoxColliderSize;
            bxCollider.offset = playerBoxColliderOffset;
        } 
    }

    void CheckJump()
    {
        //check if we are grounded, if no, then we are falling
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundRadius, whatIsGround);
        setGrounded(isGrounded);
        myAnimator.SetFloat("verticalSpeed", rb2d.velocity.y);
    }

    void ChangeBoxCollider(float offsetX, float offsetY, float sizeX, float sizeY)
    {
        bxCollider.offset = new Vector2(offsetX, offsetY);
        bxCollider.size = new Vector2(sizeX, sizeY);
    }

    void HandleInput(float vertical)
    {
        //if (Input.GetAxis("Jump") > 0) //if (Input.GetKeyDown(KeyCode.Space))
        //if (Input.GetKeyDown(KeyCode.Space))
        if ((wallSliding || isGrounded) && !isProne && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)))
        {
            if (wallSliding)
            {
                if (facingRight && Input.GetButton("Right"))
                {
                    rb2d.velocity = new Vector2(-wallDirX * wallJumpClimb.x, wallJumpClimb.y);
                } else if (!facingRight && !Input.GetButton("Left"))
                {
                    rb2d.velocity = new Vector2(-wallDirX * wallJumpClimb.x, wallJumpClimb.y);
                }
                else
                {
                    rb2d.velocity = new Vector2(-wallDirX * wallLeap.x, wallLeap.y);
                }
            }

            isGrounded = false;
            isProne = false;
            isLookingUp = false;

            setProneAnimator(isProne);
            setGrounded(isGrounded);
            setIsLookingUp(isLookingUp);
            ResetShootPoint();

            //remove any initial force before adding new force
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, jumpForce));
            
            //Change collider size
            ChangeBoxCollider(0f, 0f, 0.5f, 0.5f);
        }

        //set player animator to prone, isprone to true
        //stop player from moving
        //Change shoot point
        if (isGrounded && !isProne && (Input.GetKeyDown(KeyCode.S) || vertical < 0)) 
        {
            isProne = true;

            //shootPoint.localPosition = new Vector2(0.717f, -0.322f);
            SetProneShootPoint();

            ChangeBoxCollider(0.04f, -0.38f, 1.2f, 0.57f);
            rb2d.velocity = Vector2.zero;
            setProneAnimator(isProne);
        }

        //reset prone value and shoot position
        if ((Input.GetKeyUp(KeyCode.S) || vertical == 0) && isProne)
        {
            isProne = false;            
            
            ResetShootPoint();
            ResetCollider();
            setProneAnimator(isProne);
        }


        if ((Input.GetKeyDown(KeyCode.W) || vertical > 0) && !isLookingUp && isGrounded)
        {
            isLookingUp = true;
            SetUpwardShootPoint();
            rb2d.velocity = Vector2.zero;
            setIsLookingUp(isLookingUp);
        }

        if ((Input.GetKeyUp(KeyCode.W) || vertical == 0) && isLookingUp)
        {
            isLookingUp = false;
            setIsLookingUp(isLookingUp);            
            ResetShootPoint();        
        }

        //Allows player to either hold fire key down at a slower constant shoot rate,  
        //or mash the fire key to fire as fast as the key is hit
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Joystick1Button0)) || ((Input.GetAxisRaw("Fire1") > 0) && (counterToShootTime < Time.time)))
        {
            if (weaponFire.ammo > 0)
            {
                //deduct ammo if not standard weapon
                if (currentWeaponIndex != (int)BulletPresetType.Simple)
                {
                    weaponFire.ammo--;
                }
                counterToShootTime = weaponFire.timeBetweenShots + Time.time;
                Shoot();
                setAmmoStat();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            NextWeapon();
            BulletTypeChanger(currentWeaponIndex);
            setAmmoStat();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
           StartCoroutine(TakeDamage(100));
        }
    }



    private void setAmmoStat()
    {
        ammoStat.CurrentVal = weaponFire.ammo;
    }

    public override void Shoot()
    {
        //Shake the cam
        camShake.Shake(weaponFire.weaponShakeAmt, 0.2f);

        //Play shoot sound
        audioManager.PlaySound(weaponFire.weaponSound);

        GameObject tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
        tmp.GetComponent<Bullet>().bulletDamage = weaponFire.damage;
        Vector2.Angle(Vector2.up, Vector2.right);
        if (isLookingUp)
        {            
            if (currentWeaponIndex == (int)BulletPresetType.Shotgun)
            {
                ShotgunBehaviour(tmp, 2);
            }
            else
            {
                tmp.GetComponent<Bullet>().Initialize(Vector2.up);
            }
        }
        else if (facingRight)
        {
            /*if (currentWeaponIndex == (int)BulletPresetType.Shotgun)
            {
                tmp.GetComponent<Bullet>().Initialize(new Vector2(1, 0.1f));
                GameObject tmp2 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
                tmp2.GetComponent<Bullet>().Initialize(Vector2.right);
                GameObject tmp3 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
                tmp3.GetComponent<Bullet>().Initialize(new Vector2(1, -0.1f));
            } else
            {
                tmp.GetComponent<Bullet>().Initialize(Vector2.right);
            }*/

            if (currentWeaponIndex == (int)BulletPresetType.Shotgun)
            {
                ShotgunBehaviour(tmp, 1);
            } else
            {
                tmp.GetComponent<Bullet>().Initialize(Vector2.right);
            }
        }
        else
        {
            //Note: Might cause problems when instantiating non-symetric bullet
            /*tmp = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, 90)));
            tmp.GetComponent<Bullet>().bulletDamage = weaponFire.damage;*/
              
            if (currentWeaponIndex == (int)BulletPresetType.Shotgun)
            {
                ShotgunBehaviour(tmp, -1);
            } else
            { 
                tmp.GetComponent<Bullet>().Initialize(Vector2.left);
            }
        }
    }

    //passing in int to change direction of fire
    private void ShotgunBehaviour(GameObject tmp, int direction)
    {
        if (direction < 2)
        {
            tmp.GetComponent<Bullet>().Initialize(new Vector2(1, 0.1f) * direction);
            GameObject tmp2 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp2.GetComponent<Bullet>().Initialize(Vector2.right * direction);
            GameObject tmp3 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp3.GetComponent<Bullet>().Initialize(new Vector2(1, -0.1f) * direction);
        } else
        {
            tmp.GetComponent<Bullet>().Initialize(new Vector2(0.1f, 1f));
            GameObject tmp2 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp2.GetComponent<Bullet>().Initialize(Vector2.up);
            GameObject tmp3 = (GameObject)Instantiate(bullet, shootPoint.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp3.GetComponent<Bullet>().Initialize(new Vector2(-0.1f, 1f));
        }
    }

    private void NextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weaponInventory.Length;
    }
    
    void setIsLookingUp(bool lookup)
    {
        myAnimator.SetBool("isLookingUp", lookup);
    }

    void setProneAnimator(bool isProne)
    {
        myAnimator.SetBool("isProne", isProne);
    }

    void setGrounded(bool isGrounded)
    {
        myAnimator.SetBool("isGrounded", isGrounded);
    }

    void HandleMovement(float horizontal)
    {
        if (!isProne)
        {
            rb2d.velocity = new Vector2(horizontal * movementSpeed, rb2d.velocity.y);
            myAnimator.SetFloat("speed", Mathf.Abs(rb2d.velocity.x));
        }

        playerInput = rb2d.velocity;
        //if (rb2d.velocity.y < 0)
        if(rb2d.velocity.y < -0.1 && !isProne)
        { 
            myAnimator.SetBool("land", true);
            ResetCollider();
        }
    }

    private bool IsWall()
    {
        return Physics2D.OverlapCircle(wallPoint.position, 0.5f, whatIsWall);
    }

    //TODO: Need to check that animation is not in jump animation additionally
    private bool IsGrounded()
    {
        //return Physics2D.OverlapCircle(groundPoint.position, groundRadius, whatIsGround);
        //if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Land") && Physics2D.OverlapCircle(groundPoint.position, groundRadius, whatIsGround))
        if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("jumpBlendTree") && Physics2D.OverlapCircle(groundPoint.position, groundRadius, whatIsGround))
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        } else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }

    private void healthStatUpdate()
    {
        healthStat.CurrentVal = health;
    }

    public override IEnumerator TakeDamage(int damage)
    {
        Debug.Log("Take damage " + damage);
        //TODO: Fix this damn mess
        if (!isDead)
        {
            health -= damage;
            healthStatUpdate();

            damaged = true;
        }


        if (!isDead)
        {
            myAnimator.SetTrigger("Hit");
        } else
        {
            rb2d.velocity = Vector2.zero;
            myAnimator.SetTrigger("Death");
            yield return null;
        }
    }

    public override void Death()
    {
        //Destroy(gameObject);
        //myAnimator.ResetTrigger("Death");
        //Respawn Player
        //GameMaster.gm.RespawnPlayer();  

        Debug.Log("Death called");
        
        health = maxHealth;
        myAnimator.ResetTrigger("Death");
        myAnimator.SetTrigger("idle");        
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = spawnPoint.position;
        
    }
}
