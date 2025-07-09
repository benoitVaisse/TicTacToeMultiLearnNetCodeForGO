using Unity.Netcode;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameVisualManager : MonoBehaviour
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
            Transform spawnedCrossTransform = Instantiate(_crossPrebab, GetGrisWorldPosition(e.x, e.y), Quaternion.identity);
            spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
        }

        private Vector2 GetGrisWorldPosition(int x, int y)
            => new Vector2(-GRID_SIZE + (x * GRID_SIZE), -GRID_SIZE + (y * GRID_SIZE));
    }
}
