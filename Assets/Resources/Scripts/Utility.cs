﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    public static class Utility
    {
        public static void ScreenWrap(Transform transform)
        {

            float buffer = 1.0f;
            float distanceZ = 10.0f;
            var leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
            var rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
            var topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).y;
            var bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, distanceZ)).y;

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
