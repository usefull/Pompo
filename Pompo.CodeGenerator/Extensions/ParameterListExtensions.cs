using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace Pompo.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="SeparatedSyntaxList<ParameterSyntax>"/>.
    /// </summary>
    internal static class ParameterListExtensions
    {
        /// <summary>
        /// Returns a list of parameters in the JS style.
        /// </summary>
        /// <param name="list">A parameter list.</param>
        /// <returns>The string representing a list of parameters in the JS style.</returns>
        public static string ToJsLikeParameterListString(this SeparatedSyntaxList<ParameterSyntax> list) =>
            string.Join(", ", list.Select(p => p.Identifier.Text));
    }
}