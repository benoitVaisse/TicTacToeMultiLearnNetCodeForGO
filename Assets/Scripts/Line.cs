using System.Collections.Generic;
using UnityEngine;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public struct Line
    {
        public List<Vector2Int> ListPositionWinnerVector;
        public Vector2Int CenterGridPosition;
        public RotationLine RotationLine;
    }

    public enum RotationLine : short
    {
        Horizontal = 0,
        Vertical = 90,
        DiagonalA = 45,
        DiagonalB = -45
    }
}
