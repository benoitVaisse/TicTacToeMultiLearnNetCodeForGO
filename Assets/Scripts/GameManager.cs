using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Multi game manager");
            }
            Instance = this;
        }

        public void ClikedOnGridPosition(int x, int y)
        {
            Debug.Log($"CLiked on position {x}, {y}");
        }
    }
}
