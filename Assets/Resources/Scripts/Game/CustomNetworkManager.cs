using System;
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

        public static int PlayerCount { get; set; }

        #region Overrides of NetworkManager

        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("OnPlayerConnected Player count: " + this.numPlayers);
            base.OnServerConnect(conn);
        }
        public override void OnClientConnect(NetworkConnection conn)
        {
            Debug.Log("OnClientConnect");
            base.OnClientConnect(conn);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("OnClientDisconnect");
            base.OnClientDisconnect(conn);
        }


        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            PlayerCount++;
            Debug.Log("OnServerAddPlayer");
            base.OnServerAddPlayer(conn, playerControllerId, extraMessageReader);
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            PlayerCount++;
            Debug.Log("OnServerAddPlayer");
            base.OnServerAddPlayer(conn, playerControllerId);
        }

        public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
        {
            PlayerCount--;
            Debug.Log("OnServerRemovePlayer");
            base.OnServerRemovePlayer(conn, player);
        }
        #endregion

        void Start()
        {
            Debug.Log("Start - CustomNetworkmanager");
        }
    }
}
