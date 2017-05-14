using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Resources.Scripts
{
    class Bullet : MonoBehaviour
    {

        private SpriteRenderer sp;
        private Vector3 direction;

        void Start()
        {
            sp = this.gameObject.AddComponent<SpriteRenderer>();
            sp.sortingLayerName = "Foreground";
            sp.sprite = UnityEngine.Resources.Load<Sprite>("Images/laser");
        }

        public void Init(Vector3 direction)
        {
            this.direction = direction;
        }

        void Update()
        {
            Utility.ScreenWrap(this.transform);
        }

    }
}
