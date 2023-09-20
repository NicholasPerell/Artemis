using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDisable : MonoBehaviour
{
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
