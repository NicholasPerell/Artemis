using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Perell.Artemis.Saving;

namespace Perell.Artemis.Example.Rituals
{
    public class StartScreenManager : MonoBehaviour
    {
        [SerializeField]
        Camera mainCamera;

        [SerializeField]
        GameObject[] startScreenContainers;
        [SerializeField]
        GameObject[] creditsScreenContainers;
        [SerializeField]
        GameObject[] introScreenContainers;

        [Space]
        [SerializeField]
        Constellation gameSaveData;
        [SerializeField]
        TextAsset newGameSaveData;
        [SerializeField]
        Archer[] archersToInitOnNewGame;

        public event UnityAction OnGameBegins;

        private void OnEnable()
        {
            mainCamera.transform.position = new Vector3(0, mainCamera.transform.position.y, 0);
            ShowStartScreen();
        }

        private void HideAllScreens()
        {
            foreach (GameObject gameObject in startScreenContainers)
            {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in creditsScreenContainers)
            {
                gameObject.SetActive(false);
            }
            foreach (GameObject gameObject in introScreenContainers)
            {
                gameObject.SetActive(false);
            }
        }

        private void ShowSpecificScreen(GameObject[] containers)
        {
            HideAllScreens();
            foreach (GameObject gameObject in containers)
            {
                gameObject.SetActive(true);
            }
        }

        public void ShowStartScreen()
        {
            ShowSpecificScreen(startScreenContainers);
        }

        public void ShowCreditsScreen()
        {
            ShowSpecificScreen(creditsScreenContainers);
        }

        public void ShowIntroScreen()
        {
            ShowSpecificScreen(introScreenContainers);
        }

        public void NewGame()
        {
            foreach(Archer archer in archersToInitOnNewGame)
            {
                archer.Init();
            }

            if(gameSaveData && newGameSaveData)
            {
                gameSaveData.LoadFromTextAsset(newGameSaveData);
            }

            OnGameBegins?.Invoke();
        }

        public void OnClickQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}