using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtemisDebugDeliveryActor : ArtemisDeliveryActor<ArtemisDebugData>
{
    [SerializeField]
    float delayTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        delayTimer = 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        delayTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(delayTimer > 0)
        {
            delayTimer -= Time.deltaTime;
            delayTimer = Mathf.Max(0, delayTimer);
            if(delayTimer == 0)
            {
                ReportEnd();
            }
        }
    }

    public override void Send(ArtemisDebugData data)
    {
        Debug.ClearDeveloperConsole();

        switch (data.messageType)
        {
            case ArtemisDebugData.ArtemisDebugMessageType.DEFAULT:
                Debug.Log(data.message);
                break;
            case ArtemisDebugData.ArtemisDebugMessageType.ERROR:
                Debug.LogError(data.message);
                break;
            case ArtemisDebugData.ArtemisDebugMessageType.WARNING:
                Debug.LogWarning(data.message);
                break;
            default:
                Debug.LogError(name + ": ArtemisDebugData didn't have a valid message type value.");
                break;
        }

        delayTimer = data.timeSystemIsBusy;
    }

    public override bool IsBusy()
    {
        return delayTimer > 0;
    }

    public override void AbruptEnd()
    {
        delayTimer = 0;
    }
}
