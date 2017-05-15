using System.Collections;
using System.Text.RegularExpressions;
using Assets.Resources.Scripts.Game;
using UnityEngine;

namespace Assets.Resources.Scripts.Util
{

    public enum DymicLabelPosition
    {
        HORIZONTAL_AND_VERTICAL_CENTERED,
    }

    class DynamicLabel : MonoBehaviour
    {
        private GUIStyle _guiStyle;
        private GUIContent _guiContent;
        private Rect _guiRect;

        private void OnGUI()
        {
            GUILayout.BeginArea(AspectUtility.screenRect);
            GUI.Label(_guiRect, _guiContent, _guiStyle);
            GUILayout.EndArea();
        }

        public void OnDestroy()
        {

        }

        public void Start()
        {

        }

        public IEnumerator FadeIn(float duration)
        {
            float step = duration * 0.01f;
            float n = 0;
            while (n < 1)
            {
                Color c = _guiStyle.normal.textColor;
                _guiStyle.normal.textColor = new Color(c.r, c.g, c.b, n);
                yield return new WaitForSeconds(step);
                n += 0.02f;
            }
        }

        public IEnumerator FadeOut(float duration)
        {
            float step = duration * 0.01f;
            float n = 1.0f;
            while (n > 0)
            {
                Color c = _guiStyle.normal.textColor;
                _guiStyle.normal.textColor = new Color(c.r, c.g, c.b, n);
                yield return new WaitForSeconds(step);
                n -= 0.02f;
            }
        }

        public IEnumerator FadeInAndFadeOut(float duration)
        {
            yield return StartCoroutine(this.FadeIn(duration * 0.3f));
            yield return StartCoroutine(this.FadeOut(duration * 0.7f));
        }

        public IEnumerator SelfDestruct(float after)
        {
            yield return new WaitForSeconds(after);
            Destroy(this.gameObject);
        }

        public void Init(string text, DymicLabelPosition pos, float duration, int fontSize, bool fadeInAndOut, string fontPath, Color? color)
        {
            Rect screenRect = AspectUtility.screenRect;
            this.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(screenRect.width / 2, screenRect.height / 2));
            _guiStyle = new GUIStyle();
            _guiStyle.fontSize = (int)(screenRect.width / fontSize);
            _guiStyle.font = UnityEngine.Resources.Load<Font>(fontPath);
            _guiContent = new GUIContent(text);
            _guiStyle.normal.textColor = color.Value;
            var size = _guiStyle.CalcSize(_guiContent);
            _guiRect = new Rect();

            switch (pos)
            {
                case DymicLabelPosition.HORIZONTAL_AND_VERTICAL_CENTERED:
                    _guiRect.x = (float)(screenRect.width - size.x) / 2;
                    _guiRect.y = (float)(screenRect.height - size.y) / 2;
                    _guiRect.width = (float)(screenRect.width * size.x);
                    _guiRect.height = (float)(screenRect.height * size.y);
                    break;
            }

            this.gameObject.name = string.Format("DynamicLabel {0}", Regex.Replace(text, @"\r\n?|\n", ""));

            if (duration > 0)
                StartCoroutine(this.SelfDestruct(duration));

            if (duration > 0 && fadeInAndOut)
                StartCoroutine(this.FadeInAndFadeOut(duration));
        }

        public static void CreateLabel(string text, DymicLabelPosition pos, float duration = 1f, int fontSize = 5, bool fadeInAndOut = true, string fontPath = "Fonts/Nulshock_free", Color? color = null)
        {
            foreach (var lbl in FindObjectsOfType<DynamicLabel>())
            {
                Destroy(lbl.gameObject);
            }

            if (color == null)
                color = Color.white;
            var obj = new GameObject().AddComponent<DynamicLabel>();
            obj.Init(text, pos, duration, fontSize, fadeInAndOut, fontPath, color);
        }

        public void Update()
        {

        }
    }
}
