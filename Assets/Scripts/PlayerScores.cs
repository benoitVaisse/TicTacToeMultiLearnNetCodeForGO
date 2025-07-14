using System;
using Unity.Netcode;

namespace TicTacToeMultiLearnNetCodeForGO.Assets.Scripts
{
    public struct PlayerScores : INetworkSerializable, IEquatable<PlayerScores>
    {
        public int CrossPlayerScore;
        public int CirclePlayerScore;

        public bool Equals(PlayerScores other)
        {
            return CrossPlayerScore == other.CrossPlayerScore && CirclePlayerScore == other.CirclePlayerScore;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out CrossPlayerScore);
                reader.ReadValueSafe(out CirclePlayerScore);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(CrossPlayerScore);
                writer.WriteValueSafe(CirclePlayerScore);
            }
        }
    }
}
