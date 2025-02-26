using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pompo.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Validates a string as alias.
        /// </summary>
        /// <param name="str">A string to validate.</param>
        /// <exception cref="Exceptions.InvalidAliasException">In case of insuccessful validation</exception>
        public static void ValidateAlias(this string str)
        {
            if (CSharpKeywords.Contains(str))
                throw new Exceptions.InvalidAliasException(Resources.Error.AliasMatchesKeyword);

            if (!AliasPattern.IsMatch(str))
                throw new Exceptions.InvalidAliasException(Resources.Error.AliasNotFollowRules);
        }

        private static readonly Regex AliasPattern = new Regex("^[A-Za-z_][A-Za-z0-9_]*$");

        private static readonly string[] CSharpKeywords =
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while"
        };
    }
}