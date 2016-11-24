using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {

    //Type of pickup eg. Health, Shotgun, Machinegun, etc
    [SerializeField]
    private PickupType kindOfPickup;
    
    private enum PickupType
    {
        Health = 0,
        Shotgun = 1,
        GatlingGun = 2
    }

    //Value of type, eg. 50 health, 20 bullets
    [SerializeField]
    private int pickupValue;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (kindOfPickup == PickupType.Health)
            {
                other.gameObject.GetComponent<Player>().updateHealth(pickupValue);
            } else
            {
                //Give value to weapon
                other.gameObject.GetComponent<Player>().updateAmmo((int)kindOfPickup, pickupValue);
            }
            DestroyObject(this.gameObject);
        }
    }
}
