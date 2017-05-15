using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;


namespace Assets.Resources.Scripts.Game
{
    public class Player : NetworkBehaviour
    {
        public static string TAG = "PLAYER";
        private SpriteRenderer sr;
        private double degree;
        private Vector2 velocityVector2;
        private Material mat;
        private List<Vector3> positions;
        private float SPEED_CONSTANT = 4.5f;
        private float MAX_SPEED = 6.0f;
        private Quaternion initialRotation;
        private GameObject bulletSpawnSpot;
        private DateTime lastShot;
        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private DateTime invulnUntil;
        private NetworkIdentity networkIdentity;
        private bool dead;


        void Start()
        {
            degree = 0;
            sr = this.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/spaceship_triangle");
            velocityVector2 = new Vector2(0, 0);
            initialRotation = this.transform.localRotation;
            bulletSpawnSpot = new GameObject();
            bulletSpawnSpot.transform.position = this.transform.position + new Vector3(0, 1.0f);
            bulletSpawnSpot.transform.SetParent(this.transform);
            lastShot = DateTime.MinValue;
            col = this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            this.tag = TAG;
            this.name = this.GetType().Name;
            SetInvuln();
            if (this.GetComponent<NetworkIdentity>() == null)
                networkIdentity = this.gameObject.AddComponent<NetworkIdentity>();
            this.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

        }

        public Player SetInvuln(int milliseconds = 1500)
        {
            invulnUntil = DateTime.Now.AddMilliseconds(milliseconds);
            return this;
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

            transform.position += new Vector3(velocityVector2.x, velocityVector2.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;
        }

        public void Shoot()
        {
            var bullet = new GameObject().AddComponent<Bullet>();
            bullet.Init(bulletSpawnSpot.transform.position - this.transform.position, bulletSpawnSpot.transform.position, bulletSpawnSpot.transform.rotation);
        }

        private void OnTriggerEnter2D(Collider2D c)
        {
            if (c.tag != Asteroid.TAG || dead)
                return;
            Debug.Log("Player colllided with asteroid.");

            dead = true;

            if (GameManager.Instance().StatusBar().Lives > 0)
                GameManager.Instance().StatusBar().Lives--;

            Debug.Log("Remaining player lives: " + GameManager.Instance().StatusBar().Lives);

            if (GameManager.Instance().StatusBar().Lives == 0)
            {
                GameManager.Instance().SetGameOver();
            }
            else
            {
                CmdRespawnPlayer();
            }

        }

        [Command]
        public void CmdRespawnPlayer()
        {
            MyNetworkManager myNetworkManager = GameManager.Instance().networkManager;
            myNetworkManager.PlayerWasKilled();
        }


        void Update()
        {
            if (!isLocalPlayer)
            {
                Debug.Log("Is not local player!");
                return;
            }

            if (GameManager.Instance().IsGameOver)
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
                Shoot();
                lastShot = DateTime.Now.AddMilliseconds(100);
            }

            ModAcceleration(v);
            Util.Utility.ScreenWrap(this.transform);
            SetDegree(h);
        }

        void FixedUpdate()
        {

        }
    }
}


//[Command]
//void CmdRespawnSvr()
//{
//var spawn = NetworkManager.singleton.GetStartPosition();
//var newPlayer = (GameObject)Instantiate(NetworkManager.singleton.playerPrefab, spawn.position, spawn.rotation);
//NetworkServer.Destroy(this.gameObject);
//NetworkServer.ReplacePlayerForConnection(this.connectionToClient, newPlayer, this.playerControllerId);
//
//}