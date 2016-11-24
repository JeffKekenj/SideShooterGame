using UnityEngine;
using System.Collections;

public class Boundary : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            print("Player is in here");
        }
    }
}
