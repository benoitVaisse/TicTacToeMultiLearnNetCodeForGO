using System;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event EventHandler<OnClickOnGridPositionEventArgs> OnClickOnGridPosition;
        public class OnClickOnGridPositionEventArgs : EventArgs
        {
            public int x;
            public int y;
        }

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
            OnClickOnGridPosition?.Invoke(this, new OnClickOnGridPositionEventArgs() { x = x, y = y });
        }
    }
}
