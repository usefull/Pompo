using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pompo.Entities
{
    internal class CtorDescription
    {
        public string Alias { get; set; }

        public SeparatedSyntaxList<ParameterSyntax>? Parameters { get; set; }
    }
}