using System.Linq;
using Assets.Resources.Scripts.Util;
using UnityEngine;

namespace Assets.Resources.Scripts.Game
{
    public sealed class GameManager : MonoBehaviour
    {


        public bool IsGameOver;

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


        public void StartGame()
        {

            FindObjectsOfType<Player>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Bullet>().ToList().ForEach(k => Destroy(k.gameObject));
            FindObjectsOfType<Asteroid>().ToList().ForEach(k => Destroy(k.gameObject));

            RespawnPlayer();
            for (int i = 0; i < 10; i++)
                AddAsteroid(null);
        }

        public void RespawnPlayer()
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
                return;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                StartGame();
            }
        }
    }
}
