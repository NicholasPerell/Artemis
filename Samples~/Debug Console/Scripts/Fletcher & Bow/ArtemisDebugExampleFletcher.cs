using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.DebugConsole
{

    [System.Serializable]
    public struct ArtemisDebugData
    {
        public enum ArtemisDebugMessageType
        {
            DEFAULT,
            ERROR,
            WARNING
        }

        public string message;
        public ArtemisDebugMessageType messageType;
        public float timeSystemIsBusy;
    }

    [CreateAssetMenu(fileName = "New Artemis Debug Fletcher", menuName = "Artemis Examples/Debug Fletcher")]
    public class ArtemisDebugExampleFletcher : Fletcher<ArtemisDebugData>
    {
        protected override bool SetUpDataFromCells(string[] dataToInterpret, out ArtemisDebugData valueDetermined)
        {
            bool success = true;
            valueDetermined = new ArtemisDebugData();

            //Message
            valueDetermined.message = dataToInterpret[0];

            //Message Type
            switch (dataToInterpret[1])
            {
                case "ERROR":
                    valueDetermined.messageType = ArtemisDebugData.ArtemisDebugMessageType.ERROR;
                    break;
                case "WARNING":
                    valueDetermined.messageType = ArtemisDebugData.ArtemisDebugMessageType.WARNING;
                    break;
                case "DEFAULT":
                default:
                    valueDetermined.messageType = ArtemisDebugData.ArtemisDebugMessageType.DEFAULT;
                    break;
            }

            //Time System Is Busy
            if (!float.TryParse(dataToInterpret[2], out valueDetermined.timeSystemIsBusy))
            {
                Debug.LogError("\"Time (s)\" column doesn't have a float value");
                success = false;
            }

            return success;
        }
    }
}
