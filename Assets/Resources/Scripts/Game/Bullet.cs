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
            networkTransform.sendInterval = 0.015f;
        }

        private void OnTriggerEnter2D(Collider2D c)
        {

            if (!isServer)
                return;

            if (collidedAlready || c.tag != Asteroid.TAG)
                return;


            collidedAlready = true;

            CustomNetworkManager.Instance().StatusBar().Points += Asteroid.POINTS_ASTEROID;

            var asteroid = c.gameObject.GetComponent<Asteroid>();

            Debug.Log("Bullet colllided with an asteroid. Type: " + asteroid.Type);

            var direction = asteroid.direction;
            var direcIndex = Asteroid.DIRECTIONS.IndexOf(direction);
            var orthoDirectPos = Asteroid.DIRECTIONS[(direcIndex - 1 + Asteroid.DIRECTIONS.Count - 1) % (Asteroid.DIRECTIONS.Count - 1)];
            var orthoDirectNeg = Asteroid.DIRECTIONS[(direcIndex + 1) % (Asteroid.DIRECTIONS.Count - 1)];
            switch (asteroid.Type)
            {
                case AsteroidType.AsteroidL:
                    var asteroidMOrthoPos = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Asteroid"));
                    var asteroidMScriptCompOrthoPos = asteroidMOrthoPos.GetComponent<Asteroid>();
                    asteroidMScriptCompOrthoPos.SetAsteroidType(AsteroidType.AsteroidM);
                    asteroidMOrthoPos.transform.position = c.transform.position;
                    asteroidMScriptCompOrthoPos.direction = orthoDirectPos;
                    NetworkServer.Spawn(asteroidMOrthoPos);

                    var asteroidMOrthoNeg = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Asteroid"));
                    var asteroidMScriptCompOrthoNeg = asteroidMOrthoNeg.GetComponent<Asteroid>();
                    asteroidMScriptCompOrthoNeg.SetAsteroidType(AsteroidType.AsteroidM);
                    asteroidMOrthoNeg.transform.position = c.transform.position;
                    asteroidMScriptCompOrthoNeg.direction = orthoDirectNeg;
                    NetworkServer.Spawn(asteroidMOrthoNeg);
                    break;

                case AsteroidType.AsteroidM:
                    var asteroidSOrthoPos = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Asteroid"));
                    var asteroidSScriptCompOrthoPos = asteroidSOrthoPos.GetComponent<Asteroid>();
                    asteroidSScriptCompOrthoPos.SetAsteroidType(AsteroidType.AsteroidS);
                    asteroidSOrthoPos.transform.position = c.transform.position;
                    asteroidSScriptCompOrthoPos.direction = orthoDirectPos;
                    NetworkServer.Spawn(asteroidSOrthoPos);

                    var asteroidSOrthoNeg = (GameObject)Instantiate(UnityEngine.Resources.Load<GameObject>("Asteroid"));
                    var asteroidSScriptCompOrthoNeg = asteroidSOrthoNeg.GetComponent<Asteroid>();
                    asteroidSScriptCompOrthoNeg.SetAsteroidType(AsteroidType.AsteroidS);
                    asteroidSOrthoNeg.transform.position = c.transform.position;
                    asteroidSScriptCompOrthoNeg.direction = orthoDirectNeg;
                    NetworkServer.Spawn(asteroidSOrthoNeg);
                    break;
            }

            Destroy(c.gameObject);
            NetworkServer.Destroy(c.gameObject);
            Destroy(this.gameObject);
            NetworkServer.Destroy(this.gameObject);
        }

        void Update()
        {
            if (CustomNetworkManager.Instance().IsGameOver)
                return;

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
