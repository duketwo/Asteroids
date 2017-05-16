﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using Assets.Resources.Scripts.Game.Menu;
using Assets.Resources.Scripts.Util;

namespace Assets.Resources.Scripts.Game
{
    public class CustomNetworkManager : NetworkManager
    {
        // CONSTANTS
        public static int POINTS_ASTEROID = 10;


        // ATTRIBUTES
        private StatusBar _statusBar;
        private int asteroidSpawnDelay = 3000;
        private DateTime nextAsteroidSpawn;
        private static volatile CustomNetworkManager instance;
        private bool isGameStarted;

        // MEMBERS
        public bool IsGameOver { get; set; }

        // METHODS
        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("OnPlayerConnected");
        }

        void Start()
        {
            Debug.Log("Start - CustomNetworkmanager");
        }

        void Update()
        {
            Debug.Log("Update - CustomNetworkmanager");
        }
        public StatusBar StatusBar()
        {
            return _statusBar;
        }

        public void StartGame()
        {
            FindObjectsOfType<StatusBar>().ToList().ForEach(k => Destroy(k.gameObject));
            _statusBar = new GameObject().AddComponent<StatusBar>();
            _statusBar.Init();

            StatusBar().Lives = 3;

            FindObjectsOfType<Player>().ToList().ForEach(k => Destroy(k.gameObject));
            new GameObject().AddComponent<Player>();


            FindObjectsOfType<Bullet>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Asteroid>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<DynamicLabel>().ToList().ForEach(k => Destroy(k.gameObject));
            for (int i = 0; i < 10; i++)
                AddAsteroid(null);
            isGameStarted = true;
        }


        public void RespawnPlayer()
        {
            FindObjectsOfType<Player>().ToList().ForEach(k => Destroy(k.gameObject));
            new GameObject().AddComponent<Player>();
        }


        public static CustomNetworkManager Instance()
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CustomNetworkManager>();

            }
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

        public void DoUpdate()
        {
            if (!isGameStarted)
                return;

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
