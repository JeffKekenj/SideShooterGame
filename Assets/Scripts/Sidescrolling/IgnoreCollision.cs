using UnityEngine;
using System.Collections;

public class IgnoreCollision : MonoBehaviour {
    
    [SerializeField]
    private Player other;

	// Use this for initialization
	private void Awake () {
        //Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.GetComponent<Collider2D>(), true);
	}
}
