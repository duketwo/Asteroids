using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Resources.Scripts.Game
{
    public class MyNetworkManager : NetworkManager
    {
        public NetworkConnection conn;
        public short playerControllerId;


        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            this.conn = conn;
            this.playerControllerId = playerControllerId;
            playerPrefab = new GameObject();
            playerPrefab.AddComponent<Player>();
            GameObject player = playerPrefab;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }
}
