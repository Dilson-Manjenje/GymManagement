
namespace TestCommon.TestConstants;

public static partial class Constants
{
    public static class Sessions
    {
        public static Guid NewId = Guid.NewGuid();
        public const string Title = "Session 1";
        public static DateTime StartDate = DateTime.Now;
        public static DateTime EndDate = DateTime.Now.AddHours(2);
    }
}