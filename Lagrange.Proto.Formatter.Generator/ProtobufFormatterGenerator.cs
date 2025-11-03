using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Lagrange.Proto.Formatter.Generator;

[Generator]
public class ProtobufFormatterGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var candidates = context
            .SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) =>
                    (node is ClassDeclarationSyntax || node is RecordDeclarationSyntax)
                    && ((BaseTypeDeclarationSyntax)node).AttributeLists.Count > 0,
                transform: static (ctx, _) =>
                {
                    var decl = (BaseTypeDeclarationSyntax)ctx.Node;
                    var symbol = ctx.SemanticModel.GetDeclaredSymbol(decl) as INamedTypeSymbol;
                    if (symbol == null)
                        return null;

                    foreach (var attr in symbol.GetAttributes())
                    {
                        var name = attr.AttributeClass?.Name;
                        if (name == "ProtoPackableAttribute" || name == "ProtoPackable")
                            return symbol;
                    }

                    return null;
                }
            )
            .Where(s => s != null)
            .Collect();

        context.RegisterSourceOutput(
            candidates,
            static (spc, list) =>
            {
                var types = list.Distinct(SymbolEqualityComparer.Default).ToArray();
                if (types.Length == 0)
                    return;

                var sb = new StringBuilder();
                sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
                sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
                sb.AppendLine();
                sb.AppendLine("namespace Lagrange.Proto.Formatter.Generated");
                sb.AppendLine("{");
                sb.AppendLine(
                    "    public static partial class ProtobufFormatterGeneratedExtensions"
                );
                sb.AppendLine("    {");
                sb.AppendLine(
                    "        public static IMvcBuilder AddGeneratedProtobufFormatters(this IMvcBuilder builder, ProtobufFormatterOptions options)"
                );
                sb.AppendLine("        {");
                sb.AppendLine(
                    "            if (builder == null) throw new System.ArgumentNullException(nameof(builder));"
                );
                sb.AppendLine(
                    "            if (options == null) throw new System.ArgumentNullException(nameof(options));"
                );
                sb.AppendLine();
                sb.AppendLine("            builder.AddMvcOptions(o =>");
                sb.AppendLine("            {");

                foreach (var t in types)
                {
                    if (t != null)
                    {
                        var fq = t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                        if (fq.StartsWith("global::"))
                            fq = fq.Substring("global::".Length);
                        sb.AppendLine(
                            $"                o.InputFormatters.Add(new Lagrange.Proto.Formatter.ProtobufInputFormatter<{fq}>(options));"
                        );
                        sb.AppendLine(
                            $"                o.OutputFormatters.Add(new Lagrange.Proto.Formatter.ProtobufOutputFormatter<{fq}>(options));"
                        );
                    }
                }

                sb.AppendLine("            });");
                sb.AppendLine();
                sb.AppendLine("            return builder;");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                spc.AddSource(
                    "ProtobufFormatterGeneratedExtensions.g.cs",
                    SourceText.From(sb.ToString(), Encoding.UTF8)
                );
            }
        );
    }
}
