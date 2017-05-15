using System;
using System.Collections.Generic;
using Assets.Resources.Scripts.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Resources.Scripts.Game
{
    class Asteroid : MonoBehaviour
    {
        public AsteroidType type;
        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        public Vector2 direction;
        private float SPEED_CONSTANT = 4.0f;
        public static string TAG = "ASTEROID";
        public List<Vector2> DIRECTIONS = new List<Vector2>()
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

        public void Start()
        {
        }

        public Asteroid Init(AsteroidType? type, Vector2? direction, Vector3? position = null)
        {

            if (direction == null)
            {
                this.direction = DIRECTIONS[Random.Range(0, DIRECTIONS.Count)];
            }
            else
            {
                this.direction = direction.Value;
            }

            sr = this.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/square");
            col = this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            if (type == null) // pick random type if type is null
            {
                Array values = Enum.GetValues(typeof(AsteroidType));
                this.type = (AsteroidType)values.GetValue(Random.Range(0, values.Length));
            }
            else
            {
                this.type = type.Value;
            }

            switch (this.type)
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


            this.tag = TAG;
            this.name = this.type.ToString();


            if (position == null)
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
                        cornerB = Utility.botRightCorner;
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
            else
            {
                this.transform.position = position.Value;
            }

            return this;
        }

        public void Update()
        {
            Utility.ScreenWrap(this.transform);
            this.transform.Rotate(0, 0, 1);

            if (GameManager.Instance().IsGameOver)
                return;

            transform.position += new Vector3(direction.x, direction.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;
        }

        private void Scale()
        {

        }

    }


    public enum AsteroidType
    {
        AsteroidL,
        AsteroidM,
        AsteroidS
    }


}
