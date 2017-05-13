using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.VR.WSA;

namespace Assets.Resources.Scripts
{
    public class Player : MonoBehaviour
    {

        private float velocity;
        private float acceleration;
        private LineRenderer lr;
        private SpriteRenderer sp;
        private float rotation;
        private float speed;
        private float MIN_SPEED = -20;
        private float MAX_SPEED = 20;
        private Vector2 direction;
        private Material mat;

        void Start()
        {
            mat = new Material(Shader.Find("Sprites/Default"));
            rotation = 0;
            lr = this.gameObject.AddComponent<LineRenderer>();
            sp = this.gameObject.AddComponent<SpriteRenderer>();
            sp.sortingLayerName = "Foreground";
            lr.sortingLayerName = "Foreground";
        }

        void DrawShip()
        {
            lr.material = mat;
            lr.enabled = false;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.startColor = Color.green;
            lr.endColor = Color.green;

            var pos = this.gameObject.transform.position;

            var p0 = new Vector2(pos.x, pos.y + 1f);
            var p1 = new Vector2(pos.x + 1f, pos.y - 1f);
            var p2 = new Vector2(pos.x, pos.y);
            var p3 = new Vector2(pos.x - 1f, pos.y - 1f);
            var p4 = new Vector2(pos.x, pos.y + 1f);



            var positions = new List<Vector3> { p0, p1, p2, p3, p4, }.Select(c =>
                {
                    var k = RotatePointAroundPivot(c, pos, new Vector3(0, 0, rotation));
                    return k;
                })
                .ToArray();

            direction = new Vector2(positions[0].x, positions[0].y) - new Vector2(pos.x, pos.y);

            lr.SetPositions(positions);
            lr.positionCount = positions.Length;
            lr.enabled = true;

        }

        public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            Vector3 dir = point - pivot;
            dir = Quaternion.Euler(angles) * dir;
            point = dir + pivot;
            return point;
        }

        public void Turn(float val)
        {
            this.rotation -= val * Time.deltaTime * 200.0f;
        }

        public void ChangeAcceleration(float val)
        {
            var speed = this.speed + val * 0.1f;
            this.speed = speed <= MAX_SPEED && speed >= MIN_SPEED ? speed : this.speed;
        }

        void Update()
        {
            //var h = Input.GetAxis("Horizontal");
            //var v = Input.GetAxis("Vertical");

            //this.rotation += h * Time.deltaTime * 100.0f;

            //this.transform.position += new Vector3(0.05f,0,0);
           
            transform.position += new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
            //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
            //var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

            //transform.Rotate(0, 0, Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f);
            //transform.Translate(z, 0, 0);


            DrawShip();
        }
    }
}
