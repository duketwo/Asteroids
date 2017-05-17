using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using Utility = Assets.Resources.Scripts.Util.Utility;

namespace Assets.Resources.Scripts.Game
{
    class Asteroid : NetworkBehaviour
    {

        public static int POINTS_ASTEROID = 10;

        public AsteroidType Type
        {
            get { return (AsteroidType)_typeInt; }
            set { _typeInt = (int)value; }
        }

        [SyncVar]
        private int _typeInt;
        private int TypeInt
        {
            get
            {
                return _typeInt;
            }

            set { _typeInt = value; }
        }

        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        [SyncVar]
        public Vector2 direction;
        private float SPEED_CONSTANT = 4.0f;
        public static string TAG = "ASTEROID";
        private NetworkIdentity networkIdentity;
        private NetworkTransform networkTransform;



        void Start()
        {
            if (this.GetComponent<NetworkIdentity>() == null)
                networkIdentity = this.gameObject.AddComponent<NetworkIdentity>();
            else
                networkIdentity = this.gameObject.GetComponent<NetworkIdentity>();
            networkIdentity.localPlayerAuthority = false;

            if (this.GetComponent<NetworkTransform>() == null)
                networkTransform = this.gameObject.AddComponent<NetworkTransform>();
            else
                networkTransform = this.gameObject.GetComponent<NetworkTransform>();
            networkTransform.sendInterval = 0.01f;
            this.tag = TAG;

            if (_typeInt != 0)
                SetAsteroidType(Type);

        }

        public void SetRandomDirection()
        {
            this.direction = GetRandomDirection();
        }

        public void SetAsteroidType(AsteroidType? type = null)
        {
            sr = this.gameObject.GetComponent<SpriteRenderer>() != null
                ? this.gameObject.GetComponent<SpriteRenderer>()
                : this.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";


            this.Type = type == null ? GetSemiRandomAsteroidType() : type.Value;
            switch (this.Type)
            {
                case AsteroidType.AsteroidS:
                    sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/square_s");
                    sr.color = Color.yellow;
                    break;
                case AsteroidType.AsteroidM:
                    sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/square_m");
                    sr.color = Color.cyan;
                    break;
                case AsteroidType.AsteroidL:
                    sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/square_l");
                    break;
            }
            this.name = this.Type.ToString();

            col = this.gameObject.GetComponent<PolygonCollider2D>() != null
                ? this.gameObject.GetComponent<PolygonCollider2D>()
                : this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.GetComponent<Rigidbody2D>() != null
                ? this.gameObject.GetComponent<Rigidbody2D>()
                : this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;


        }

        public void SetRandomPosition()
        {
            var rnd = Random.Range(0, 3);
            Vector3 cornerA = Vector3.zero;
            Vector3 cornerB = Vector3.zero;

            switch (rnd)
            {
                case 0:
                    cornerA = Utility.topLeftCorner;
                    cornerB = Utility.botLeftCorner;
                    break;
                case 1:
                    cornerA = Utility.topLeftCorner;
                    cornerB = Utility.topRightCorner;
                    break;
                case 2:
                    cornerA = Utility.topRightCorner;
                    cornerB = Utility.botRightCorner;
                    break;
                case 3:
                    cornerA = Utility.botLeftCorner;
                    cornerB = Utility.botRightCorner;
                    break;
            }

            var xMin = cornerA.x < cornerB.x ? cornerA.x : cornerB.x;
            var xMax = cornerA.x > cornerB.x ? cornerA.x : cornerB.x;
            var yMin = cornerA.y < cornerB.y ? cornerA.y : cornerB.y;
            var yMax = cornerA.y > cornerB.y ? cornerA.y : cornerB.y;

            this.transform.position = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), cornerB.z);
        }

        public void Update()
        {
            if (CustomNetworkManager.Instance().IsGameOver)
                return;

            transform.position += new Vector3(direction.x, direction.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;
            this.transform.Rotate(0, 0, 1);

            ServerUpdate();
        }

        [ServerCallback]
        public void ServerUpdate()
        {
            Utility.ScreenWrap(this.transform);
        }

        #region static, enums, types

        public static AsteroidType GetSemiRandomAsteroidType()
        {
            if (asteroidEntropyDictionary.Values.All(k => k >= 10))
            {
                asteroidEntropyDictionary[AsteroidType.AsteroidL] = 0;
                asteroidEntropyDictionary[AsteroidType.AsteroidM] = 0;
                asteroidEntropyDictionary[AsteroidType.AsteroidS] = 0;
            }

            var l = asteroidEntropyDictionary.Where(k => k.Value < 10);
            var rnd = l.ElementAt(Random.Range(0, l.Count()));
            asteroidEntropyDictionary[rnd.Key]++;
            return rnd.Key;
        }

        public static Vector2 GetRandomDirection()
        {
            return DIRECTIONS[Random.Range(0, DIRECTIONS.Count)];
        }


        private static Dictionary<AsteroidType, int> asteroidEntropyDictionary = new Dictionary<AsteroidType, int>()
        {
            {AsteroidType.AsteroidL, 0},
            {AsteroidType.AsteroidM, 0},
            {AsteroidType.AsteroidS, 0}
        };

        public static List<Vector2> DIRECTIONS = new List<Vector2>()
        {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,0),
            new Vector2(-1,1),
        };
    }


    public enum AsteroidType
    {
        AsteroidL = 1,
        AsteroidM = 2,
        AsteroidS = 3
    }

    #endregion
}
