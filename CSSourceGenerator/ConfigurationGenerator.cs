using CSSourceGenerator.Stucts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;

namespace CSSourceGenerator
{
    [Generator]
    public class ConfigurationGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource("ConfigurationAttribute.g.cs", Code.SourceTextUtf8(Code.ConfigurationAttribute));
            });

            var configurationClasses = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => IsConfigurationClass(node),
                transform: static (ctx, _) => TransformConfigurationToModel(ctx))
                .Where(x => x.HasValue)
                .Collect();

            var configurationSections = context.AdditionalTextsProvider
                .Where(static x => x.Path.EndsWith("_appSettings.json"))
                .Select(static (at, _) => TransformConfigurationFileToSection(at))
                .Collect();

            var combinedSource = configurationClasses.Combine(configurationSections);

            context.RegisterSourceOutput(combinedSource, static (context, source) =>
            {
                if (source.Left.IsEmpty)
                    return;

                foreach (ConfigurationToGenerate configurationToGenerate in source.Left)
                {
                    var section = source.Right[0].Single(x => x.SectionName == configurationToGenerate.SectionName);
                    var sourceText = Code.SourceTextUtf8(Code.ConfigurationClass(configurationToGenerate, section));
                    context.AddSource($"{configurationToGenerate.Name}.g.cs", sourceText);
                }
            });

        }

        private static bool IsConfigurationClass(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax classSyntax && classSyntax.AttributeLists.Count > 0;
        }

        private static ConfigurationToGenerate? TransformConfigurationToModel(GeneratorSyntaxContext context)
        {
            if (context.Node is not ClassDeclarationSyntax classSyntax)
                return null;

            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classSyntax);

            if (classSymbol == null)
                return null;

            foreach (var attributeListSyntax in classSyntax.AttributeLists)
            {
                foreach (var attributeSyntax in attributeListSyntax.Attributes)
                {
                    if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol methodSymbol)
                        continue;

                    INamedTypeSymbol attributeSymbol = methodSymbol.ContainingType;

                    if (attributeSymbol.ToDisplayString() == Code.ConfigurationAttributeFullName)
                    {
                        var attributeData = classSymbol.GetAttributes().Where(x => x.AttributeClass?.Name == Code.ConfigurationAttributeName).First();

                        if (attributeData.ConstructorArguments[0].Value is not string sectionName)
                            continue;

                        CustomConversion[] customConversion = [];

                        if (attributeData.ConstructorArguments.Length > 1)
                        {
                            var constructorArgument = attributeData.ConstructorArguments[1];
                            if (constructorArgument.Values != null)
                                customConversion = CustomConversion.FromStringArray(constructorArgument.Values.Where(x => !x.IsNull).Select(x => (string)x.Value)).ToArray();
                        }

                        var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();

                        return new ConfigurationToGenerate(classNamespace, classSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), sectionName, customConversion) ;
                    }
                }
            }

            return null;
        }

        private static IEnumerable<SectionToGenerate> TransformConfigurationFileToSection(AdditionalText additionalText)
        {
            var text = additionalText.GetText().ToString();

            var root = JsonDocument.Parse(text).RootElement;
            var sections = JsonParser.Traverse(root);
            return sections;
        }
    }
}
