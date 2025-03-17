namespace Pompo
{
    /// <summary>
    /// The attribute to declare an alias of a class or constructor.
    /// </summary>
    /// <param name="pseudonym">An alias string.</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor, AllowMultiple = false)]
    public class PompoAliasAttribute(string pseudonym) : Attribute
    {
        /// <summary>
        /// A type alias string.
        /// </summary>
        public string Pseudonym { get; private set; } = pseudonym;
    }
}