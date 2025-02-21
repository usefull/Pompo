using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Pompo
{
    [Generator]
    public class CodeGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<ImmutableArray<SyntaxToken>> pipeline =
                context.SyntaxProvider.CreateSyntaxProvider(
                    (node, _) => node is ClassDeclarationSyntax,
                    (syntax, _) => ((ClassDeclarationSyntax)syntax.Node).Identifier)
                .Collect();

            context.RegisterSourceOutput(pipeline, Build);
        }

        private void Build(SourceProductionContext context, ImmutableArray<SyntaxToken> source)
        {
            context.AddSource("PompoTest", @"
namespace Pompo
{
    public static class Test
    {
        public static string Report() => $""{DateTime.Now}"";
    }
}
");
            context.AddSource("js", @"/*
alert('1235677';)
*/");
        }
    }
}