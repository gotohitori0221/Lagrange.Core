using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lagrange.Proto.Generator;

[Generator(LanguageNames.CSharp)]
public partial class ProtoSourceGenerator : IIncrementalGenerator
{
    private const string ProtoPackableAttributeName = "Lagrange.Proto.ProtoPackableAttribute";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var valuesProvider = context.SyntaxProvider.ForAttributeWithMetadataName(ProtoPackableAttributeName,
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => (ContextClass: (ClassDeclarationSyntax)ctx.TargetNode, ctx.SemanticModel))
            .Select((items, token) =>
            {
                var (contextClass, semanticModel) = items;
                var parser = new Parser(contextClass, semanticModel);
                parser.Parse(token);
                return parser;
            });
        
        context.RegisterSourceOutput(valuesProvider, Emit);
    }
    
    private static void Emit(SourceProductionContext context, Parser parser)
    {
        foreach (var diagnostic in parser.Diagnostics) context.ReportDiagnostic(diagnostic);

        if (parser.TypeDeclarations.Count == 0) return;

        var emitter = new Emitter(parser);
        emitter.Emit(context);
    }
}