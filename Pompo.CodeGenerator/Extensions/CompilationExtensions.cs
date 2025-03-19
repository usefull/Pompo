using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Pompo.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Compilation"/>.
    /// </summary>
    internal static class CompilationExtensions
    {
        /// <summary>
        /// Extracts all namespaces from <see cref="Compilation"/> object.
        /// </summary>
        /// <param name="compilation">Compilation object.</param>
        /// <returns>The immutable array of namespace symbols</returns>
        public static ImmutableArray<INamespaceSymbol> GetAllNamespaces(this Compilation compilation)
        {
            HashSet<INamespaceSymbol> seen = new HashSet<INamespaceSymbol>(SymbolEqualityComparer.Default);
            Queue<INamespaceSymbol> visit = new Queue<INamespaceSymbol>();
            visit.Enqueue(compilation.GlobalNamespace);

            do
            {
                INamespaceSymbol search = visit.Dequeue();
                seen.Add(search);

                foreach (INamespaceSymbol space in search.GetNamespaceMembers())
                {
                    if (space == null || seen.Contains(space))
                    {
                        continue;
                    }

                    visit.Enqueue(space);
                }
            } while (visit.Count > 0);

            return seen.ToImmutableArray();
        }
    }
}