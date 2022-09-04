namespace PlanningPoker.Core.Enums
{
    public static class GameHubActions
    {
        public static string CreateSession => nameof(CreateSession);
        public static string JoinSession => nameof(JoinSession);
        public static string StartSession => nameof(StartSession);
        public static string StopSession => nameof(StopSession);
        public static string HasSession => nameof(HasSession);
        public static string SelectCardNumber => nameof(SelectCardNumber);
        public static string UpdateDescription => nameof(UpdateDescription);
    }
}
