using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalutemES.Engineer.SourceGenerator;

[AttributeUsage(AttributeTargets.Class)]
public class ViewModelContextAttribute : Attribute
{
    public Type SourceType { get; set; }

    public ViewModelContextAttribute(Type SourceType) => this.SourceType = SourceType;
}

[Generator]
public class ViewModelBaseGenerator : ISourceGenerator
{
    private static string AttributeSearchName => typeof(ViewModelContextAttribute).Name.Replace("Attribute", "");

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

        foreach (var GenClass in MatchedClassesNames)
        {
            StringBuilder generatedCode = new StringBuilder()
                .AppendLine(
                $$"""
                #nullable enable
                
                using CommunityToolkit.Mvvm.ComponentModel;
                using System.Collections.ObjectModel;
                using SalutemES.Engineer.Infrastructure;
                using SalutemES.Engineer.Infrastructure.DataBase;

                namespace {{MatchedClassesPaths.First().Item2}};

                public partial class {{GenClass.Item1}} : ObservableObject
                {
                    public Action? OnSelectedModelChanged { private get; set; } = null;
                    private {{GenClass.Item2}}? _{{GenClass.Item2.ToLower()}}selected;
                    public {{GenClass.Item2}}? {{GenClass.Item2}}Selected
                    {
                        get => _{{GenClass.Item2.ToLower()}}selected ?? new {{GenClass.Item2}}();
                        set { if (SetProperty(ref _{{GenClass.Item2.ToLower()}}selected, value)) OnSelectedModelChanged?.Invoke(); }
                    }

                    public ObservableCollection<{{GenClass.Item2}}> {{GenClass.Item2}}Collection { get; } = new ObservableCollection<{{GenClass.Item2}}>();

                    public Action? OnFillCollection { private get; set; } = null;
                    public {{GenClass.Item1}} FillCollection(DataBaseRequest Request, params string[] Args)
                    {
                        {{GenClass.Item2}}Collection.Clear();
                        DataBaseApi.ConnectionAvailable()
                        .DoIf(conn => conn.IsSuccess, error => Logger.WriteLine(error.Exception.message))
                        ?.Api.PrepareCommand(Request, Args)
                        .DoIf(prep => prep.IsSuccess, error => Logger.WriteLine(error.Exception.message))
                        ?.Api.ExecuteCommand<List<string[]>>()
                        .DoIf(exec => exec.IsSuccess, error => Logger.WriteLine(error.Exception.message))
                        ?.Api.DataBaseResponse<List<string[]>>()
                        ?.ForEach(cortage => {
                            {{GenClass.Item2}}Collection.Add(new {{GenClass.Item2}}(cortage));
                            if ({{GenClass.Item2}}Selected!.Equals({{GenClass.Item2}}Collection.Last()))
                                {{GenClass.Item2}}Selected = {{GenClass.Item2}}Collection.Last();
                        });

                        OnFillCollection?.Invoke();

                        return this;
                    }
                }
                """);

            MatchedClassesPaths.RemoveAt(0);

            context.AddSource($"{GenClass.Item1}.g.cs", generatedCode.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}