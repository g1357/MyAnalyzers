using System;
using System.Linq;
using System.Text;

namespace MyAnalyzers
{
    internal static class NamingHelper
    {
        /// <summary>Converts a name to PascalCase.</summary>
        public static string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var parts = SplitIntoWords(name);
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                if (part.Length == 0) continue;
                sb.Append(char.ToUpperInvariant(part[0]));
                sb.Append(part.Substring(1).ToLowerInvariant());
            }
            return sb.Length == 0 ? name : sb.ToString();
        }

        /// <summary>Converts a name to camelCase.</summary>
        public static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var parts = SplitIntoWords(name);
            var sb = new StringBuilder();
            bool first = true;
            foreach (var part in parts)
            {
                if (part.Length == 0) continue;
                if (first)
                {
                    sb.Append(char.ToLowerInvariant(part[0]));
                    sb.Append(part.Substring(1).ToLowerInvariant());
                    first = false;
                }
                else
                {
                    sb.Append(char.ToUpperInvariant(part[0]));
                    sb.Append(part.Substring(1).ToLowerInvariant());
                }
            }
            return sb.Length == 0 ? name : sb.ToString();
        }

        /// <summary>Converts a name to _camelCase (private field convention).</summary>
        public static string ToPrivateFieldCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var stripped = name.TrimStart('_');
            var camel = ToCamelCase(stripped);
            return "_" + camel;
        }

        /// <summary>Converts a name to interface convention: I + PascalCase.</summary>
        public static string ToInterfaceCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            var stripped = name.TrimStart('I', 'i', '_');
            var pascal = ToPascalCase(stripped);
            return "I" + pascal;
        }

        public static bool IsPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return char.IsUpper(name[0]);
        }

        public static bool IsCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            return char.IsLower(name[0]);
        }

        public static bool IsPrivateFieldCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (name.Length < 2) return false;
            return name[0] == '_' && char.IsLower(name[1]);
        }

        public static bool IsInterfaceCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            if (name.Length < 2) return false;
            return name[0] == 'I' && char.IsUpper(name[1]);
        }

        private static string[] SplitIntoWords(string name)
        {
            // Remove leading/trailing underscores
            var stripped = name.Trim('_');
            if (string.IsNullOrEmpty(stripped)) return new[] { name };

            // Split on underscores and hyphens
            var segments = stripped.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);

            // Further split each segment on camelCase/PascalCase boundaries
            var words = segments.SelectMany(SplitOnCaseBoundary).ToArray();
            return words.Length > 0 ? words : new[] { stripped };
        }

        private static string[] SplitOnCaseBoundary(string segment)
        {
            if (string.IsNullOrEmpty(segment)) return Array.Empty<string>();
            var words = new System.Collections.Generic.List<string>();
            var current = new StringBuilder();
            for (int i = 0; i < segment.Length; i++)
            {
                char c = segment[i];
                if (i > 0 && char.IsUpper(c) && !char.IsUpper(segment[i - 1]))
                {
                    if (current.Length > 0) words.Add(current.ToString());
                    current.Clear();
                }
                else if (i > 0 && char.IsUpper(c) && char.IsUpper(segment[i - 1])
                    && i + 1 < segment.Length && char.IsLower(segment[i + 1]))
                {
                    if (current.Length > 0) words.Add(current.ToString());
                    current.Clear();
                }
                current.Append(c);
            }
            if (current.Length > 0) words.Add(current.ToString());
            return words.ToArray();
        }
    }
}
