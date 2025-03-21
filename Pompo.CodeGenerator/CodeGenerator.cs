﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    /// <summary>
    /// Generates source code for a object creation factory class, a WebAssemblyHost extension and a JS transmitter.
    /// </summary>
    [Generator]
    public partial class CodeGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// All types info.
        /// </summary>
        private List<TypeDescription> _types;

        /// <summary>
        /// Initializes source code analizer pipelines.
        /// </summary>
        /// <param name="context">Initialization context.</param>
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Extract all types info.
            var typesPipeline = context.CompilationProvider
                .SelectMany(GetTypes)
                .Collect();
            context.RegisterSourceOutput(typesPipeline, SetTypes);

            // Collect classes that should be accessible from JS and generate the necessary code.
            var pipeline =
                context.SyntaxProvider.CreateSyntaxProvider(
                    ClassDeclarationPredicate,
                    ClassInfoExtractor)
                .Collect();
            context.RegisterSourceOutput(pipeline, Build);
        }

        /// <summary>
        /// Extracts types info from <see cref="Compilation"/> object.
        /// </summary>
        /// <param name="compilation">Compilation object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Type symbol enumeration.</returns>
        private IEnumerable<INamedTypeSymbol> GetTypes(Compilation compilation, CancellationToken cancellationToken) =>
            compilation.GetAllNamespaces().SelectMany(ns => ns.GetTypeMembers());

        /// <summary>
        /// Save types info in <see cref="_types"/> propersty.
        /// </summary>
        /// <param name="context">A source production context.</param>
        /// <param name="source">List of all types.</param>
        private void SetTypes(SourceProductionContext context, ImmutableArray<INamedTypeSymbol> source)
        {
            _types = source.Select(t =>
            {
                var ns = t.ContainingNamespace;
                var namespaces = new List<string>();
                while (ns != null)
                {
                    namespaces.Add(ns.Name);
                    ns = ns.ContainingNamespace;
                }

                return new TypeDescription
                {
                    Namespace = string.Join(".", namespaces.Where(n => !string.IsNullOrWhiteSpace(n)).Reverse()),
                    Name = t.Name
                };
            }).ToList();
        }

        /// <summary>
        /// Selection predicat for class declarations whose instances should be accessible from JS.
        /// </summary>
        /// <param name="node">Syntax node.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>true if node is class declaration whose instance should be accessible from JS, otherwise - false.</returns>
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

        /// <summary>
        /// Extracts the class info from the syntax context.
        /// </summary>
        /// <param name="context">Syntax context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The class info or null if syntax context is not a class declaration.</returns>
        private ClassDescription ClassInfoExtractor(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            ClassDescription classDescription = null;

            if (context.Node is ClassDeclarationSyntax cds)
            {
                var ctors = cds.ParameterList != null
                    ? new List<MethodDescription> {new MethodDescription
                    {
                        Alias = cds.GetAlias(),
                        Parameters = cds.ParameterList.Parameters,
                        SourceFilePath = cds.SyntaxTree.FilePath,
                        Name = cds.Identifier.Text
                    }}
                    : new List<MethodDescription>();

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
                            return new MethodDescription
                            {
                                Alias = ctor?.GetAlias(),
                                Parameters = ctor?.ParameterList?.Parameters,
                                SourceFilePath = ctor?.SyntaxTree.FilePath,
                                Name = cds.Identifier.Text
                            };
                        }).Concat(ctors).ToList(),
                    Methods = cds.Members.Where(JsInvokableMethodPredicat)
                        .Select(m =>
                        {
                            var method = m as MethodDeclarationSyntax;
                            var alias = method?.GetAlias();
                            return new MethodDescription
                            {
                                Name = string.IsNullOrWhiteSpace(alias) ? method?.Identifier.Text : alias,
                                Alias = alias,
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
        /// Produces source code.
        /// </summary>
        /// <param name="context">Source production context.</param>
        /// <param name="source">The collection of class declaration info.</param>
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

                if (result.Ctors.Count < 1)
                    result.Ctors.Add(new MethodDescription
                    {
                        Alias = result.Alias,
                        Name = result.Name,
                        SourceFilePath = result.SourceFilePath
                    });

                var nonaliasedCtor = result.Ctors.FirstOrDefault(c => string.IsNullOrWhiteSpace(c.Alias));
                if (nonaliasedCtor != null && !string.IsNullOrWhiteSpace(result.Alias) && !result.Ctors.Any(c => c.Alias == result.Alias))
                    nonaliasedCtor.Alias = result.Alias;

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
                ReportErrors(context, validationResults);
                return;
            }

            if (classes.Count > 0)
            {
                context.AddSource("Factory.Constant", GenerateConstantFactorySourceCode());
                if (classes.Count > 0)
                    context.AddSource("Factory", GenerateFactorySourceCode(classes));
                context.AddSource("Transmit", classes.Count > 0 ? GenerateTransmitterSourceCode(classes) : " ");
                context.AddSource("WebAssemblyHostExtension", classes.Count > 0 ? GenerateWebAssemblyHostExtensionCode() : " ");
            }
        }

        /// <summary>
        /// Reports validation errors.
        /// </summary>
        /// <param name="context">Source production context.</param>
        /// <param name="validationResults">Validation errors.</param>
        private void ReportErrors(SourceProductionContext context, List<Exception> validationResults) =>
            validationResults.ForEach(vr => context.ReportDiagnostic(Diagnostic.Create(
                new DiagnosticDescriptor(
                    "POMPO0001",
                    "Pompo source generator error",
                    vr.Message,
                    "PompoGeneration",
                    DiagnosticSeverity.Error,
                    isEnabledByDefault: true),
                Location.None)));
    }
}