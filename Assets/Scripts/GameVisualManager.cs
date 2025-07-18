﻿using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameVisualManager : NetworkBehaviour
    {

        private const float GRID_SIZE = 3.5f;
        [SerializeField]
        private Transform _crossPrebab;
        [SerializeField]
        private Transform _circlePrebab;
        [SerializeField]
        private Transform _lineCompletePrefab;

        private List<GameObject> _intantiatePrebabsInGameToReenitialize = new();


        private void Start()
        {
            GameManager.Instance.OnClickOnGridPosition += Gamemanager_OnClickOnGridPosition;
            GameManager.Instance.OnGameWinnerEvent += GameManager_OnGameWinnerEvent;
            GameManager.Instance.OnGameRematchEvent += GameManager_OnGameRematchEvent;
        }

        private void GameManager_OnGameRematchEvent(object sender, System.EventArgs e)
        {
            foreach (GameObject go in _intantiatePrebabsInGameToReenitialize)
            {
                Destroy(go);
            }
            _intantiatePrebabsInGameToReenitialize.Clear();
        }

        private void OnGameRematchEvent()
        {

        }

        private void GameManager_OnGameWinnerEvent(object sender, GameManager.OnGameWinnerEventArguments e)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            Transform line = Instantiate(_lineCompletePrefab,
                                        GetGrisWorldPosition(e.CenterGridWin.x, e.CenterGridWin.y),
                                        Quaternion.Euler(0, 0, (float)e.RotationLine));
            line.GetComponent<NetworkObject>().Spawn(true);
            _intantiatePrebabsInGameToReenitialize.Add(line.gameObject);

        }

        private void Gamemanager_OnClickOnGridPosition(object sender, GameManager.OnClickOnGridPositionEventArgs e)
        {
            SpawnObjectRpc(e.X, e.Y, e.PlayerType);
        }

        [Rpc(SendTo.Server)]
        private void SpawnObjectRpc(int x, int y, PlayerType playerType)
        {
            Transform prefab = _crossPrebab;
            switch (playerType)
            {
                case PlayerType.Cross:
                default:
                    prefab = _crossPrebab;
                    break;
                case PlayerType.Circle:
                    prefab = _circlePrebab;
                    break;
            }
            Transform spawnedCrossTransform = Instantiate(prefab, GetGrisWorldPosition(x, y), quaternion.identity);
            spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
            _intantiatePrebabsInGameToReenitialize.Add(spawnedCrossTransform.gameObject);
        }

        private Vector2 GetGrisWorldPosition(int x, int y)
            => new(-GRID_SIZE + (x * GRID_SIZE), -GRID_SIZE + (y * GRID_SIZE));
    }
}
