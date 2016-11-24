using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {

    public static GameMaster gm;

    private AudioManager audioManager;

    [SerializeField]
    private Transform PlayerSpawnPoint;

    [SerializeField]
    private Transform player;

    void Awake()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        }        
    }
        
    // Use this for initialization
    void Start () {

        //caching
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("NO Audo Manager in scene");
        }
    }
	
	// Update is called once per frame
	void Update () {
	    //If player is null, instantiate player
        
	}
}
