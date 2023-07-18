using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E) && IsBusy())
            {
                ShowNextLine();
            }
        }

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
