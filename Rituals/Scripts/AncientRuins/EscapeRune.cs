using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class EscapeRune : MonoBehaviour
    {
        public event UnityAction OnComplete;

        [SerializeField]
        [Min(float.Epsilon)]
        float pauseTime = 1;
        [SerializeField]
        [Min(float.Epsilon)]
        float scaleTime = 1;
        [SerializeField]
        [Min(float.Epsilon)]
        float spinTime = 1;
        [SerializeField]
        [Min(float.Epsilon)]
        float fadeTime = 1;
        [Space]
        [SerializeField]
        RectTransform runes;
        [SerializeField]
        AnimationCurve runesScale;
        [SerializeField]
        [Min(1)]
        int runesTurns = 1;
        [SerializeField]
        Image panel;
        [SerializeField]
        Gradient panelColor;
        [SerializeField]
        Color pauseColor;

        float timer;

        private void OnEnable()
        {
            InitializeScreen();
        }

        private void Update()
        {
            timer += Time.unscaledDeltaTime;
            if (timer < pauseTime)
            {

            }
            else if (timer < pauseTime + scaleTime)
            {
                panel.color = panelColor.Evaluate(0);
                runes.gameObject.SetActive(true);
                runes.transform.localScale = Vector3.one * runesScale.Evaluate((timer - pauseTime) / scaleTime);
            }
            else if (timer < pauseTime + scaleTime + spinTime)
            {
                runes.pivot = Vector2.one * .5f;
                runes.transform.localScale = Vector3.one * runesScale.Evaluate(1);
                runes.localRotation = Quaternion.Euler(0,
                    (runesTurns * 360) * ((timer - pauseTime - scaleTime) / spinTime),
                    0);
            }
            else if (timer < pauseTime + scaleTime + spinTime + fadeTime)
            {
                runes.localRotation = Quaternion.identity;
                panel.color = panelColor.Evaluate((timer - pauseTime - scaleTime - spinTime) / fadeTime);
            }
            else
            {
                panel.color = panelColor.Evaluate(1);
                EndScreen();
            }
        }

        private void InitializeScreen()
        {
            Vector3 pixelPlayerPos = Camera.main.WorldToScreenPoint(AncientRuinsManager.Player.position);
            runes.pivot = new Vector2(pixelPlayerPos.x/Camera.main.pixelWidth, pixelPlayerPos.y/Camera.main.pixelHeight);
    
        panel.gameObject.SetActive(true);
            runes.gameObject.SetActive(false);
            panel.color = pauseColor;
            runes.localRotation = Quaternion.identity;
            runes.transform.localScale = Vector3.one * runesScale.Evaluate(0);
            timer = 0;
        }

        private void EndScreen()
        {
            OnComplete?.Invoke();
        }
    }
}