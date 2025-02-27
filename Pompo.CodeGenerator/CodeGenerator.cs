using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pompo.Entities;
using Pompo.Exceptions;
using Pompo.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Pompo
{
    [Generator]
    public partial class CodeGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// The list of build properties required for source generation and their default values.
        /// </summary>
        private readonly Dictionary<string, string> _defaultProps = new Dictionary<string, string>
        {
            { "build_property.RootNamespace", string.Empty },
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
            // Extract build property values and save them in the _props field.
            var propPipeline = context.AnalyzerConfigOptionsProvider
                .SelectMany(SelectBuildProps)
                .Collect();
            context.RegisterSourceOutput(propPipeline, SetBuildProps);

            // Collect classes that should be accessible from JS and generate the necessary code.
            var pipeline =
                context.SyntaxProvider.CreateSyntaxProvider(
                    ClassDeclarationPredicate,
                    ClassInfoExtractor)
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
        /// Selects class declarations whose instances should be accessible from JS.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private bool ClassDeclarationPredicate(SyntaxNode node, CancellationToken cancellationToken) =>
            node is ClassDeclarationSyntax cds &&                               // select class declaration
            cds.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)) &&     // with public modifier
            !cds.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)) &&      // without static modifier
            cds.Members.Any(JsInvokableMethodPredicat) &&                       // contains JS invokable methods
            (                                                                   // with at least one public constructor
                !cds.Members.Any(m => m is ConstructorDeclarationSyntax) ||
                cds.Members.Where(m => m is ConstructorDeclarationSyntax)
                        .Any(m => m.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)))
            );

        private ClassDescription ClassInfoExtractor(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            ClassDescription classDescription = null;

            if (context.Node is ClassDeclarationSyntax cds)
            {
                classDescription = new ClassDescription
                {
                    Name = cds.Identifier.Text,
                    Alias = cds.GetAlias(),
                    SourceFilePath = cds.SyntaxTree.FilePath,
                    Namespace = cds.GetNamespace(),
                    Ctors = cds.Members.Where(m => m is ConstructorDeclarationSyntax)
                        .Where(m => m.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)))
                        .Select(m =>
                        {
                            var ctor = m as ConstructorDeclarationSyntax;
                            return new CtorDescription
                            {
                                Alias = ctor?.GetAlias(),
                                Parameters = ctor?.ParameterList?.Parameters,
                                SourceFilePath = ctor?.SyntaxTree.FilePath
                            };
                        }).ToList(),
                    Methods = cds.Members.Where(JsInvokableMethodPredicat)
                        .Select(m =>
                        {
                            var method = m as MethodDeclarationSyntax;
                            return new MethodDescription
                            {
                                Name = method?.Identifier.Text,
                                Alias = method?.GetAlias(),
                                Parameters = method?.ParameterList?.Parameters,
                                SourceFilePath = method?.SyntaxTree.FilePath
                            };
                        }).ToList()
                };
            }

            return classDescription;
        }

        /// <summary>
        /// Selects JS invokable methods.
        /// </summary>
        /// <param name="mds">A class member declaration syntax.</param>
        /// <returns>true if member is public non static JS invokable method, otherwise - false.</returns>
        private bool JsInvokableMethodPredicat(MemberDeclarationSyntax mds) =>
            mds is MethodDeclarationSyntax &&                                 // method declaration
            !mds.Modifiers.Any(mf => mf.IsKind(SyntaxKind.StaticKeyword)) &&  // without static modifier
            mds.Modifiers.Any(mf => mf.IsKind(SyntaxKind.PublicKeyword)) &&   // with public modifier
            mds.AttributeLists.Any(al =>                                      // with JSInvokableAttribute
                al.Attributes.Any(a => ((a.Name as IdentifierNameSyntax)?.Identifier.Text ?? string.Empty) == "JSInvokable")
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="source"></param>
        private void Build(SourceProductionContext context, ImmutableArray<ClassDescription> source)
        {
            // Grouping partial classes descriptions.
            var classGrouping = source.GroupBy(s => s.FullName);

            // Each group validating.
            var validationResults = classGrouping.SelectMany(g => g.Validate()).ToList();

            // Merging partial classes descriptions.
            var classes = classGrouping.Select(g =>
            {
                var result = g.First();
                result.Alias = g.FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Alias))?.Alias;
                result.Ctors = g.SelectMany(i => i.Ctors).ToList();
                result.Methods = g.SelectMany(i => i.Methods).ToList();
                result.UniqueName = string.IsNullOrWhiteSpace(result.Alias) ? result.Name : result.Alias;

                return result;
            }).ToList();

            // Checking class aliases for uniqueness.
            var classAliases = classes.Where(c => !string.IsNullOrWhiteSpace(c.Alias)).GroupBy(c => c.Alias);
            var nonUnique = classAliases.FirstOrDefault(g => g.Count() > 1);
            if (nonUnique != null)
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.NotUniqueClassAlias, nonUnique.Key)));

            // Check if alice matches any nonaliased class name.
            var classNameMatchingAlias = classAliases.Select(a => a.Key).Intersect(classes.Where(c => string.IsNullOrWhiteSpace(c.Alias)).Select(c => c.Name)).FirstOrDefault();
            if (classNameMatchingAlias != null)
                validationResults.Add(new InvalidAliasException(string.Format(Resources.Error.AliasMatchesClassName, classNameMatchingAlias)));

            // Report validation errors if exists and abort generation.
            if (validationResults.Count > 0)
            {
                ReportErrors(validationResults);
                return;
            }

            // Tune unique names.
            foreach (var grouping in classes.Where(c => string.IsNullOrWhiteSpace(c.Alias)).GroupBy(c => c.UniqueName).Where(g => g.Count() > 1))
            {
                var classesToUnique = grouping.Any(c => string.IsNullOrWhiteSpace(c.Namespace))
                    ? grouping.Where(c => !string.IsNullOrWhiteSpace(c.Namespace))
                    : grouping.Skip(1);

                classesToUnique.ToList().ForEach(c => c.UniqueName = c.FullName.Replace('.', '_'));
            }

            context.AddSource("Factory", GenerateFactorySourceCode(classes));
            //context.AddSource("Transmit", GenerateTransmitterSourceCode(classes));

            ;
            //context.ReportDiagnostic(Diagnostic.Create(
            //        new DiagnosticDescriptor(
            //            "POMPO0000",
            //            "An exception was thrown by the StrongInject generator",
            //            "An exception was thrown by the StrongInject generator: '{0}'",
            //            "StrongInject",
            //            DiagnosticSeverity.Error,
            //            isEnabledByDefault: true),
            //        Location.None,
            //        "detailed message"));
        }

        private void ReportErrors(List<Exception> validationResults)
        {
            throw new NotImplementedException();
        }
    }
}