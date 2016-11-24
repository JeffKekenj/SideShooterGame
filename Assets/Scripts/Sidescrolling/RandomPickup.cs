using UnityEngine;
using System.Collections;

public class RandomPickup : MonoBehaviour   
{
    [SerializeField]
    private Sprite[] spriteList;

    private SpriteRenderer spriteRenderer;

    private PickupType kindOfPickup;
    private int pickupValue;

    private enum PickupType
    {
        Health = 0,
        Shotgun = 1,
        GatlingGun = 2
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pickups"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Pickups"), LayerMask.NameToLayer("Pickups"), true);

        //Selects random value
        kindOfPickup = GetRandomEnum<PickupType>();
        pickupValue = Random.Range(10, 20);

        //Based on type, select correct sprite
        spriteRenderer.sprite = spriteList[(int)kindOfPickup];
    }

    //http://answers.unity3d.com/questions/56429/enum-choose-random-.html
    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (kindOfPickup == PickupType.Health)
            {
                other.gameObject.GetComponent<Player>().updateHealth(pickupValue);
            }
            else
            {
                //Give value to weapon
                other.gameObject.GetComponent<Player>().updateAmmo((int)kindOfPickup, pickupValue);
            }
            DestroyObject(this.gameObject);
        }
    }
}
