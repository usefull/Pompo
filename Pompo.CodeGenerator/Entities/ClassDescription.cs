using Pompo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pompo.Entities
{
    /// <summary>
    /// A description of a class.
    /// </summary>
    internal class ClassDescription
    {
        /// <summary>
        /// Class name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Class namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Class alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Constructor list.
        /// </summary>
        public IList<CtorDescription> Ctors { get; set; }

        /// <summary>
        /// Method list.
        /// </summary>
        public IList<MethodDescription> Methods { get; set; }

        /// <summary>
        /// Source file path.
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Class full name.
        /// </summary>
        public string FullName => $"{Namespace}{(string.IsNullOrWhiteSpace(Namespace) ? string.Empty : ".")}{Name}";

        /// <summary>
        /// Vdlidates the object.
        /// </summary>
        /// <returns>The validation exception list.</returns>
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