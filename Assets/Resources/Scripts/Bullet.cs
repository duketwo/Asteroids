using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    class Bullet : MonoBehaviour
    {

        private PolygonCollider2D col;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private Vector3 direction;
        private float SPEED_CONSTANT = 9.0f;
        private DateTime timeDestroy;
        public static string TAG = "BULLET";


        void Start()
        {

        }

        public void Init(Vector3 direction, Vector3 pos, Quaternion rotation)
        {
            timeDestroy = DateTime.Now.AddSeconds(3);
            sr = this.gameObject.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";
            sr.sprite = UnityEngine.Resources.Load<Sprite>("Images/laser");
            col = this.gameObject.AddComponent<PolygonCollider2D>();
            col.isTrigger = true;
            rb = this.gameObject.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;
            this.tag = TAG;
            this.name = TAG;
            this.direction = direction;
            this.transform.position = pos;
            this.transform.rotation = rotation;
        }

        private void OnTriggerEnter2D(Collider2D c)
        {

            if (c.tag == Asteroid.TAG)
            {
                Debug.Log("Bullet colllided with asteroid.");
            }
            
        }

        void Update()
        {
            Utility.ScreenWrap(this.transform);
            transform.position += new Vector3(direction.x, direction.y, 0) * SPEED_CONSTANT * Time.smoothDeltaTime;

            if (timeDestroy < DateTime.Now)
                Destroy(this.gameObject);
        }

    }
}
