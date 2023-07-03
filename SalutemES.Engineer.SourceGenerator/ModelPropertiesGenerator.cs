using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
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
public class ModelPropertiesGenerator : ISourceGenerator
{
    private static string ClassesNamespace => "SalutemES.Engineer.Domain";
    private static string AttributeSearchName => typeof(ModelBySourcePropertiesAttribute).Name.Replace("Attribute", "");
    private static string AttributeIgnoreName => "SkipReflection";

    private static string DefaultBuilder(string Type) => Type.ToLower() switch
    {
        "string" => "\"\"",
        _ => $"default({Type})"
    };

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
                .Select(s => (s.Type.ToString().Replace("?", ""), s.Name))
                .ToList())
            .ToList();

        foreach (var GenClass in MatchedClassesNames)
        {
            StringBuilder generatedCode = new StringBuilder()
                .AppendLine($"#nullable enable\n")
                .AppendLine($"using CommunityToolkit.Mvvm.ComponentModel;")
                .AppendLine($"using {ClassesNamespace};\n")
                .AppendLine($"namespace {MatchedClassesPaths.First().Item2};\n")
                .AppendLine($"public partial class {GenClass.Item1} : ObservableObject")
                .AppendLine("{")
                .AppendLine($"    private {GenClass.Item2}? _base = null;\n")
                .AppendLine($"    public {GenClass.Item1}() {{}}")
                .AppendLine($"    public {GenClass.Item1}(string[] SetterValue) : this(new {GenClass.Item2}(SetterValue)) {{ }}")
                .AppendLine($"    public {GenClass.Item1}({GenClass.Item2} Base) => _base = Base;\n");

            foreach (var GenProps in MatchedSourceSetters.First())
            {
                generatedCode
                    .AppendLine($"    " +
                    $"public Action? On{GenProps.Item2}ChangedAction {{ get; set; }} = null;")

                    ?.AppendLine($"    " +
                    $"private {GenProps.Item1} _{GenProps.Item2.ToLower()} = {DefaultBuilder(GenProps.Item1)};")

                    ?.AppendLine($"    " +
                    $"public {GenProps.Item1} {GenProps.Item2}\n" +
                    $"    {{")

                    ?.AppendLine($"        " +
                    $"get => !IsDefault(_{GenProps.Item2.ToLower()}) ? _{GenProps.Item2.ToLower()} : (_base?.{GenProps.Item2} ?? {DefaultBuilder(GenProps.Item1)}); ")

                    ?.AppendLine($"        " +
                    $"set {{ if (SetProperty(ref _{GenProps.Item2.ToLower()}, value)) On{GenProps.Item2}ChangedAction?.Invoke(); " +
                    $"}}")

                    ?.AppendLine($"    " +
                    $"}}\n");
            }

            MatchedClassesPaths.RemoveAt(0);
            MatchedSourceSetters.RemoveAt(0);

            generatedCode.AppendLine("    private bool IsDefault<T>(T obj) => typeof(T) == typeof(string) ? object.Equals(obj, \"\") : object.Equals(obj, default(T));");
            generatedCode.Append("}");

            context.AddSource($"{GenClass.Item1}.g.cs", generatedCode.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}