using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VR.WSA;

namespace Assets.Resources.Scripts
{
    public class Player : MonoBehaviour
    {

        public float bottomConstraint = 0.0f;
        public float topConstraint = 0.0f;
        public float leftConstraint = 0.0f;
        public float rightConstraint = 0.0f;
        public float buffer = 1.0f;
        public float distanceZ = 10.0f;


        private SpriteRenderer sp;
        private double degree;
        private double speed;
        private Vector2 velocityVector2;
        private Material mat;
        private List<Vector3> positions;
        private float SPEED_STEP_INCREASE = 4.5f;
        private float MAX_SPEED = 10.0f;
        private Quaternion initialRotation;


        void Awake()
        {
            leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
            rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
            topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
            bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;
        }

        void Start()
        {
            degree = 0;
            speed = 0;
            sp = this.gameObject.AddComponent<SpriteRenderer>();
            sp.sortingLayerName = "Foreground";
            sp.sprite = UnityEngine.Resources.Load<Sprite>("Images/spaceship_triangle");
            velocityVector2 = new Vector2(0, 0);
            initialRotation = this.transform.localRotation;
        }

        //        void DrawShip()
        //        {
        //
        //            lr.startWidth = 0.2f;
        //            lr.endWidth = 0.2f;
        //
        //            var pos = this.gameObject.transform.position;
        //
        //            var p0 = new Vector2(pos.x, pos.y + 0.5f);
        //            var p1 = new Vector2(pos.x + 0.5f, pos.y - 0.5f);
        //            var p2 = new Vector2(pos.x, pos.y);
        //            var p3 = new Vector2(pos.x - 0.5f, pos.y - 0.5f);
        //            var p4 = new Vector2(pos.x, pos.y + 0.5f);
        //
        //
        //            positions = new List<Vector3> { p0, p1, p2, p3, p4, }.Select(c =>
        //              {
        //                  var k = Rotate(c, pos, new Vector3(0, 0, (float)degree));
        //                  return k;
        //              }).ToList();
        //
        //            lr.SetPositions(positions.ToArray());
        //            lr.positionCount = positions.Count;
        //        }

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


                velocityVector2.x += Convert.ToSingle(cosAngle) * SPEED_STEP_INCREASE * Time.deltaTime;
                velocityVector2.y += Convert.ToSingle(sinAngle) * SPEED_STEP_INCREASE * Time.deltaTime;

                if (velocityVector2.x > MAX_SPEED)
                    velocityVector2.x = MAX_SPEED;

                if (velocityVector2.y > MAX_SPEED)
                    velocityVector2.y = MAX_SPEED;
            }
            else
            {
                velocityVector2.x -= velocityVector2.x * 0.3f * Time.deltaTime;
                velocityVector2.y -= velocityVector2.y * 0.3f * Time.deltaTime;
            }

            transform.position += new Vector3(velocityVector2.x, velocityVector2.y, 0) * SPEED_STEP_INCREASE * Time.smoothDeltaTime;
        }

        public void Respawn()
        {

        }

        void Update()
        {
           
        }

        void FixedUpdate()
        {
            var v = Input.GetAxis("Vertical");
            var h = Input.GetAxis("Horizontal");

            ModAcceleration(v);
            Utility.ScreenWrap(this.transform);
            SetDegree(h);
        }
    }
}