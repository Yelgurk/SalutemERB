using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SalutemES.Engineer.SourceGenerator;

[AttributeUsage(AttributeTargets.Class)]
public class ModelBySourcePropertiesAttribute : Attribute
{
    public Type SourceType { get; set; }

    public ModelBySourcePropertiesAttribute(Type SourceType) => this.SourceType = SourceType;
}

[Generator]
public class ModelBySourcePropertiesGenerator : ISourceGenerator
{
    private const string ClassesNamespace = "SalutemES.Engineer.Domain";

    public void Execute(GeneratorExecutionContext context)
    {
        List<ClassDeclarationSyntax> ObservableClasses = context
            .Compilation
            .SyntaxTrees
            .SelectMany(sm => sm.GetRoot().ChildNodes())
            .SelectMany(sm => sm.ChildNodes().OfType<ClassDeclarationSyntax>())
            .Where(w => w.AttributeLists
                .SelectMany(sm => sm.Attributes)
                .Select(s => s.Name.ToString())
                .Contains(typeof(ModelBySourcePropertiesAttribute).Name))
            .Where(w => w.Modifiers.ToString().Contains("partial"))
            .ToList();

        List<(string, string)> MatchedClassesPaths = ObservableClasses
            .Select(s => (
                s.SyntaxTree.FilePath,
                context
                .Compilation
                .GetSemanticModel(s.SyntaxTree)
                .GetDeclaredSymbol(s)
                .ContainingAssembly
                .MetadataName
            ))
            .ToList();

        List<(string, string)> MatchedClassesNames = ObservableClasses
            .Select(s => (
                s.Identifier.Text,
                s.AttributeLists
                .SelectMany(sm => sm.Attributes)
                .Single(w => w.Name.ToString().Contains(typeof(ModelBySourcePropertiesAttribute).Name))
                .ArgumentList
                .Arguments[0]
                .Expression
                .ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .ToList()[0]
                .Identifier
                .Text
            ))
            .ToList();

        List<List<(string, string)>> MatchedSourceSetters = MatchedClassesNames
            .Select(s => context
                .Compilation
                .GetTypeByMetadataName($"{ClassesNamespace}.{s.Item2}")
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.CanBeReferencedByName && !p.IsIndexer)
                .Select(s => (s.Type.ToString(), s.Name))
                .ToList())
            .ToList();
    }

    public void Initialize(GeneratorInitializationContext context) { }
}
