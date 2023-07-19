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
            List<string> generatedEquals = new List<string>();
            StringBuilder generatedCode = new StringBuilder()
                .AppendLine(
                $$"""
                #nullable enable

                using CommunityToolkit.Mvvm.ComponentModel;
                using System.Text.RegularExpressions;
                using {{ClassesNamespace}};

                namespace {{MatchedClassesPaths.First().Item2}};

                public partial class {{GenClass.Item1}} : ObservableObject
                {
                    private static readonly Regex _regex = new Regex("[0-9]*[.]{0,1}[0-9]*");

                    public {{GenClass.Item2}}? Base { get; private set; } = null;

                    public {{GenClass.Item1}}() { }
                    public {{GenClass.Item1}}(string[] SetterValue) : this(new {{GenClass.Item2}}(SetterValue)) { }
                    public {{GenClass.Item1}}({{GenClass.Item2}} Base) => this.Base = Base;

                """
                );

            foreach (var GenProps in MatchedSourceSetters.First())
            {
                generatedCode
                .AppendLine(
                $$"""
                    public Action? On{{GenProps.Item2}}ChangedAction { get; set; } = null;
                    private {{GenProps.Item1}} _{{GenProps.Item2.ToLower()}} = {{DefaultBuilder(GenProps.Item1)}};

                    public bool Numeric{{GenProps.Item2}}Regex { get; set; } = false;
                    public {{GenProps.Item1}} {{GenProps.Item2}}
                    {
                        get => RegexFormatter(Numeric{{GenProps.Item2}}Regex, (!IsDefault(_{{GenProps.Item2.ToLower()}}) ? _{{GenProps.Item2.ToLower()}} : (_{{GenProps.Item2.ToLower()}} = Base?.{{GenProps.Item2}} ?? {{DefaultBuilder(GenProps.Item1)}})));
                        set { if (SetProperty(ref _{{GenProps.Item2.ToLower()}}, RegexFormatter(Numeric{{GenProps.Item2}}Regex, value))) On{{GenProps.Item2}}ChangedAction?.Invoke(); }
                    }

                """
                );

                generatedEquals.Add($"this.{GenProps.Item2} == Model.{GenProps.Item2}");
            }

            MatchedClassesPaths.RemoveAt(0);
            MatchedSourceSetters.RemoveAt(0);

            generatedCode
                .AppendLine(
                $$"""
                    public void ResetByBase()
                    {
                        {{string.Join("!);\n        ", generatedEquals).Replace("== Model", "= SetDefaultOrValue(Base!")}}!);
                    }
                    
                    private T SetDefaultOrValue<T>(T? obj) => obj is not null ? obj! : (typeof(T) == typeof(string) ? (T)Convert.ChangeType("", typeof(T)) : default(T))!;

                    private bool IsDefault<T>(T obj) => typeof(T) == typeof(string) ? object.Equals(obj, "") : object.Equals(obj, default(T));

                    private string RegexFormatter(bool IsOn, string Content) => IsOn ? _regex.Match(Content.Replace(',', '.')).Value : Content;

                    public override bool Equals(object? obj)
                    {
                        if (obj is not null && obj is {{GenClass.Item1}} Model)
                            if ({{string.Join(" && ", generatedEquals)}})
                                return true;

                        return false;
                    }

                    public override int GetHashCode() => base.GetHashCode();
                }
                """
                );

            context.AddSource($"{GenClass.Item1}.g.cs", generatedCode.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}