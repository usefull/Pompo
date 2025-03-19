namespace Pompo.Entities
{
    /// <summary>
    /// Type info.
    /// </summary>
    internal class TypeDescription
    {
        /// <summary>
        /// Full namespace string.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Full type name.
        /// </summary>
        public string FullName => string.IsNullOrWhiteSpace(Namespace) ? Name : $"{Namespace}.{Name}";
    }
}