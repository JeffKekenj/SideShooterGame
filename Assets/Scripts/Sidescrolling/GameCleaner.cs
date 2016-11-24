using UnityEngine;
using System.Collections;

public class GameCleaner : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        //Destroy(other.gameObject);
        //other.GetComponent<Character>().Death();
        Destroy(other.gameObject);
    }
}
