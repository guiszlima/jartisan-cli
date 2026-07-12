using System;
using System.Text.RegularExpressions; // 1. ADDED: Required for Regex and GeneratedRegex

namespace Jartisan.Application.Utils // Fixed typo from "Utills" to "Utils"
{
    public static partial class KebabRegex // 2. FIXED: Class name changed
    {
        // The Source Generator will create the implementation of this method at build time (AOT Safe)
        [GeneratedRegex("(?<!^)(?=[A-Z])")]
        private static partial Regex GetKebabRegex(); // 3. FIXED: Method name changed to avoid conflicts

        // Public method for you to call from other parts of the code
        public static string Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            
            // Uses the compiled regex to inject the hyphen and converts everything to lowercase
            return GetKebabRegex().Replace(input, "-").ToLower();
        }
    }
}
