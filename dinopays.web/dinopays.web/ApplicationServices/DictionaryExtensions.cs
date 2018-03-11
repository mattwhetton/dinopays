using System;
using System.Collections.Generic;
using System.Linq;

namespace dinopays.web.ApplicationServices
{
    public static class DictionaryExtensions
    {
        public static Dictionary<string, string> Stringify<TKey, TValue>(this Dictionary<TKey, TValue> dict)
            where TKey : struct, IConvertible
            where TValue : struct, IConvertible
        {
            return dict.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value.ToString());
        }

        public static Dictionary<TKey, TValue> Enumify<TKey, TValue>(this Dictionary<string, string> dict)
            where TKey : struct, IConvertible
            where TValue : struct, IConvertible
        {
            return dict.ToDictionary(kvp => Enum.Parse<TKey>(kvp.Key), kvp => Enum.Parse<TValue>(kvp.Value));
        }
    }
}
