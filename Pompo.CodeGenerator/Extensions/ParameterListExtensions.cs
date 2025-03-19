using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pompo.Entities;
using Pompo.Exceptions;
using System.Collections.Generic;
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

        /// <summary>
        /// Returns a list of parameters in the CSharp style with fully qualified type names.
        /// </summary>
        /// <param name="list">A parameter list.</param>
        /// <param name="types">Type list.</param>
        /// <returns>The string representing a list of parameters.</returns>
        /// <exception cref="AmbiguousTypeNameException">In case of the fully qualified type name of a parametr could not be uniquely resolved.</exception>
        public static string ToParameterListStringWithFullTypenames(this SeparatedSyntaxList<ParameterSyntax> list, List<TypeDescription> types) =>
            string.Join(", ", list.Select(p =>
            {
                var typeName = p.Type.ToString();
                var foundTypes = types.Where(t => t.Name == typeName).ToList();
                if (foundTypes.Count == 0)
                    return $"{typeName} {p.Identifier}";
                else if (foundTypes.Count == 1)
                    return $"{foundTypes.First().FullName} {p.Identifier}";
                else
                    throw new AmbiguousTypeNameException(typeName);
            }));
    }
}