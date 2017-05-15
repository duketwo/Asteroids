using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Resources.Scripts
{
    class Asteroid : MonoBehaviour
    {
        private AsteroidType type;
        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private Vector2 direction;
        private float SPEED_CONSTANT = 9.0f;
        private List<Vector2> DIRECTIONS = new List<Vector2>()
        {
            new Vector2(1,0),
            new Vector2(-1,0),
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(-1,1),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
        };

        public void Start()
        {
        }

        public void Init()
        {
            direction = DIRECTIONS[Random.Range(0, DIRECTIONS.Count)];
            sr = this.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/square");
            col = this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
        }

        public void Update()
        {
            Utility.ScreenWrap(this.transform);
            transform.position += new Vector3(direction.x, direction.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;
            this.transform.Rotate(0, 0, 1);
        }

        private void Scale()
        {

        }

    }


    enum AsteroidType
    {
        XL,
        M,
        S
    }


}
