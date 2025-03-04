using System;

namespace Pompo.Exceptions
{
    /// <summary>
    /// Throws in case of ambiguous use of alias.
    /// </summary>
    internal class InvalidAliasException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">An error message.</param>
        public InvalidAliasException(string message) : base(message) { }
    }
}