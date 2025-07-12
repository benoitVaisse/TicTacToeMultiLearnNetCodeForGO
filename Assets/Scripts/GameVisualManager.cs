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


        private void Start()
        {
            GameManager.Instance.OnClickOnGridPosition += Gamemanager_OnClickOnGridPosition;
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
        }

        private Vector2 GetGrisWorldPosition(int x, int y)
            => new(-GRID_SIZE + (x * GRID_SIZE), -GRID_SIZE + (y * GRID_SIZE));
    }
}
