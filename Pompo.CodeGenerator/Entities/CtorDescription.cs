using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pompo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pompo.Entities
{
    /// <summary>
    /// A description of a class constructor.
    /// </summary>
    internal class CtorDescription
    {
        /// <summary>
        /// Object alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Source file path in which the object defined.
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// The parameter list.
        /// </summary>
        public SeparatedSyntaxList<ParameterSyntax>? Parameters { get; set; }

        /// <summary>
        /// Validates the object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> Validate()
        {
            try
            {
                Alias.ValidateAlias();
                return Enumerable.Empty<Exception>();
            }
            catch (Exception e)
            {
                e.Data.Add("file", SourceFilePath);
                return new [] { e };
            }
        }
    }
}