namespace PlanningPoker.Core.Enums
{
    public static class GameHubActions
    {
        /// <summary>
        /// CreateSession(string ownerName)
        /// </summary>
        ///
        public static string CreateSession => nameof(CreateSession);

        /// <summary>
        /// JoinSession(string sessionId, string playerName)
        /// </summary>
        public static string JoinSession => nameof(JoinSession);

        /// <summary>
        /// StartSession(string sessionId, string playerName)
        /// </summary>
        public static string StartSession => nameof(StartSession);

        /// <summary>
        /// RestartSession(string sessionId, string playerName)
        /// </summary>
        public static string RestartSession => nameof(RestartSession);

        /// <summary>
        /// StopSession(string sessionId, string playerName)
        /// </summary>
        public static string StopSession => nameof(StopSession);

        /// <summary>
        /// HasSession(string sessionId)
        /// </summary>
        public static string HasSession => nameof(HasSession);

        /// <summary>
        /// SelectCardNumber(string sessionId, string playerName, CardNumber number)
        /// </summary>
        public static string SelectCardNumber => nameof(SelectCardNumber);

        /// <summary>
        /// UpdateDescription(string sessionId, string playerName, string description)
        /// </summary>
        public static string UpdateDescription => nameof(UpdateDescription);
    }
}
