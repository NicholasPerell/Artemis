using Perell.Artemis.Generated;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.InkIntegration
{
    public class DuelDirector : MonoBehaviour
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
            archer.Init();

            if (bow.IsBusy())
            {
                bow.AbruptEnd();
            }

            //TODO: Reset flag values
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
    }
}