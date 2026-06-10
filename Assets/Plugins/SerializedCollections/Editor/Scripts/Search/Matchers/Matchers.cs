using System.Collections.Generic;

namespace AYellowpaper.SerializedCollections.Editor.Search
{
    public static class Matchers
    {

        private static readonly List<Matcher> _registeredMatchers = new();

        static Matchers()
        {
            _registeredMatchers.Add(new NumericMatcher());
            _registeredMatchers.Add(new StringMatcher());
            _registeredMatchers.Add(new EnumMatcher());
        }

        public static IEnumerable<Matcher> RegisteredMatchers => _registeredMatchers;

        public static void AddMatcher(Matcher matcher)
        {
            _registeredMatchers.Add(matcher);
        }

        public static bool RemoveMatcher(Matcher matcher)
        {
            return _registeredMatchers.Remove(matcher);
        }

    }
}