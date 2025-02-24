using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Pompo
{
    [Generator]
    public class CodeGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// The list of build properties required for source generation and their default values.
        /// </summary>
        private readonly Dictionary<string, string> _defaultProps = new Dictionary<string, string>
        {
            { "build_property.PompoJsWrapperOutputDir", "wwwroot" },
            { "build_property.PompoJsWrapperOutputFile", "_pompo.js" }
        };

        /// <summary>
        /// Current values of build properties.
        /// </summary>
        /// <remarks>Values are set by 
        /// <see cref="SetBuildProps(SourceProductionContext, ImmutableArray{KeyValuePair{string, string}})"/></remarks>
        private Dictionary<string, string> _props;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Extract build property values and save them in the _props.
            var propPipeline = context.AnalyzerConfigOptionsProvider
                .SelectMany(SelectBuildProps)
                .Collect();
            context.RegisterSourceOutput(propPipeline, SetBuildProps);

            // Collect classes that should be accessible from JS and generate the necessary code.
            var pipeline =
                context.SyntaxProvider.CreateSyntaxProvider(
                    ClassDeclarationNodePredicate,
                    (syntax, _) => syntax.Node)
                .Collect();
            context.RegisterSourceOutput(pipeline, Build);
        }

        /// <summary>
        /// Selects build properties needed for generation and sets their values, if they are specified. 
        /// </summary>
        /// <param name="provider">An analyzer options provider.</param>
        /// <param name="cancellationToken">An operation cancellation token.</param>
        /// <returns>Actual build property values.</returns>
        private ImmutableArray<KeyValuePair<string, string>> SelectBuildProps(
            AnalyzerConfigOptionsProvider provider, 
            CancellationToken cancellationToken) =>
            _defaultProps.Select(prop => new KeyValuePair<string, string>(
                prop.Key,
                provider.GlobalOptions.TryGetValue(prop.Key, out var str) ? str : prop.Value
            )).ToImmutableArray();

        /// <summary>
        /// Saves actual build property values in the <see cref="_props/>.
        /// </summary>
        /// <param name="context">A source production context.</param>
        /// <param name="source">Actual build property values to save.</param>
        private void SetBuildProps(SourceProductionContext context, ImmutableArray<KeyValuePair<string, string>> source) =>        
            _props = source.ToDictionary(s => s.Key, s => s.Value);        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private bool ClassDeclarationNodePredicate(SyntaxNode node, CancellationToken cancellationToken) =>
            node is ClassDeclarationSyntax cds &&
            cds.Members.Any(m =>
                m is MethodDeclarationSyntax &&
                m.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)) &&
                m.AttributeLists.Any(al => al.Attributes.Any(a => ((a.Name as IdentifierNameSyntax)?.Identifier.Text ?? string.Empty) == "JSInvokable"))
            );

        private void Build(SourceProductionContext context, ImmutableArray<SyntaxNode> source)
        {
            var ttt = source[0] as ClassDeclarationSyntax;
            var methods = ttt.Members
                .Where(m => m is MethodDeclarationSyntax)
                .Where(m => m.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)))
                .Where(m => m.AttributeLists.Any(al => al.Attributes.Any(a => ((a.Name as IdentifierNameSyntax)?.Identifier.Text ?? string.Empty) == "JSInvokable")))
                ;
            var aaa = methods.First().AttributeLists.SelectMany(al => al.Attributes);

            var attr = (aaa.First().Name as IdentifierNameSyntax).Identifier.Text;
            //SyntaxFactory.Token(SyntaxKind.PublicKeyword)
            ;
//            context.AddSource("PompoTest", @"
//namespace Pompo
//{
//    public static class Test
//    {
//        public static string Report() => $""{DateTime.Now}"";
//    }
//}
//");
//            context.AddSource("js", @"/*
//alert('1235677';)
//*/");
        }
    }
}