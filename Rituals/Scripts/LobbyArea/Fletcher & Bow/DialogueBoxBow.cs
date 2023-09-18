using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Perell.Artemis.Example.Rituals.Controls;
using Perell.Artemis.Generated;
using System;

namespace Perell.Artemis.Example.Rituals
{
    public class DialogueBoxBow : Bow<DialogueData>
    {
        [System.Serializable]
        public struct SpeakerAttributes
        {
            public DialogueData.Speaker speakerId;
            public string displayName;
            public Sprite displayImage;
            public Color nameColor;
        }

        [SerializeField]
        SpeakerAttributes[] speakerAttributes;

        [SerializeField]
        FlagBundle affectableFlags;

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
            EndLines();
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            inputActions.Narrative.Interact.performed -= RespondToInteractKey;
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
            Debug.Log("Send " + currentLines.Length);
            foreach (DialogueData.LineData line in currentLines)
                Debug.Log("\t" + line.text);

            if (currentLines == null || onLine >= currentLines.Length)
            {
                EndLines();
            }
            else
            {
                Time.timeScale = 0;
                ShowLine();
            }

            DialogueData.FlagChangeData[] flagChanges = data.flagChanges;
            if(flagChanges != null && flagChanges.Length > 0)
            {
                FlagID id;
                Flag flag;
                float value;
                foreach(DialogueData.FlagChangeData change in flagChanges)
                {
                    id = change.GetID();
                    if (id != FlagID.INVALID 
                        && affectableFlags.flagsUsed.TryGetValue(id, out flag)
                        && change.TryGetValue(out value))
                    {
                        flag.SetValue(value);
                    }
                }
            }


#if ENABLE_INPUT_SYSTEM
            inputActions.Narrative.Enable();
            inputActions.Narrative.Interact.performed += RespondToInteractKey;
#endif

            StartCoroutine(AffectArcher(data));
        }

        private IEnumerator AffectArcher(DialogueData data)
        {
            yield return new WaitForEndOfFrame();

            DialogueData.ArcherChangeData[] archerChanges = data.archerChanges;
            if (archerChanges != null && archerChanges.Length > 0)
            {
                foreach (DialogueData.ArcherChangeData change in archerChanges)
                {
                    if (change.dumping)
                    {
                        change.archer.DumpBundle(change.arrowBundle);
                    }
                    else
                    {
                        change.archer.DropBundle(change.arrowBundle);
                    }
                }
            }
        }

        private void ShowLine()
        {
            Debug.Log("ShowLine");

            overallPanel.SetActive(true);
            DialogueData.LineData lineData = currentLines[onLine];
            foreach (SpeakerAttributes attribute in speakerAttributes)
            {
                if (attribute.speakerId == lineData.speaker)
                {
                    bool isSpeaker = attribute.speakerId != DialogueData.Speaker.NARRATION;

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
            Debug.Log("ShowNextLine");

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
            Time.timeScale = 1;
            onLine = 0;
            currentLines = null;
            overallPanel.SetActive(false);


#if ENABLE_INPUT_SYSTEM
            inputActions.Narrative.Interact.performed -= RespondToInteractKey;
            inputActions.Narrative.Disable();
#endif
        }


    }
}
