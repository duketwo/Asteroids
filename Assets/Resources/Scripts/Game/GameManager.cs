﻿using System.Linq;
using Assets.Resources.Scripts.Game.Menu;
using Assets.Resources.Scripts.Util;
using UnityEngine;

namespace Assets.Resources.Scripts.Game
{
    public sealed class GameManager : MonoBehaviour
    {


        public bool IsGameOver;
        private int playerLives;
        private Player _player;
        private StatusBar _statusBar;

        void Start()
        {
            StartGame();
        }

        private GameManager()
        {

        }

        public Player AddPlayer()
        {
            _player = new GameObject().AddComponent<Player>();
            return _player;
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
            playerLives = 3;
            FindObjectsOfType<Player>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Bullet>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Asteroid>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<StatusBar>().ToList().ForEach(k => Destroy(k.gameObject));

            RespawnPlayer();
            AddStatusBar();

            for (int i = 0; i < 10; i++)
                AddAsteroid(null);
        }


        public void PlayerCollided()
        {
            playerLives--;

            Debug.Log("Remaining player lives: " + playerLives);

            if (playerLives == 0)
            {
                SetGameOver();
            }
            else
            {
                RespawnPlayer();
            }
        }

        private void RespawnPlayer()
        {
            if (_player != null)
                Destroy(_player.gameObject);
            AddPlayer().SetInvuln().gameObject.transform.position = Utility.center;
        }

        private static volatile GameManager instance;

        public static GameManager Instance()
        {
            if (instance == null)
                instance = FindObjectOfType<GameManager>();
            return instance;
        }


        public Player Player()
        {
            return _player;
        }

        public void AddAsteroid(AsteroidType? type)
        {
            var asteroid = new GameObject().AddComponent<Asteroid>();
            asteroid.Init(type, null);
        }

        public void SetGameOver()
        {
            DynamicLabel.CreateLabel(string.Format("{0}", "        GAME OVER :( \n PRESS R TO CONTINUE"), DymicLabelPosition.HORIZONTAL_AND_VERTICAL_CENTERED,
                5.0f, 30, true);
            IsGameOver = true;
        }

        void Update()
        {
            if (IsGameOver && Input.GetKeyDown(KeyCode.R))
            {
                StartGame();
                IsGameOver = false;
                return;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartGame();
            }
        }
    }
}
