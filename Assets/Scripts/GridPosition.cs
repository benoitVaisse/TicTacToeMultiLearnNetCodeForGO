using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public class GridPosition : MonoBehaviour
    {
        [SerializeField]
        private int x;
        [SerializeField]
        private int y;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnMouseDown()
        {
            Debug.Log("Click!!");
            GameManager.Instance.ClikedOnGridPosition(x, y);
        }
    }
}
