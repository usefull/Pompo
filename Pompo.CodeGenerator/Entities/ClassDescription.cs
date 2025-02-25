using System.Collections.Generic;

namespace Pompo.Entities
{
    /// <summary>
    /// A description of the class.
    /// </summary>
    internal class ClassDescription
    {
        /// <summary>
        /// Class name.
        /// </summary>
        public string Name { get; set; }

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
    }
}