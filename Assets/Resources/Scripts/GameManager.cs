using System.Collections;
using System.Collections.Generic;
using Assets.Resources.Scripts;
using UnityEngine;

public sealed class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    _player = new GameObject().AddComponent<Player>();
    }

    private GameManager()
    {
        
    }

    private static volatile GameManager instance;

    public static GameManager Instance()
    {
        if (instance == null)
            instance = FindObjectOfType<GameManager>();
        return instance;
    }


    private Player _player;
    public Player Player()
    {
        return _player;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
