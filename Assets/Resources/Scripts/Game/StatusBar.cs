using System;
using Assets.Resources.Scripts.Util;
using UnityEngine;

namespace Assets.Resources.Scripts.Game.Menu
{
    public class StatusBar : MonoBehaviour
    {

        private GUIStyle _guiStyle;
        private GUIContent _guiContentPoints;
        private Rect _guiRectPoints;
        private GUIContent _guiContentLives;
        private Rect _guiRectLives;
        private bool _initialized { get; set; }
        private static int FONT_SIZE = 70;


        void Start()
        {
            AspectUtility.OnWindowResize += AspectUtilityOnOnWindowResize;
            this.name = this.GetType().Name;
        }

        private void AspectUtilityOnOnWindowResize(object sender, EventArgs eventArgs)
        {
            this.Points = this.Points;
            this.Lives = this.Lives;
            Rect screenRect = AspectUtility.screenRect;
            _guiStyle = new GUIStyle();
            _guiStyle.fontSize = (int)(screenRect.width / FONT_SIZE);
            _guiStyle.font = UnityEngine.Resources.Load<Font>("Fonts/Nulshock_free");
            _guiStyle.normal.textColor = new Color(1, 1, 1, 0.9f);
        }

        private int _points { get; set; }

        public int Points
        {
            get { return _points; }
            set
            {
                _points = value;
                Rect screenRect = AspectUtility.screenRect;
                _guiContentPoints = new GUIContent("POINTS: " + _points.ToString("D6"));
                var size = _guiStyle.CalcSize(_guiContentPoints);
                _guiRectPoints = new Rect();
                _guiRectPoints.x = (float)0.10 * screenRect.width;
                _guiRectPoints.y = (float)0.014 * screenRect.height;
                _guiRectPoints.width = (float)(screenRect.width * size.x);
                _guiRectPoints.height = (float)(screenRect.height * size.y);
            }
        }

        private int _lives { get; set; }

        public int Lives
        {
            get { return _lives; }
            set
            {
                _lives = value;
                Rect screenRect = AspectUtility.screenRect;
                _guiContentLives = new GUIContent("LIVES: " + _lives.ToString("D2"));
                var size = _guiStyle.CalcSize(_guiContentLives);
                _guiRectLives = new Rect();
                _guiRectLives.x = (float)0.01 * screenRect.width;
                _guiRectLives.y = (float)0.014 * screenRect.height;
                _guiRectLives.width = (float)(screenRect.width * size.x);
                _guiRectLives.height = (float)(screenRect.height * size.y);
            }
        }

        public void OnGUI()
        {
            if (!this._initialized)
                return;

            GUILayout.BeginArea(AspectUtility.screenRect);
            GUI.Label(_guiRectPoints, _guiContentPoints, _guiStyle);
            GUI.Label(_guiRectLives, _guiContentLives, _guiStyle);
            GUILayout.EndArea();
        }

        public void Init()
        {
            if (this._initialized)
                return;

            Rect screenRect = AspectUtility.screenRect;
            _guiStyle = new GUIStyle();
            _guiStyle.fontSize = (int)(screenRect.width / FONT_SIZE);
            _guiStyle.font = UnityEngine.Resources.Load<Font>("Fonts/Nulshock_free");
            _guiStyle.normal.textColor = new Color(1, 1, 1, 0.9f);
            this.Points = 0;
            this.Lives = 0;
            this._initialized = true;

        }
    }
}