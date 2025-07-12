using System;
using Unity.Netcode;
using Debug = UnityEngine.Debug;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        private PlayerType _playerType;
        public PlayerType PlayerType => _playerType;
        private NetworkVariable<PlayerType> _currentPlayanlePlayerType = new();
        public PlayerType CurrentPlayanlePlayerType => _currentPlayanlePlayerType.Value;

        public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;
        public class OnClickOnGridPositionEventArgs : EventArgs
        {
            public int X;
            public int Y;
            public PlayerType PlayerType;
        }

        public event EventHandler OnGameStartedEvent;
        public event EventHandler OnSwitchCurrentPlayablePlayerEvent;

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

            Debug.Log($"Is Server : {IsServer}");
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += NetWorkManager_OnClientConnectedCallback;
            }
            OnCurrentPlayanlePlayerTypeValueChanged();
        }

        private void OnCurrentPlayanlePlayerTypeValueChanged()
        {
            _currentPlayanlePlayerType.OnValueChanged += (PlayerType oldValue, PlayerType newValue) =>
            {
                Debug.Log($"_currentPlayanlePlayerType had changed");
                Debug.Log($"_currentPlayanlePlayerType old value : {oldValue}");
                Debug.Log($"_currentPlayanlePlayerType new value : {newValue}");
                OnSwitchCurrentPlayablePlayerEvent?.Invoke(this, EventArgs.Empty);
            };
        }

        private void NetWorkManager_OnClientConnectedCallback(ulong obj)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                // start game
                Debug.Log("Game Started");
                _currentPlayanlePlayerType.Value = PlayerType.Cross;
                TriggerOnGameStartedEventRpc();
            }
            else
            {
                Debug.Log($"missing {2 - NetworkManager.Singleton.ConnectedClientsList.Count} player(s)");
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerOnGameStartedEventRpc()
        {
            OnGameStartedEvent?.Invoke(this, EventArgs.Empty);
        }

        [Rpc(SendTo.Server)]
        public void ClikedOnGridPositionRpc(int x, int y, PlayerType localPlayerType)
        {
            Debug.Log($"CLiked on position {x}, {y}");
            if (_currentPlayanlePlayerType.Value != localPlayerType)
            {
                Debug.LogWarning($"Is not to {localPlayerType.ToString()} to play");
                return;
            }
            OnClickOnGridPosition?.Invoke(this, new OnClickOnGridPositionEventArgs() { X = x, Y = y, PlayerType = localPlayerType });

            switch (_currentPlayanlePlayerType.Value)
            {
                default:
                case PlayerType.Cross:
                    _currentPlayanlePlayerType.Value = PlayerType.Circle;
                    break;
                case PlayerType.Circle:
                    _currentPlayanlePlayerType.Value = PlayerType.Cross;
                    break;
            }
        }
    }
}
