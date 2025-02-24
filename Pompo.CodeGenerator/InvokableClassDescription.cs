using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Pompo.CodeGenerator
{
    /// <summary>
    /// A description of the class that contains JS invokable methods.
    /// </summary>
    internal class InvokableClassDescription
    {
        /// <summary>
        /// Full class name (including namespace).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// JS invokable method list.
        /// </summary>
        public IList<MethodDeclarationSyntax> Methods { get; set; }
    }
}