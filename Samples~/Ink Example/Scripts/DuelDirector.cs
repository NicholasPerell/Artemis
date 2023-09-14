using Perell.Artemis.Generated;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Perell.Artemis.Example.InkIntegration
{
    public class DuelDirector : InkVariableKeeper
    {
        public event UnityAction onWin;
        public event UnityAction onLose;
        public event UnityAction onArcherExhausted;

        [SerializeField]
        Archer archer;
        [SerializeField]
        InkBow bow;
        [SerializeField]
        FlagBundle[] gameFlags;
        [SerializeField]
        [Min(1)]
        float requiredScoreToWin = 10;
        [Space]
        [SerializeField]
        RoomieSetToDefault defaultSettings;
        [Space]
        [SerializeField]
        Image statusDisplay;
        [SerializeField]
        Sprite[] up, down;

        private float score;

        private void OnEnable()
        {
            bow.onReportEnd += RespondToReportEndBow;

            SetToNewGameState();
            AttemptNextArrow();
        }

        private void OnDisable()
        {
            bow.onReportEnd -= RespondToReportEndBow;

            if(bow.IsBusy())
            {
                bow.AbruptEnd();
            }
        }

        private void SetToNewGameState()
        {
            score = 0;

            UpdateDuelImage();

            archer.Init();

            if (bow.IsBusy())
            {
                bow.AbruptEnd();
            }

            defaultSettings.SetToDefaultValues();
        }

        private void AttemptNextArrow()
        {
            if (!archer.AttemptDelivery(gameFlags))
            {
                onArcherExhausted?.Invoke();
            }
        }

        private bool CheckForWinLoss()
        {
            if(score >= requiredScoreToWin)
            {
                onWin?.Invoke();
                return true;
            }
            else if(score <= -requiredScoreToWin)
            {
                onLose?.Invoke();
                return true;
            }

            return false;
        }

        private void RespondToReportEndBow()
        {
            if(!CheckForWinLoss())
            {
                AttemptNextArrow();
            }
        }

        public override void HandleVariableChange(string variableName, Ink.Runtime.Object newValue)
        {
            if(variableName == "score")
            {
                score = float.Parse(newValue.ToString());
                Debug.Log($"Score: {score}");
                UpdateDuelImage();
            }
        }

        private void UpdateDuelImage()
        {
            if (score == 0)
            {
                statusDisplay.sprite = up[0];
            }
            else if (score > 0)
            {
                statusDisplay.sprite = up[(int)(Mathf.Lerp(0, up.Length - 1, score / requiredScoreToWin) + .5f)];
            }
            else
            {
                statusDisplay.sprite = down[(int)(Mathf.Lerp(0, down.Length - 1, -score / requiredScoreToWin) + .5f)];
            }
        }
    }
}