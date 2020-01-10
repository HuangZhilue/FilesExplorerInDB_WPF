using System.Text.RegularExpressions;
using static System.String;

namespace FilesExplorerInDB_WPF.Helper
{
    public static class CommandHelper
    {
        private static Regex NameRegex { get; } = new Regex("\\\\|\\/|\\:|\\*|\\?|\\\"|\\<|\\>|\\|");

        public static bool CheckNameIsNullOrWhiteSpace(this string name)
        {
            return IsNullOrWhiteSpace(name);
        }

        public static bool CheckNameIsNameRegex(this string name)
        {
            return !NameRegex.IsMatch(name);
        }
    }
}