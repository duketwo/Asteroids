using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets.Resources.Scripts.Game
{
    public class CustomNetworkManager : NetworkManager
    {
        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("OnPlayerConnected");
        }
    }
}
