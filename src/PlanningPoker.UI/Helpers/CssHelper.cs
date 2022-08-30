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
            if (classList.Contains(className))
                return;

            Console.WriteLine("AddClass: {0}", className);

            classList.Add(className);
        }

        public static void RemoveClass(this List<string> classList, string className)
        {
            if (!classList.Contains(className))
                return;

            Console.WriteLine("RemoveClass: {0}", className);

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
