using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField]
        PlayerController lobbyPlayer;
        [SerializeField]
        Vector3 startLocation;
        [SerializeField]
        LobbyEntrance entrance;
        [SerializeField]
        GradientFader whiteFade, blackFade;
        
        [Space]
        [SerializeField]
        Archer returnDialogue;
        [SerializeField]
        FlagBundle[] relevantFlagBundles;

        public event UnityAction OnRunBegan;

        static LobbyManager instance;
        public static Transform Player { get { return instance.lobbyPlayer.transform; } }
        
        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
            entrance.OnEntered += RespondToEntered;
            whiteFade.OnComplete += RespondToWhiteFadeComplete;
            blackFade.OnComplete += RespondToBlackFadeComplete;

            lobbyPlayer.transform.position = startLocation;
            whiteFade.StartFade();
        }

        private void OnDisable()
        {
            entrance.OnEntered -= RespondToEntered;
        }

        private void RespondToEntered()
        {
            Time.timeScale = 0;
            blackFade.StartFade();
        }

        private void RespondToBlackFadeComplete()
        {
            OnRunBegan?.Invoke();
        }

        private void RespondToWhiteFadeComplete()
        {
            Time.timeScale = 1;
            returnDialogue.AttemptDelivery(relevantFlagBundles);
        }
    }
}