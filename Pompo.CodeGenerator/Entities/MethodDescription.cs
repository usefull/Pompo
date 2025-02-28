using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pompo.Entities
{
    /// <summary>
    /// A description of a class constructor.
    /// </summary>
    internal class MethodDescription : BaseCodeEntityDescription
    {
        /// <summary>
        /// The parameter list.
        /// </summary>
        public SeparatedSyntaxList<ParameterSyntax>? Parameters { get; set; }

        /// <summary>
        /// Transmit name.
        /// </summary>
        public override string TransmitName => $"{( string.IsNullOrWhiteSpace(Alias) ? Name : Alias )}";
    }
}