namespace Pompo
{
    /// <summary>
    /// The attribute to declare an alias of a class, constructor, or method.
    /// </summary>
    /// <param name="pseudonym">An alias string.</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = false)]
    public class AliasAttribute(string pseudonym) : Attribute
    {
        private readonly string _pseudonym = pseudonym;
    }
}