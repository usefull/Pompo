using Pompo.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Pompo.Entities
{
    internal abstract class BaseCodeEntityDescription
    {
        /// <summary>
        /// Entity alias.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Source file path in which the entity defined.
        /// </summary>
        public string SourceFilePath { get; set; }

        /// <summary>
        /// Entity name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Transmit name.
        /// </summary>
        abstract public string TransmitName { get; }

        /// <summary>
        /// Validates the entity.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> Validate()
        {
            try
            {
                Alias?.ValidateAlias();
                return Enumerable.Empty<Exception>();
            }
            catch (Exception e)
            {
                e.Data.Add("file", SourceFilePath);
                return new[] { e };
            }
        }
    }
}