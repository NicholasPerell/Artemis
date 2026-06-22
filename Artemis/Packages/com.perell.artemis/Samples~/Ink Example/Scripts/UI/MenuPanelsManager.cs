using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.InkIntegration.UI
{
    public class MenuPanelsManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField]
        GameObject main; 
        [SerializeField]
        GameObject credits, win, lose, coinToss;

        public event UnityAction onPlayRequested;

        private void Awake()
        {
            OpenMain();
        }

        private void ClearOutPanels()
        {
            main.SetActive(false);
            credits.SetActive(false);
            win.SetActive(false);
            lose.SetActive(false);
            coinToss.SetActive(false);
        }

        private void OpenPanel(GameObject panel)
        {
            ClearOutPanels();
            panel.SetActive(true);
        }

        public void OpenMain() => OpenPanel(main);
        public void OpenCredits() => OpenPanel(credits);
        public void OpenWin() => OpenPanel(win);
        public void OpenLose() => OpenPanel(lose);
        public void OpenCoinToss() => OpenPanel(coinToss);

        public void RespondToClickPlay()
        {
            onPlayRequested?.Invoke();
        }

        public void RespondToClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}