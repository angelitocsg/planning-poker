namespace PlanningPoker.Core.Enums
{
    public static class GameHubActions
    {
        public static string CreateSession => nameof(CreateSession);
        public static string JoinSession => nameof(JoinSession);
        public static string StartSession => nameof(StartSession);
        public static string StopSession => nameof(StopSession);
    }
}
