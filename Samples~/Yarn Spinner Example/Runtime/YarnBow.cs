using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Perell.Artemis.Example.YarnSpinnerIntegration
{
    [RequireComponent(typeof(DialogueRunner))]
    public class YarnBow : Bow<YarnArtemisData>
    {
        [SerializeField]
        protected FlagBundle variableLinkedFlags;

        protected DialogueRunner dialogueRunner;

        protected virtual void Awake()
        {
            OverrideDialogueRunner();
        }

        private void OverrideDialogueRunner()
        {
            dialogueRunner = gameObject.GetComponent<DialogueRunner>();
            dialogueRunner.yarnProject = null;
            dialogueRunner.startAutomatically = false;
            ArtemisYarnVariableStorage variableStorage = gameObject.AddComponent<ArtemisYarnVariableStorage>();
            variableStorage.variableLinkedFlags = variableLinkedFlags;
            dialogueRunner.VariableStorage = variableStorage;
            dialogueRunner.enabled = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            dialogueRunner.onDialogueComplete.AddListener(RespondToDialogueComplete);
        }

        protected virtual void OnDisable()
        {
            dialogueRunner.onDialogueComplete.RemoveListener(RespondToDialogueComplete);
        }

        public override void AbruptEnd()
        {
            StopWithoutEventTriggering();
        }

        public override bool IsBusy()
        {
            return dialogueRunner.IsDialogueRunning;
        }

        public override void Send(YarnArtemisData data)
        {
            dialogueRunner.SetProject(data.project);
            dialogueRunner.StartDialogue(data.node);
        }

        private void RespondToDialogueComplete()
        {
            if (FletcherHasQueue())
            {
                StopWithoutEventTriggering();
            }
            ReportEnd();
        }

        private void StopWithoutEventTriggering()
        {
            if (isActiveAndEnabled) dialogueRunner.onDialogueComplete.RemoveListener(RespondToDialogueComplete);
            dialogueRunner.Stop();
            if (isActiveAndEnabled) dialogueRunner.onDialogueComplete.AddListener(RespondToDialogueComplete);
        }
    }
}