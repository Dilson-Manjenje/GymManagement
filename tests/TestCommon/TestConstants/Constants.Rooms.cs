
namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Rooms
    {
        public static Guid NewId = Guid.NewGuid();
        public const string Name = "Pipeline Room";
        public const int Capacity = 2;
        public static Guid KickBoxingRoomId = Guid.Parse("16b36cd2-c7f8-4c80-8b37-12ecdba3cc23");
    }
}