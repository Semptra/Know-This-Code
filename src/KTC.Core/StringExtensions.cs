namespace KTL.Core
{
    internal static class StringExtensions
    {
        internal static bool StartsWith(this string s, params string[] values)
        {
            foreach(var value in values)
            {
                if (s.StartsWith(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}