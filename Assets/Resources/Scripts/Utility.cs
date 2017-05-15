using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    public static class Utility
    {
        private static float buffer = 1.0f;
        private static float cameraDistZ = 10.0f;
        private static float leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cameraDistZ)).x;
        private static float rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, cameraDistZ)).x;
        private static float topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cameraDistZ)).y;
        private static float bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, cameraDistZ)).y;

        public static Vector3 topLeftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, cameraDistZ));
        public static Vector3 topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, cameraDistZ));
        public static Vector3 botLeftCorner = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, cameraDistZ));
        public static Vector3 botRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cameraDistZ));
        public static Vector3 center = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2, Screen.height/2, cameraDistZ));


        public static void ScreenWrap(Transform transform)
        {
            if (transform.position.x < leftConstraint - buffer)
            {
                transform.position = new Vector2(rightConstraint + buffer, transform.position.y);
            }

            if (transform.position.x > rightConstraint + buffer)
            {
                transform.position = new Vector2(leftConstraint - buffer, transform.position.y);
            }

            if (transform.position.y < topConstraint - buffer)
            {
                transform.position = new Vector2(transform.position.x, bottomConstraint + buffer);
            }

            if (transform.position.y > bottomConstraint + buffer)
            {
                transform.position = new Vector2(transform.position.x, topConstraint - buffer);
            }
        }
    }
}
