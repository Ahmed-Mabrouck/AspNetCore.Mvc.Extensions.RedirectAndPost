using System;
using System.Linq;

namespace AspNetCore.Mvc.Extensions.RedirectAndPost.Helpers
{
    internal static class StringHelpers
    {
        internal static Random random = new Random();
        internal static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        internal static string Join(in string prefix, in string name)
        {
            return prefix is null ? name : $"{prefix}[{name}]";
        }
    }
}
