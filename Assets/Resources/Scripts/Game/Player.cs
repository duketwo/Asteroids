﻿using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Resources.Scripts.Game.Menu;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


namespace Assets.Resources.Scripts.Game
{
    public class Player : NetworkBehaviour
    {
        public static string TAG = "PLAYER";
        private static int PLAYER_LIVES = 10;
        private SpriteRenderer sr;
        [SyncVar]
        private double degree;
        [SyncVar]
        private Vector2 velocityVector2;
        private Material mat;
        private float SPEED_CONSTANT = 4.5f;
        private float MAX_SPEED = 6.0f;
        private Quaternion initialRotation;
        private GameObject bulletSpawnSpot;


        [SyncVar]
        private long _lastShotTicks;

        private DateTime lastShot
        {
            get { return new DateTime(_lastShotTicks); }
            set { _lastShotTicks = value.Ticks; }

        }

        private PolygonCollider2D col;
        private Rigidbody2D rb;


        [SyncVar]
        private long _invulnUntilTicks;

        private DateTime invulnUntil
        {
            get { return new DateTime(_invulnUntilTicks); }
            set { _invulnUntilTicks = value.Ticks; }
        }

        private NetworkIdentity networkIdentity;
        private NetworkTransform networkTransform;
        [SyncVar]
        private bool dead;
        private StatusBar _statusBar;
        [SyncVar]
        private int _playerLives;
        [SyncVar]
        private int _playerPoints;
        public static Player LocalPlayer { get; private set; }



        void Start()
        {
            degree = 0;
            if (this.gameObject.GetComponent<SpriteRenderer>() == null)
                sr = this.gameObject.AddComponent<SpriteRenderer>();
            else
                sr = this.gameObject.GetComponent<SpriteRenderer>();

            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/spaceship_triangle");
            velocityVector2 = Vector2.zero;
            initialRotation = this.transform.localRotation;
            bulletSpawnSpot = new GameObject();
            bulletSpawnSpot.transform.position = this.transform.position + new Vector3(0, 1.0f);
            bulletSpawnSpot.transform.SetParent(this.transform);
            lastShot = DateTime.MinValue;


            if (this.gameObject.GetComponent<PolygonCollider2D>() == null)
                col = this.gameObject.AddComponent<PolygonCollider2D>();
            else
                col = this.gameObject.GetComponent<PolygonCollider2D>();

            col.isTrigger = true;


            if (this.gameObject.GetComponent<Rigidbody2D>() == null)
                rb = this.gameObject.AddComponent<Rigidbody2D>();
            else
                rb = this.gameObject.GetComponent<Rigidbody2D>();

            rb.isKinematic = true;
            this.tag = TAG;
            this.name = this.GetType().Name;
            CmdSetInvuln();
            if (this.GetComponent<NetworkIdentity>() == null)
                networkIdentity = this.gameObject.AddComponent<NetworkIdentity>();
            else
                networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();
            networkIdentity.localPlayerAuthority = true;

            if (this.GetComponent<NetworkTransform>() == null)
                networkTransform = this.gameObject.AddComponent<NetworkTransform>();
            else
                networkTransform = this.gameObject.GetComponent<NetworkTransform>();
            networkTransform.sendInterval = 0.005f;

        }

        [TargetRpc]
        public void TargetSetPlayerLives(NetworkConnection nc)
        {
            StatusBar().Lives = _playerLives;
        }

        [TargetRpc]
        public void TargetSetPlayerPoints(NetworkConnection nc)
        {
            StatusBar().Points = _playerPoints;
        }

        public StatusBar StatusBar()
        {
            if (_statusBar == null)
            {
                FindObjectsOfType<StatusBar>().ToList().ForEach(k => DestroyImmediate(k.gameObject));
                _statusBar = new GameObject().AddComponent<StatusBar>();
                _statusBar.Init();
            }
            return _statusBar;
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("OnStartLocalPlayer");
            base.OnStartLocalPlayer();
        }

        public override void OnStartClient() // CALLED ON EVERY CLIENT INIT
        {
            Debug.Log("OnStartClient");
            LocalPlayer = this;
            StatusBar().Init();
            base.OnStartClient();
        }

