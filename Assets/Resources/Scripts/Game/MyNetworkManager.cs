using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Resources.Scripts.Game
{
    public class MyNetworkManager : NetworkManager
    {
        public NetworkConnection conn;
        public short playerControllerId;
        public Player localPlayer;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameManager.Instance().StartGame();
            this.conn = conn;
            this.playerControllerId = playerControllerId;
            //playerPrefab = PlayerPrefab;
            GameObject player = Instantiate(playerPrefab);
            player.SetActive(true);
            player.AddComponent<Player>();
            localPlayer = player.GetComponent<Player>();
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            Debug.Log("PlayerId: " + playerControllerId);
        }

        public void PlayerWasKilled()
        {
            if (localPlayer == null)
                return;

            var conn = localPlayer.connectionToClient;
            Destroy(localPlayer.gameObject);

            GameObject player = Instantiate(playerPrefab);
            player.SetActive(true);
            localPlayer = player.GetComponent<Player>();
            NetworkServer.ReplacePlayerForConnection(conn, player, playerControllerId);
        }


//        private GameObject _playerPrefab;
//        public GameObject PlayerPrefab
//        {
//            get
//            {
//                if (_playerPrefab == null)
//                {
//                    _playerPrefab = new GameObject();
//                    _playerPrefab.SetActive(false);
//                    _playerPrefab.AddComponent<Player>();
//                }
//                return _playerPrefab;
//            }
//        }
    }
}
