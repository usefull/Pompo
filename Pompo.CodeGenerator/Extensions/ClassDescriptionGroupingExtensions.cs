using Pompo.Entities;
using Pompo.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pompo.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IGrouping<string, ClassDescription>"/>.
    /// </summary>
    internal static class ClassDescriptionGroupingExtensions
    {
        /// <summary>
        /// Validates a class grouping.
        /// </summary>
        /// <param name="grouping">A class grouping to validate.</param>
        /// <returns>The validation exception list.</returns>
        public static List<Exception> Validate(this IGrouping<string, ClassDescription> grouping)
        {
            // Each class description validation.
            var validationResults = grouping.SelectMany(cd => cd.Validate()).ToList();

            // For partial classes the alias must be uniquely defined.
            if (grouping.GroupBy(c => c.Alias).Where(ag => !string.IsNullOrWhiteSpace(ag.Key)).Count() > 1)
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.MultipleClassAliasDeclarations, grouping.Key)));

            // Each constructor overload must have a unique alias.
            var multipleCtorAlias = grouping.SelectMany(c => c.Ctors).GroupBy(c => c.Alias).FirstOrDefault(g => g.Count() > 1);
            if (multipleCtorAlias != null)
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.OneAliasForSeveralCtors, multipleCtorAlias.Key, grouping.Key)));

            var methods = grouping.SelectMany(c => c.Methods);
            var nonAliased = methods.Where(m => string.IsNullOrWhiteSpace(m.Alias)).Select(m => m.Name);
            var aliases = methods.Where(m => !string.IsNullOrWhiteSpace(m.Alias)).Select(m => m.Alias);

            // Alias must not be the same as non aliased method names.
            var intersect = nonAliased.Intersect(aliases);
            if (intersect.Any())
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.AliasMatchesMethodName, intersect.First(), grouping.Key)));

            // Method aliases mus be unique.
            var multipleMethodAlias = aliases.GroupBy(a => a).FirstOrDefault(g => g.Count() > 1);
            if (multipleMethodAlias != null)
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.OneAliasForSeveralMethods, multipleMethodAlias.Key, grouping.Key)));

            return validationResults;
        }
    }
}