using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.InkIntegration.Posing
{
    public class PoseCapturer : MonoBehaviour
    {
        [SerializeField]
        FencePoseSlider slider;

        string folder = "Assets/Ink Example/Art/Duel";
        void Start()
        {
            StartCoroutine(Coroutine());
            IEnumerator Coroutine()
            {
                for (int i = -5; i <= 5; i++)
                {
                    slider.points = i;
                    yield return new WaitForSeconds(1);
                    ScreenCapture.CaptureScreenshot(folder + $"/DuelImageOrder_{Random.Range(100,100000)}.png");
                }
            }
        }
    }
}