using System.Collections.Generic;

namespace Pompo.Entities
{
    /// <summary>
    /// A description of a class.
    /// </summary>
    internal class ClassDescription : BaseCodeEntityDescription
    {
        /// <summary>
        /// Class namespace.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Constructor list.
        /// </summary>
        public IList<MethodDescription> Ctors { get; set; }

        /// <summary>
        /// Method list.
        /// </summary>
        public IList<MethodDescription> Methods { get; set; }

        /// <summary>
        /// Class full name.
        /// </summary>
        public string FullName => $"{Namespace}{(string.IsNullOrWhiteSpace(Namespace) ? string.Empty : ".")}{Name}";

        /// <summary>
        /// Transmit name.
        /// </summary>
        public override string TransmitName => string.IsNullOrWhiteSpace(Alias)
            ? $"{(string.IsNullOrWhiteSpace(Namespace) ? string.Empty : $"{Namespace?.Replace('.', '_')}_")}{Name}"
            : Alias;
    }
}