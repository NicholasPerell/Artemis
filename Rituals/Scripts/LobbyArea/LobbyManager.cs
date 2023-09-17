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

        public event UnityAction OnRunBegan;

        static LobbyManager instance;
        public static Transform Player { get { return instance.lobbyPlayer.transform; } }
        
        private void Awake()
        {
            instance = this;
        }

        private void OnEnable()
        {
            lobbyPlayer.transform.position = startLocation;
            entrance.OnEntered += RespondToEntered;
        }

        private void OnDisable()
        {
            entrance.OnEntered -= RespondToEntered;
        }

        private void RespondToEntered()
        {
            OnRunBegan?.Invoke();
        }
    }
}