        public override void OnStartServer() // ONLY CALLED WHILE SERVER INIT
        {
            Debug.Log("OnStartServer");
            Debug.Log("connectionToClientId: " + connectionToClient.connectionId);
            _playerLives = PLAYER_LIVES;
            TargetSetPlayerLives(connectionToClient);
            CmdRespawn();
            base.OnStartServer();
        }

        [Command]
        public void CmdSetInvuln()
        {
            invulnUntil = DateTime.Now.AddMilliseconds(1500);
        }

        public Vector3 Rotate(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            return Quaternion.Euler(-angles) * (point - pivot) + pivot;
        }

        public void SetDegree(float val)
        {

            if (val == 0)
                return;

            this.degree += (val * Time.deltaTime * 200.0f);
            this.degree %= 360;
            while (this.degree < 0)
                this.degree += 360;

            this.transform.rotation = initialRotation;
            transform.Rotate(0, 0, Convert.ToSingle(-degree));
        }

        public void ModAcceleration(float val)
        {
            if (val > 0)
            {

                double rad = Math.PI * degree / 180.0;
                double sinAngle = Math.Cos(rad);
                double cosAngle = Math.Sin(rad);


                velocityVector2.x += Convert.ToSingle(cosAngle) * SPEED_CONSTANT * Time.deltaTime;
                velocityVector2.y += Convert.ToSingle(sinAngle) * SPEED_CONSTANT * Time.deltaTime;

                if (velocityVector2.x > MAX_SPEED)
                    velocityVector2.x = MAX_SPEED;

                if (velocityVector2.x < -MAX_SPEED)
                    velocityVector2.x = -MAX_SPEED;

                if (velocityVector2.y > MAX_SPEED)
                    velocityVector2.y = MAX_SPEED;

                if (velocityVector2.y < -MAX_SPEED)
                    velocityVector2.y = -MAX_SPEED;
            }
            else
            {
                velocityVector2.x -= velocityVector2.x * 0.3f * Time.deltaTime;
                velocityVector2.y -= velocityVector2.y * 0.3f * Time.deltaTime;
            }
        }


        [Command]
        public void CmdShoot()
        {
            var obj = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Bullet"));
            var bulletScript = obj.GetComponent<Bullet>();
            bulletScript.direction = bulletSpawnSpot.transform.position - this.transform.position;
            bulletScript.transform.position = bulletSpawnSpot.transform.position;
            bulletScript.transform.rotation = bulletSpawnSpot.transform.rotation;
            NetworkServer.Spawn(obj);
        }

        [ServerCallback]
        private void OnTriggerEnter2D(Collider2D c)
        {
            if (c.tag != Asteroid.TAG || dead)
                return;
            Debug.Log("Player colllided with asteroid.");

            dead = true;

            if (_playerLives > 0)
            {
                _playerLives--;
                TargetSetPlayerLives(connectionToClient);
            }

            Debug.Log("Remaining player lives: " + _playerLives);

            if (_playerLives == 0)
            {
                CustomNetworkManager.Instance().SetGameOver();
            }
            else
            {
                CmdRespawn();
            }

        }


        [Command]
        public void CmdRespawn()
        {
            CmdSetInvuln();
            this.transform.position = Util.Utility.center;
            dead = false;
            this.gameObject.transform.localRotation = initialRotation;
            this.velocityVector2 = Vector2.zero;
            this.degree = 0;
        }


        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (CustomNetworkManager.Instance().IsGameOver)
                return;

            if (invulnUntil > DateTime.Now)
            {
                col.enabled = false;
                sr.color = Color.grey;
            }
            else
            {
                col.enabled = true;
                sr.color = Color.green;
            }

            var v = Input.GetAxis("Vertical");
            var h = Input.GetAxis("Horizontal");

            if (Input.GetKey(KeyCode.Space) && lastShot < DateTime.Now)
            {

                CmdShoot();
                lastShot = DateTime.Now.AddMilliseconds(100);
            }

            ModAcceleration(v);
            Util.Utility.ScreenWrap(this.transform);
            SetDegree(h);

            transform.position += new Vector3(velocityVector2.x, velocityVector2.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;
        }

        void FixedUpdate()
        {

        }
    }
}