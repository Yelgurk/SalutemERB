using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using SalutemES.Engineer.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace SalutemES.Engineer.Application.SourceGenerator;

[AttributeUsage(AttributeTargets.Class)]
public class CopyPropertiesAttribute : Attribute
{
    public Type SourceType { get; set; }

    public CopyPropertiesAttribute(Type sourceType)
    {
        SourceType = sourceType;
    }
}

[Generator]
public class PartialClassGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var compilation = context.Compilation;
        var attributeSymbol = compilation.GetTypeByMetadataName(typeof(CopyPropertiesAttribute).Name);

        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var model = compilation.GetSemanticModel(syntaxTree);

            foreach (var attribute in syntaxTree.GetRoot().DescendantNodes().OfType<AttributeSyntax>())
            {
                var attributeType = model.GetTypeInfo(attribute).Type;
                if (attributeType == null || attributeType.TypeKind != TypeKind.Class || attributeType.Name != typeof(CopyPropertiesAttribute).Name)
                    continue;

                var targetSymbol = model.GetTypeInfo(attribute.ArgumentList!.Arguments.Single().Expression).Type;
                var attributeData = targetSymbol!.GetAttributes().SingleOrDefault(x => x.AttributeClass?.Name == typeof(CopyPropertiesAttribute).Name);

                if (targetSymbol != null && targetSymbol.TypeKind == TypeKind.Class)
                {
                    var namespaceName = targetSymbol.ContainingNamespace.ToDisplayString();
                    var className = targetSymbol.Name;

                    var sb = new StringBuilder();

                    sb.AppendLine($"namespace {namespaceName}");
                    sb.AppendLine($"{{");
                    sb.AppendLine($"  partial class {className}");
                    sb.AppendLine($"  {{");

                    // Генерируем публичные свойства для всех членов класса.
                    foreach (var propertySymbol in targetSymbol.GetMembers().OfType<IPropertySymbol>())
                    {
                        var propertyType = propertySymbol.Type.ToDisplayString();
                        var propertyName = propertySymbol.Name;
                        sb.AppendLine($"    public {propertyType} {propertyName} {{ get; set; }}");
                    }

                    sb.AppendLine($"  }}");
                    sb.AppendLine($"}}");

                    // Добавляем сгенерированный код в исходные файлы компиляции.
                    context.AddSource($"{className}.g.cs", sb.ToString());
                }
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }
}