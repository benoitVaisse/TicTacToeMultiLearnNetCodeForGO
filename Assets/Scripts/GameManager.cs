using System;
using Unity.Netcode;
using Debug = UnityEngine.Debug;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;

        private PlayerType _playerType;
        public PlayerType PlayerType => _playerType;
        private PlayerType _currentPlayanlePlayerType;


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

            if (IsServer)
                _currentPlayanlePlayerType = PlayerType.Cross;

        }

        [Rpc(SendTo.Server)]
        public void ClikedOnGridPositionRpc(int x, int y, PlayerType localPlayerType)
        {
            Debug.Log($"CLiked on position {x}, {y}");
            if (_currentPlayanlePlayerType != localPlayerType)
            {
                Debug.LogWarning($"Is not to {localPlayerType.ToString()} to play");
                return;
            }
            OnClickOnGridPosition?.Invoke(this, new OnClickOnGridPositionEventArgs() { X = x, Y = y, PlayerType = localPlayerType });

            switch (_currentPlayanlePlayerType)
            {
                default:
                case PlayerType.Cross:
                    _currentPlayanlePlayerType = PlayerType.Circle;
                    break;
                case PlayerType.Circle:
                    _currentPlayanlePlayerType = PlayerType.Cross;
                    break;
            }
        }
    }
}
