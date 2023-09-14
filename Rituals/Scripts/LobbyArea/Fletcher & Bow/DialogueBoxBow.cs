using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Perell.Artemis.Example.Rituals.Controls;

namespace Perell.Artemis.Example.Rituals
{
    public class DialogueBoxBow : Bow<DialogueData>
    {
        [System.Serializable]
        public struct SpeakerAttributes
        {
            public DialogueData.Speaker speakerID;
            public string displayName;
            public Sprite displayImage;
            public Color nameColor;
        }

        [SerializeField]
        SpeakerAttributes[] speakerAttributes;

        [SerializeField]
        GameObject overallPanel;

        [SerializeField]
        GameObject iconBorder;
        [SerializeField]
        Image iconImage;

        [SerializeField]
        GameObject speakerBorder;
        [SerializeField]
        TextMeshProUGUI speakerTextBox;

        [SerializeField]
        TextMeshProUGUI lineTextBox;

        int onLine = 0;
        DialogueData.LineData[] currentLines;

#if ENABLE_INPUT_SYSTEM
        SorcererInputs inputActions;

        private void Awake()
        {
            inputActions = new SorcererInputs();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();
#if ENABLE_INPUT_SYSTEM
            inputActions.Narrative.Enable();
            inputActions.Narrative.Interact.performed += RespondToInteractKey;
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            inputActions.Narrative.Disable();
#endif
        }


#if ENABLE_INPUT_SYSTEM
        private void RespondToInteractKey(InputAction.CallbackContext context)
        {
            if(IsBusy())
            {
                ShowNextLine();
            }
        }
#else
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && IsBusy())
            {
                ShowNextLine();
            }
        }
#endif

        public override void AbruptEnd()
        {
            EndLines();
        }

        public override bool IsBusy()
        {
            return currentLines != null;
        }

        public override void Send(DialogueData data)
        {
            onLine = 0;
            currentLines = data.lines;
            if (currentLines != null && onLine >= currentLines.Length)
            {
                EndLines();
            }
            else
            {
                ShowLine();
            }
        }

        private void ShowLine()
        {
            overallPanel.SetActive(true);
            DialogueData.LineData lineData = currentLines[onLine];
            foreach (SpeakerAttributes attribute in speakerAttributes)
            {
                if (attribute.speakerID == lineData.speaker)
                {
                    bool isSpeaker = attribute.speakerID != DialogueData.Speaker.NARRATION;

                    iconBorder.SetActive(isSpeaker);
                    iconImage.sprite = attribute.displayImage;

                    speakerBorder.SetActive(isSpeaker);
                    speakerTextBox.text = attribute.displayName;
                    speakerTextBox.color = attribute.nameColor;
                    break;
                }
            }
            lineTextBox.text = lineData.text;
        }

        private void ShowNextLine()
        {
            onLine++;
            if (onLine >= currentLines.Length)
            {
                EndLines();
            }
            else
            {
                ShowLine();
            }
        }

        private void EndLines()
        {
            onLine = 0;
            currentLines = null;
            overallPanel.SetActive(false);
        }
    }
}
