using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LetterBoxSizing : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;
    Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        img.rectTransform.sizeDelta = text.textBounds.size + Vector3.one * 10; //new Vector2(text.preferredWidth, text.preferredHeight);
    }
}
