using Pompo.Resources;
using System;

namespace Pompo.Exceptions
{
    /// <summary>
    /// Thrown when the fully qualified name of a type could not be uniquely resolved during code generation while constructing a method parameter list.
    /// </summary>
    internal class AmbiguousTypeNameException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typeName">The name of the type that caused the error.</param>
        public AmbiguousTypeNameException(string typeName)
            : base(string.Format(Error.AmbiguousTypeName, typeName))
        { }
    }
}