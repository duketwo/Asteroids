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
            playerPrefab = new GameObject();
            localPlayer = playerPrefab.AddComponent<Player>();
            GameObject player = playerPrefab;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public void PlayerWasKilled()
        {
            if (localPlayer == null)
                return;

            var conn = localPlayer.connectionToClient;
            Destroy(localPlayer.gameObject);
            var obj = new GameObject();
            var player = obj.AddComponent<Player>();
            localPlayer = player;
            NetworkServer.ReplacePlayerForConnection(conn, obj, playerControllerId);
        }
    }
}
