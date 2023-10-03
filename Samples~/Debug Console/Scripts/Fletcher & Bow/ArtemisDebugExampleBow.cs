using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Perell.Artemis.Example.DebugConsole
{
    public class ArtemisDebugExampleBow : Bow<ArtemisDebugData>
    {
        float delayTimer = 0;
        float timeCap = 1;

        [SerializeField]
        TextMeshProUGUI canvasTextBox;
        [SerializeField]
        Image timerFill;

        // Start is called before the first frame update
        void Start()
        {
            delayTimer = 0;
            timerFill.fillAmount = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            delayTimer = 0;
            timerFill.fillAmount = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (delayTimer > 0)
            {
                delayTimer -= Time.deltaTime;
                delayTimer = Mathf.Max(0, delayTimer);
                timerFill.fillAmount = delayTimer / timeCap;
                if (delayTimer == 0)
                {
                    ReportEnd();
                }
            }
        }

        public override void Send(ArtemisDebugData data)
        {
            switch (data.messageType)
            {
                case ArtemisDebugData.ArtemisDebugMessageType.DEFAULT:
                    Debug.Log(data.message);
                    canvasTextBox.text = "";
                    break;
                case ArtemisDebugData.ArtemisDebugMessageType.ERROR:
                    Debug.LogError(data.message);
                    canvasTextBox.text = "<color=red>[!] </color>";
                    break;
                case ArtemisDebugData.ArtemisDebugMessageType.WARNING:
                    Debug.LogWarning(data.message);
                    canvasTextBox.text = "<color=yellow>\u25B2 </color>";
                    break;
                default:
                    canvasTextBox.text = "<s>";
                    Debug.LogError(name + ": ArtemisDebugData didn't have a valid message type value.");
                    break;
            }

            canvasTextBox.text += data.message;

            timeCap = data.timeSystemIsBusy;
            delayTimer = data.timeSystemIsBusy;

            if(timeCap == 0)
            {
                timerFill.fillAmount = 0;
                ReportEnd();
            }
        }

        public override bool IsBusy()
        {
            return delayTimer > 0;
        }

        public override void AbruptEnd()
        {
            delayTimer = 0;
            timerFill.fillAmount = 0;
        }
    }
}