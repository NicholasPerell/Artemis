using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    public class IntroductionCutsceneDirector : MonoBehaviour
    {
        [System.Serializable]
        private struct CutsceneStep
        {
            public Sprite spriteOnScreen;
            public float timeOnScreen;
        }

        [SerializeField]
        CutsceneStep[] instructions;
        [Space]
        [SerializeField]
        Image imageOnScreen;
        [SerializeField]
        float timeToFadeToNextScene = 1;

        int step = 0;
        float timer;

        public event UnityAction onCutsceneFinished;

        private void OnEnable()
        {
            step = 0;
            timer = 0;

            Color opaqueColor = imageOnScreen.color;
            opaqueColor.a = 1;
            imageOnScreen.color = opaqueColor;

            MatchImageToStep();
        }

        void Update()
        {
            if(step < instructions.Length)
            {
                timer += Time.deltaTime;
                if(timer >= instructions[step].timeOnScreen)
                {
                    step++;
                    timer = 0;
                    if (step < instructions.Length)
                    {
                        MatchImageToStep();
                    }
                }
            }
            else if(step == instructions.Length)
            {
                timer += Time.deltaTime;

                if(timeToFadeToNextScene != 0)
                {
                    Color fadingColor = imageOnScreen.color;
                    fadingColor.a = 1 - timer/timeToFadeToNextScene;
                    imageOnScreen.color = fadingColor;
                }

                if (timer >= timeToFadeToNextScene)
                {
                    step++;
                    timer = 0;
                    onCutsceneFinished?.Invoke();
                }
            }
        }

        void MatchImageToStep()
        {
            imageOnScreen.sprite = instructions[step].spriteOnScreen;
        }
    }
}