using System;
using System.Collections.Generic;
using System.Linq;
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
        private NetworkVariable<PlayerScores> _scoreplayer = new NetworkVariable<PlayerScores>();
        public PlayerType CurrentPlayanlePlayerType => _currentPlayanlePlayerType.Value;
        public PlayerScores Scoreplayer => _scoreplayer.Value;
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
        public event EventHandler OnScorePLayerChangeEvent;
        public event EventHandler<OnGameWinnerEventArguments> OnGameWinnerEvent;
        public event EventHandler OnGameRematchEvent;
        public event EventHandler OnGameTieEvent;
        public event EventHandler OnPlaceSoundFXEvent;
        public class OnGameWinnerEventArguments : EventArgs
        {
            public Vector2Int CenterGridWin;
            public RotationLine RotationLine;
            public PlayerType WinnerPlayerType;
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
            OnScorePlayerValuesChanges();
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

        private void OnScorePlayerValuesChanges()
        {
            _scoreplayer.OnValueChanged += (PlayerScores oldValues, PlayerScores newValues) =>
            {
                Debug.Log($"OnScorePlayerValuesChanges have changed");
                OnScorePLayerChangeEvent?.Invoke(this, EventArgs.Empty);
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

            TriggerOnPlaceSoundFXEventRpc();
            OnClickOnGridPosition?.Invoke(this, new OnClickOnGridPositionEventArgs() { X = x, Y = y, PlayerType = localPlayerType });
            TestWinner();

            switch (_currentPlayanlePlayerType.Value)
            {
                case PlayerType.Cross:
                    _currentPlayanlePlayerType.Value = PlayerType.Circle;
                    break;
                case PlayerType.Circle:
                    _currentPlayanlePlayerType.Value = PlayerType.Cross;
                    break;
                default:
                    _currentPlayanlePlayerType.Value = PlayerType.None;
                    break;
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerOnPlaceSoundFXEventRpc()
        {
            OnPlaceSoundFXEvent?.Invoke(this, EventArgs.Empty);
        }

        private void TestWinner()
        {
            foreach (var line in _linesToWin.Select((line, index) => new { line, index }).ToList())
            {
                if (TestWinnerLine(line.line)
                    )
                {
                    Debug.Log("winner");
                    Debug.Log($"TestWinner {_currentPlayanlePlayerType.Value}");
                    Debug.Log($"we change score {_scoreplayer}");
                    var score = _scoreplayer.Value;

                    if (_currentPlayanlePlayerType.Value == PlayerType.Cross)
                        score.CrossPlayerScore++;
                    else if (_currentPlayanlePlayerType.Value == PlayerType.Circle)
                        score.CirclePlayerScore++;

                    _scoreplayer.Value = score; // ✅ Réassignation nécessaire avec un struct

                    TriggerOnGameWinnerEventRpc(line.index, _currentPlayanlePlayerType.Value);
                    _currentPlayanlePlayerType.Value = PlayerType.None;
                    return;
                }

            }

            if (!_playerTypePositions.Cast<PlayerType>().Any(p => p == PlayerType.None))
            {
                TriggerOnGameTieEventRpc();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerOnGameTieEventRpc()
        {
            OnGameTieEvent?.Invoke(this, EventArgs.Empty);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerOnGameWinnerEventRpc(int indexWinnerLine, PlayerType playerType)
        {
            Line line = _linesToWin[indexWinnerLine];
            OnGameWinnerEvent?.Invoke(this, new OnGameWinnerEventArguments()
            {
                CenterGridWin = line.CenterGridPosition,
                RotationLine = line.RotationLine,
                WinnerPlayerType = playerType
            });
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

        [Rpc(SendTo.Server)]
        public void RematchRpc()
        {
            for (int x = 0; x < _playerTypePositions.GetLength(0); x++)
            {
                for (int y = 0; y < _playerTypePositions.GetLength(1); y++)
                {
                    _playerTypePositions[x, y] = PlayerType.None;
                }
            }

            _currentPlayanlePlayerType.Value = PlayerType.Cross;
            TriggerOnRematchEventRpc();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void TriggerOnRematchEventRpc()
        {
            OnGameRematchEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
