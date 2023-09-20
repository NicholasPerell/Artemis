using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class GradientFader : MonoBehaviour
{
    [SerializeField]
    private Gradient gradient;
    [SerializeField]
    [Min(float.Epsilon)]
    private float timeToFade = 1;
    [SerializeField]
    private bool deactivateOnComplete = true, ignoreTimeScale = true;

    public event UnityAction OnComplete;

    [SerializeField]
    private float timer;
    private Image image;

    private void Awake()
    {
        if (!image)
        {
            timer = -1;
            image = GetComponent<Image>();
        }
    }
    
    public void StartFade()
    {
        image ??= GetComponent<Image>();
        timer = 0;
        image.color = gradient.Evaluate(0);
        gameObject.SetActive(true);
    }

    void Update()
    {
        if(timer >= 0)
        {
            if(ignoreTimeScale)
            {
                timer += Time.unscaledDeltaTime;
            }
            else
            {
                timer += Time.deltaTime;
            }

            image.color = gradient.Evaluate(timer / timeToFade);

            if(timer >= timeToFade)
            {
                timer = -1;
                OnComplete?.Invoke();
                gameObject.SetActive(!deactivateOnComplete);
            }
        }
    }
}
