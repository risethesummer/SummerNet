using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace SourceGenerator;

public class NetworkMessageIdGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is StructDeclarationSyntax or ClassDeclarationSyntax,
                (ctx, _) => 
                    ctx.GetClassOrStructInheritInherit(GeneratorUtilities.Realtime.Networks.INetworkPayload, true))
            .Where(t => t is not null)
            .Collect();
        // context.RegisterSourceOutput(provider, GenerateCode);
    }
    private void GenerateCode(SourceProductionContext spc, ImmutableArray<INamedTypeSymbol?> networkPayloadTypes)
    {
        
        var ordersBuilder = new StringBuilder();
        for (int i = 0; i < networkPayloadTypes.Length; i++)
        {
            var type = networkPayloadTypes[i];
            if (type is null)
                continue;
            ordersBuilder.Append($@"
            case {type.ToDisplayString()}: 
                return {i};
");
        }
        spc.AddSource("NetworkMessageHelper.g.cs", SourceText.From($@"
// <auto-generated>
namespace Realtime.Networks;
public static class NetworkMessageHelper
{{
    public static uint GetPayloadId<TData>(in TData data) where TData : {GeneratorUtilities.Realtime.Networks.INetworkPayload} {{
        switch (data) {{
            {ordersBuilder}
            default:
                throw new KeyNotFoundException(typeof(TData).FullName);
        }}
    }}
}}
", Encoding.UTF8));
    }
}