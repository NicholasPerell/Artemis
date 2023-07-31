using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Perell.Artemis.Example.Rituals
{
    [RequireComponent(typeof(Image))]
    public class PossessionDarknessBorderPulsing : PossessableMonoBehaviour
    {
        [SerializeField]
        Image image;
        [SerializeField]
        [Min(float.Epsilon)]
        float timeToChange = 1;
        [SerializeField]
        float pulseSpeed = 1;
        float timer;

        private void OnValidate()
        {
            if(image == null)
            {
                if(!TryGetComponent<Image>(out image))
                {
                    image = gameObject.AddComponent<Image>();
                }
            }
        }

        protected override void OnPossessed(bool isPossessed)
        {
            if(isPossessed)
            {
                image.enabled = true;
            }

            timer = timeToChange;
        }

        private void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                if (IsPossessed)
                {
                    image.pixelsPerUnitMultiplier = Mathf.Lerp(2, 50, timer / timeToChange);
                }
                else
                {
                    image.pixelsPerUnitMultiplier = Mathf.Lerp(50, 2, timer / timeToChange);
                }

                if(timer <= 0)
                {
                    image.enabled = IsPossessed;
                }
            }
            else if(IsPossessed)
            {
                image.pixelsPerUnitMultiplier = Mathf.Lerp(2, 4, (Mathf.Cos(Time.time * pulseSpeed) + 1) * .5f);
            }
        }
    }
}