using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {        
        //Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
    }
	
	// Update is called once per frame
	void Update () {

	}
}
