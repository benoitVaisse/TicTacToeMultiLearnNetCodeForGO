using System;
using Unity.Netcode;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;

        private PlayerType _playerType;
        public PlayerType PlayerType => _playerType;


        public class OnClickOnGridPositionEventArgs : EventArgs
        {
            public int X;
            public int Y;
            public PlayerType PlayerType;
        }

        public void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multi game manager");
            }
            Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log($"network id : {NetworkManager.Singleton.LocalClientId}");
            if (NetworkManager.Singleton.LocalClientId == 0)
                _playerType = PlayerType.Cross;
            else if (NetworkManager.Singleton.LocalClientId == 1)
                _playerType = PlayerType.Circle;

        }

        public void ClikedOnGridPosition(int x, int y)
        {
            Debug.Log($"CLiked on position {x}, {y}");
            OnClickOnGridPosition?.Invoke(this, new OnClickOnGridPositionEventArgs() { X = x, Y = y, PlayerType = _playerType });
        }
    }
}
