using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Perell.Artemis.Example.Rituals
{
    public class StateChanger : MonoBehaviour
    {
        [SerializeField]
        StartScreenManager startScreen;
        [SerializeField]
        AncientRuinsManager ancientRuins;
        [SerializeField]
        LobbyManager lobby;
        

        private void OnEnable()
        {
            startScreen.OnGameBegins += RespondToGameBegins;
            ancientRuins.OnEscaped += RespondToEscaped;
            lobby.OnRunBegan += RespondToRunBegan;
        }

        private void RespondToRunBegan()
        {
            ShowSpecificSubscene(ancientRuins.gameObject);
        }

        private void RespondToEscaped()
        {
            ShowSpecificSubscene(lobby.gameObject);
        }

        private void RespondToGameBegins()
        {
            ShowSpecificSubscene(ancientRuins.gameObject);
        }

        private void OnDisable()
        {
            startScreen.OnGameBegins -= RespondToGameBegins;
            ancientRuins.OnEscaped -= RespondToEscaped;
            lobby.OnRunBegan -= RespondToRunBegan;
        }

        private void HideAllSubscenes()
        {
            startScreen.gameObject.SetActive(false);
            ancientRuins.gameObject.SetActive(false);
            lobby.gameObject.SetActive(false);
        }

        private void ShowSpecificSubscene(GameObject subScene)
        {
            HideAllSubscenes();
            subScene.SetActive(true);
        }

        
    }
}