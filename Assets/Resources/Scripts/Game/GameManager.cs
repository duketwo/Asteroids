using System;
using System.Linq;
using Assets.Resources.Scripts.Game.Menu;
using Assets.Resources.Scripts.Util;
using UnityEngine;
using UnityEngine.Networking;
using Utility = Assets.Resources.Scripts.Util.Utility;

namespace Assets.Resources.Scripts.Game
{
    public sealed class GameManager : NetworkBehaviour
    {


        public bool IsGameOver;
        private StatusBar _statusBar;
        public static int POINTS_ASTEROID = 10;
        private int asteroidSpawnDelay = 3000;
        private DateTime nextAsteroidSpawn;
        private GameObject networkManagerGameObject;
        public MyNetworkManager networkManager;
        private NetworkManagerHUD networkManagerHud;
        private static volatile GameManager instance;

        void Start()
        {
            networkManagerGameObject = new GameObject();
            networkManager = networkManagerGameObject.AddComponent<MyNetworkManager>();
            networkManagerHud = networkManagerGameObject.AddComponent<NetworkManagerHUD>();
            networkManagerHud.manager = networkManager;
            networkManager.autoCreatePlayer = true;
            networkManagerHud.showGUI = true;
            StartGame();
        }

        public StatusBar AddStatusBar()
        {
            _statusBar = new GameObject().AddComponent<StatusBar>();
            _statusBar.Init();
            return _statusBar;
        }

        public StatusBar StatusBar()
        {
            return _statusBar;
        }


        public void StartGame()
        {
            FindObjectsOfType<Player>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Bullet>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Asteroid>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<StatusBar>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<DynamicLabel>().ToList().ForEach(k => Destroy(k.gameObject));

            //Player.CmdRespawnPlayer();
            AddStatusBar();

            StatusBar().Lives = 3;

            for (int i = 0; i < 10; i++)
                AddAsteroid(null);
        }


        public static GameManager Instance()
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }

        public void AddAsteroid(AsteroidType? type)
        {
            var asteroid = new GameObject().AddComponent<Asteroid>();
            asteroid.Init(type, null);
        }

        public void SetGameOver()
        {
            if (IsGameOver)
                return;

            DynamicLabel.CreateLabel(string.Format("{0}", "        GAME OVER :( \n PRESS R TO CONTINUE"), DymicLabelPosition.HORIZONTAL_AND_VERTICAL_CENTERED,
                5.0f, 30, true);
            IsGameOver = true;
        }

        void Update()
        {

            if (!IsGameOver && nextAsteroidSpawn < DateTime.Now)
            {
                nextAsteroidSpawn = DateTime.Now.AddMilliseconds(asteroidSpawnDelay);
                if (asteroidSpawnDelay > 150)
                    asteroidSpawnDelay -= 50;
                AddAsteroid(null);
            }

            if (IsGameOver && Input.GetKeyDown(KeyCode.R))
            {
                IsGameOver = false;
                StartGame();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StatusBar().Lives = 0;
                SetGameOver();
                return;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartGame();
                IsGameOver = false;
                return;
            }
        }
    }
}
