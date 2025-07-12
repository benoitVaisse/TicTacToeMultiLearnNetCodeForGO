using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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
        private PlayerType[,] _playerTypePositions;

        public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;
        public class OnClickOnGridPositionEventArgs : EventArgs
        {
            public int X;
            public int Y;
            public PlayerType PlayerType;
        }

        public event EventHandler OnGameStartedEvent;
        public event EventHandler OnSwitchCurrentPlayablePlayerEvent;
        public event EventHandler<OnGameWinnerEventArguments> OnGameWinnerEvent;
        public class OnGameWinnerEventArguments : EventArgs
        {
            public Vector2Int CenterGridWin;
            public RotationLine RotationLine;
        }

        private List<Line> _linesToWin = new();

        public void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multi game manager");
            }
            Instance = this;
            _playerTypePositions = new PlayerType[3, 3];

            // horizontal
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 0), new(1, 0), new(2, 0) }, CenterGridPosition = new(1, 0), RotationLine = RotationLine.Horizontal });
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 1), new(1, 1), new(2, 1) }, CenterGridPosition = new(1, 1), RotationLine = RotationLine.Horizontal });
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 2), new(1, 2), new(2, 2) }, CenterGridPosition = new(1, 2), RotationLine = RotationLine.Horizontal });
            // vertical
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 0), new(0, 1), new(0, 2) }, CenterGridPosition = new(1, 0), RotationLine = RotationLine.Vertical });
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(1, 0), new(1, 1), new(1, 2) }, CenterGridPosition = new(1, 1), RotationLine = RotationLine.Vertical });
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(2, 0), new(2, 1), new(2, 2) }, CenterGridPosition = new(2, 1), RotationLine = RotationLine.Vertical });
            // diagonal
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 0), new(1, 1), new(2, 2) }, CenterGridPosition = new(1, 1), RotationLine = RotationLine.DiagonalA });
            _linesToWin.Add(new() { ListPositionWinnerVector = new() { new(0, 2), new(1, 1), new(2, 0) }, CenterGridPosition = new(1, 1), RotationLine = RotationLine.DiagonalB });
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

            if (_playerTypePositions[x, y] != PlayerType.None)
                return;

            _playerTypePositions[x, y] = localPlayerType;

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
            TestWinner();
        }

        private void TestWinner()
        {
            foreach (var line in _linesToWin)
            {
                if (TestWinnerLine(line)
                    )
                {
                    Debug.Log("winner");
                    OnGameWinnerEvent?.Invoke(this, new OnGameWinnerEventArguments()
                    {
                        CenterGridWin = line.CenterGridPosition,
                        RotationLine = line.RotationLine,
                    });
                }

            }
        }

        private bool TestWinnerLine(Line line)
        {
            return TestWinnerLine(
                    _playerTypePositions[line.ListPositionWinnerVector[0].x, line.ListPositionWinnerVector[0].y],
                    _playerTypePositions[line.ListPositionWinnerVector[1].x, line.ListPositionWinnerVector[1].y],
                    _playerTypePositions[line.ListPositionWinnerVector[2].x, line.ListPositionWinnerVector[2].y]
                );
        }

        private bool TestWinnerLine(PlayerType playerTypePos1, PlayerType playerTypePos2, PlayerType playerTypePos3)
        {
            return playerTypePos1 != PlayerType.None &&
                playerTypePos1 == playerTypePos2 &&
                playerTypePos2 == playerTypePos3;
        }
    }
}
