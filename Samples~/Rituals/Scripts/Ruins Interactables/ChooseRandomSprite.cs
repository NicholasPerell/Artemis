using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ChooseRandomSprite : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        Destroy(this);
    }
}
