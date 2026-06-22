using Perell.Artemis;
using UnityEngine;

public class MonoBoy : MonoBehaviour
{
    [SerializeField]
    InternalSymboler sym;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnValidate()
    {
        sym._logger.Info(nameof(OnValidate));
    }
}
