using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Resources.Scripts.Util;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Resources.Scripts.Game
{
    public class GameManager : NetworkBehaviour
    {

        private int asteroidSpawnDelay = 3000;
        private DateTime nextAsteroidSpawn;
        private static volatile GameManager instance;
        private bool isGameStarted;

        void Start()
        {

        }

        void Update()
        {
            //&& CustomNetworkManager.PlayerCount > 1
            if (!isGameStarted)
            {
                StartGame();
                return;
            }

            if (!isGameStarted)
                return;

            if (nextAsteroidSpawn < DateTime.Now)
            {
                nextAsteroidSpawn = DateTime.Now.AddMilliseconds(asteroidSpawnDelay);
                if (asteroidSpawnDelay > 150)
                    asteroidSpawnDelay -= 50;
                //AddAsteroid(null);
            }

        
        }

        public void StartGame()
        {
            FindObjectsOfType<Player>().Where(k => k.isLocalPlayer).ToList().ForEach(k => k.CmdInitPlayer());
            FindObjectsOfType<Bullet>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Asteroid>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<DynamicLabel>().ToList().ForEach(k => Destroy(k.gameObject));
            for (int i = 0; i < 10; i++)
                AddAsteroid(null);
            isGameStarted = true;
        }

        public void AddAsteroid(AsteroidType? type)
        {
            var obj = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Asteroid"));
            var asteroid = obj.GetComponent<Asteroid>();
            asteroid.SetAsteroidType();
            asteroid.SetRandomDirection();
            asteroid.SetRandomPosition();
            NetworkServer.Spawn(obj);
        }
    }
}
