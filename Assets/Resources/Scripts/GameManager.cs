using System.Collections;
using System.Collections.Generic;
using Assets.Resources.Scripts;
using UnityEngine;

public sealed class GameManager : MonoBehaviour
{

    void Start()
    {
        _player = new GameObject().AddComponent<Player>();

        for (int i = 0; i < 10; i++)
            AddAsteroid(null);

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

    public void AddAsteroid(AsteroidType? type)
    {
        var asteroid = new GameObject().AddComponent<Asteroid>();
        asteroid.Init(type, null);
    }

    void Update()
    {

    }
}
