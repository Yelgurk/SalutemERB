using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private static string ClassesNamespace => "SalutemES.Engineer.Domain";
    private static string AttributeSearchName => typeof(ModelBySourcePropertiesAttribute).Name;
    private static string AttributeIgnoreName => "SkipReflection";

    public void Execute(GeneratorExecutionContext context)
    {
        //find all partial classes contains ModelBySourcePropertiesAttribute
        List<ClassDeclarationSyntax> ObservableClasses = context
            .Compilation
            .SyntaxTrees
            .SelectMany(sm => sm.GetRoot().ChildNodes())
            .SelectMany(sm => sm.ChildNodes().OfType<ClassDeclarationSyntax>())
            .Where(w => w.AttributeLists
                .SelectMany(sm => sm.Attributes)
                .Select(s => s.Name.ToString())
                .Contains(AttributeSearchName))
            .Where(w => w.Modifiers.ToString().Contains("partial"))
            .ToList();

        //get full path to file contains matched class + solution name
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

        //get names of heir and inherited classes names (main class name and class name in attribute as arg)
        List<(string, string)> MatchedClassesNames = ObservableClasses
            .Select(s => (
                s.Identifier.Text,
                s.AttributeLists
                .SelectMany(sm => sm.Attributes)
                .Single(w => w.Name.ToString().Contains(AttributeSearchName))
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

        //get public get-set non-attr inherited class properties for next source generation
        List<List<(string, string)>> MatchedSourceSetters = MatchedClassesNames
            .Select(s => context
                .Compilation
                .GetTypeByMetadataName($"{ClassesNamespace}.{s.Item2}")
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.CanBeReferencedByName
                    && !p.IsIndexer
                    && p.SetMethod != null && p.SetMethod.DeclaredAccessibility == Accessibility.Public
                    && p.GetMethod != null && p.GetMethod.DeclaredAccessibility == Accessibility.Public
                    && !p.GetAttributes()
                        .Select(s => s.AttributeClass.Name)
                        .Contains(AttributeIgnoreName))
                .Select(s => (s.Type.ToString(), s.Name))
                .ToList())
            .ToList();

        int genIndex = 0;
        foreach (var GenClass in MatchedClassesNames)
        {
            StringBuilder generatedCode = new StringBuilder()
                .AppendLine($"using CommunityToolkit.Mvvm.ComponentModel;")
                .AppendLine($"using {ClassesNamespace};\n")
                .AppendLine($"namespace {MatchedClassesPaths[genIndex].Item2};\n")
                .AppendLine($"public partial class {GenClass.Item1} : ObservableObject")
                .AppendLine("{")
                .AppendLine($"    private {GenClass.Item2} _base;\n")
                .AppendLine($"    public {GenClass.Item1}() : this(new {GenClass.Item2}()) {{ }}")
                .AppendLine($"    public {GenClass.Item1}(string[] SetterValue) : this(new {GenClass.Item2}(SetterValue)) {{ }}")
                .AppendLine($"    public {GenClass.Item1}({GenClass.Item2} Base) => _base = Base;\n");

            foreach (var GenProps in MatchedSourceSetters[genIndex])
            {
                generatedCode
                    .AppendLine($"    " +
                    $"private {GenProps.Item1} _{GenProps.Item2.ToLower()};")

                    ?.AppendLine($"    " +
                    $"public {GenProps.Item1} {GenProps.Item2} " +
                    $"{{ get => _{GenProps.Item2.ToLower()} ?? _base.{GenProps.Item2} " +
                    $"; set {{ _{GenProps.Item2.ToLower()} = value; OnPropertyChanged(nameof({GenProps.Item2})); " +
                    $"}} }}\n");
            }

            generatedCode.Append("}");

            context.AddSource($"{GenClass.Item1}.g.cs", generatedCode.ToString());
            genIndex++;
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}