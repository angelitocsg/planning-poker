namespace PlanningPoker.UI.Helpers
{
    public static class CssHelperExtensions
    {
        public static string Join(this List<string> classList)
        {
            if (classList == null)
                return "";

            return string.Join(' ', classList);
        }

        public static void AddClass(this List<string> classList, string className)
        {
            if (classList == null)
                return;

            if (classList.Contains(className))
                return;

            classList.Add(className);
        }

        public static void RemoveClass(this List<string> classList, string className)
        {
            if (classList == null)
                return;

            if (!classList.Contains(className))
                return;

            classList.Remove(className);
        }
    }

    public static class CssHelper
    {
        public static List<string> Create(params string[] cssClasses)
        {
            return new List<string>(cssClasses);
        }
    }
}
