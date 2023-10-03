using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Perell.Artemis.Example.InkIntegration.UI;
using System;

namespace Perell.Artemis.Example.InkIntegration
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField]
        MenuPanelsManager menus;
        [SerializeField]
        IntroductionCutsceneDirector introductionCutscene;
        [SerializeField]
        DuelDirector gameplay;

        private void Awake()
        {
            menus.gameObject.SetActive(true);
            introductionCutscene.gameObject.SetActive(false);
            gameplay.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            menus.onPlayRequested += RespondToPlayRequested;
            introductionCutscene.onCutsceneFinished += RespondToCutsceneFinished;
            gameplay.onWin += RespondToWin;
            gameplay.onLose += RespondToLose;
            gameplay.onArcherExhausted += RespondToArcherExhausted;
        }
        private void OnDisable()
        {
            menus.onPlayRequested -= RespondToPlayRequested;
            introductionCutscene.onCutsceneFinished -= RespondToCutsceneFinished;
            gameplay.onWin -= RespondToWin;
            gameplay.onLose -= RespondToLose;
            gameplay.onArcherExhausted -= RespondToArcherExhausted;
        }

        private void RespondToArcherExhausted()
        {
            gameplay.gameObject.SetActive(false);
            menus.gameObject.SetActive(true);
            menus.OpenCoinToss();
        }

        private void RespondToLose()
        {
            gameplay.gameObject.SetActive(false);
            menus.gameObject.SetActive(true);
            menus.OpenLose();
        }

        private void RespondToWin()
        {
            gameplay.gameObject.SetActive(false);
            menus.gameObject.SetActive(true);
            menus.OpenWin();
        }

        private void RespondToCutsceneFinished()
        {
            introductionCutscene.gameObject.SetActive(false);
            gameplay.gameObject.SetActive(true);
        }

        private void RespondToPlayRequested()
        {
            menus.gameObject.SetActive(false);
            introductionCutscene.gameObject.SetActive(true);
        }
    }
}