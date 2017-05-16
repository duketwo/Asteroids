using System;
using UnityEngine;
using UnityEngine.Networking;
using Utility = Assets.Resources.Scripts.Util.Utility;

namespace Assets.Resources.Scripts.Game
{
    class Bullet : NetworkBehaviour
    {

        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        [SyncVar]
        public Vector3 direction;
        private float SPEED_CONSTANT = 9.0f;
        private DateTime timeDestroy;
        public static string TAG = "BULLET";
        [SyncVar]
        private bool collidedAlready;
        private NetworkIdentity networkIdentity;
        private NetworkTransform networkTransform;


        void Start()
        {
            timeDestroy = DateTime.Now.AddSeconds(2);
            sr = this.gameObject.GetComponent<SpriteRenderer>() != null ? this.gameObject.GetComponent<SpriteRenderer>() : this.gameObject.AddComponent<SpriteRenderer>();

            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/laser");
            col = this.gameObject.GetComponent<PolygonCollider2D>() != null ? this.gameObject.GetComponent<PolygonCollider2D>() : this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.GetComponent<Rigidbody2D>() != null ? this.gameObject.GetComponent<Rigidbody2D>() : this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            this.tag = TAG;
            this.name = this.GetType().Name;

            if (this.GetComponent<NetworkIdentity>() == null)
                networkIdentity = this.gameObject.AddComponent<NetworkIdentity>();
            else
                networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();
            networkIdentity.localPlayerAuthority = true;

            if (this.GetComponent<NetworkTransform>() == null)
                networkTransform = this.gameObject.AddComponent<NetworkTransform>();
            else
                networkTransform = this.gameObject.GetComponent<NetworkTransform>();
            networkTransform.sendInterval = 1.0f;
        }

        private void OnTriggerEnter2D(Collider2D c)
        {
//            if (collidedAlready || c.tag != Asteroid.TAG)
//                return;
//
//            Debug.Log("Bullet colllided with an asteroid.");
//            collidedAlready = true;
//
//            CustomNetworkManager.Instance().StatusBar().Points += CustomNetworkManager.POINTS_ASTEROID;
//
//            var asteroid = c.gameObject.GetComponent<Asteroid>();
//            var direction = asteroid.direction;
//            var direcIndex = asteroid.DIRECTIONS.IndexOf(direction);
//            var orthoDirectPos = asteroid.DIRECTIONS[(direcIndex - 1 + asteroid.DIRECTIONS.Count - 1) % (asteroid.DIRECTIONS.Count - 1)];
//            var orthoDirectNeg = asteroid.DIRECTIONS[(direcIndex + 1) % (asteroid.DIRECTIONS.Count - 1)];
//            switch (asteroid.type)
//            {
//                case AsteroidType.AsteroidL:
//                    new GameObject().AddComponent<Asteroid>().Init(AsteroidType.AsteroidM, orthoDirectPos).transform.position = c.transform.position;
//                    new GameObject().AddComponent<Asteroid>().Init(AsteroidType.AsteroidM, orthoDirectNeg).transform.position = c.transform.position;
//                    break;
//                case AsteroidType.AsteroidM:
//                    new GameObject().AddComponent<Asteroid>().Init(AsteroidType.AsteroidS, orthoDirectPos).transform.position = c.transform.position;
//                    new GameObject().AddComponent<Asteroid>().Init(AsteroidType.AsteroidS, orthoDirectNeg).transform.position = c.transform.position;
//                    break;
//            }
//
//            Destroy(c.gameObject);
//            NetworkServer.Destroy(this.gameObject);
//            Destroy(this.gameObject);
        }

        void Update()
        {

//            if (CustomNetworkManager.Instance().IsGameOver)
//                return;

            Utility.ScreenWrap(this.transform);
            transform.position += new Vector3(direction.x, direction.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;

            if (timeDestroy < DateTime.Now)
            {
                Destroy(this.gameObject);
                NetworkServer.Destroy(this.gameObject);
            }
        }

    }
}
