using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Pompo.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="MemberDeclarationSyntax"/>.
    /// </summary>
    internal static class MemberDeclarationSyntaxExtensions
    {
        /// <summary>
        /// Extracts an alias string from the attribute.
        /// </summary>
        /// <param name="member">A member declaration syntax object.</param>
        /// <returns>The alias string or <see cref="null"/> if the attribute is not found in the declaration.</returns>
        /// <remarks>The alias string extracts form the PompoAlias attribute for classes and constructors
        /// or from JSInvokable attribute for methods.</remarks>
        public static string GetAlias(this MemberDeclarationSyntax member)
        {
            var attrName = member is ClassDeclarationSyntax || member is ConstructorDeclarationSyntax
                ? "PompoAlias"
                : "JSInvokable";

            return (member.AttributeLists.SelectMany(al => al.Attributes)
                .FirstOrDefault(a => ((a.Name as IdentifierNameSyntax)?.Identifier.Text ?? string.Empty) == attrName)
                ?.ArgumentList?.Arguments.FirstOrDefault()
                ?.Expression as LiteralExpressionSyntax)?.Token.Value?.ToString()?.Trim();
        }

        /// <summary>
        /// Extracts a namespace for member declarationSyntax
        /// </summary>
        /// <param name="member">A member declaration syntax object.</param>
        /// <returns>The namespace string or <see cref="null"/> if the namespace is not found in the declaration..</returns>
        public static string GetNamespace(this SyntaxNode member)
        {
            var currentNode = member;

            do
            {
                if (currentNode.Parent == null)
                    return null;

                if (currentNode.Parent is BaseNamespaceDeclarationSyntax ns)
                    return ns.Name?.ToString();

                currentNode = currentNode.Parent;
            }
            while (true);
        }
    }
}