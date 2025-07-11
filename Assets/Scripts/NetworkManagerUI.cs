﻿using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField]
        private Button _startHostButton;

        [SerializeField]
        private Button _startClientButton;

        private void Awake()
        {
            _startHostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                Hide();
            });

            _startClientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                Hide();
            });
        }

        private void Hide()
        {
            this.gameObject.SetActive(false);
        }
    }

}
