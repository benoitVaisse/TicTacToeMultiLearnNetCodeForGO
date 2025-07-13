using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class GridPosition : MonoBehaviour
    {
        [SerializeField]
        private int x;
        [SerializeField]
        private int y;

        public void OnMouseDown()
        {
            Debug.Log("Click!!");
            GameManager.Instance.ClikedOnGridPositionRpc(x, y, GameManager.Instance.PlayerType);
        }
    }
}
