using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerator;

public static class RoslynExtensions
{
    public static bool InheritInterface(this ITypeSymbol symbol, string interfaceName, bool goDeep)
    {
        var interfaces = goDeep ? symbol.AllInterfaces : symbol.Interfaces;
        foreach (var @interface in interfaces)
        {
            if (@interface.ToDisplayString() == interfaceName)
                return true;
        }
        return false;
    }

    private static string FormArguments(ImmutableArray<ITypeSymbol> arguments)
    {
        if (arguments.Length == 0)
            return "";
        if (arguments.Length == 1)
            return arguments[0].ToDisplayString();
        return string.Join(", ", arguments.Select(e => e.ToDisplayString()));
    } 

    public static INamedTypeSymbol? InheritClass(this INamedTypeSymbol symbol, string baseName, int genericCount = 0)
    {
        if (symbol.ToDisplayString() == baseName)
            return symbol;
        var baseType = symbol.BaseType;
        while (baseType is not null)
        {
            if (genericCount == 0 || baseType.TypeParameters.Length == genericCount)
            {
                var compareName = baseName;
                if (genericCount > 0)
                    compareName = $"{baseName}<{FormArguments(baseType.TypeArguments)}>";
                if (baseType.ToDisplayString() == compareName)
                    return baseType;
            }
            baseType = baseType.BaseType;
        }
        return null;
    }

    public static INamedTypeSymbol? GetNamedTypeSymbol(this GeneratorSyntaxContext context)
    {
        return context.SemanticModel.GetDeclaredSymbol(context.Node) as INamedTypeSymbol;
    }
    public static INamedTypeSymbol? GetClassOrStructInheritInherit(this GeneratorSyntaxContext context, string interfaceName, bool goDeep)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(context.Node);
        if (symbol is not INamedTypeSymbol namedTypeSymbol || !namedTypeSymbol.InheritInterface(interfaceName, goDeep))
        {
            return null;
        }
        return namedTypeSymbol;
    }

    public static uint GetBaseTypeSize(this INamedTypeSymbol memberType)
    {
        switch (memberType.SpecialType)
        {
            case SpecialType.System_Enum:
                return 4;
            case SpecialType.System_Boolean:
                return sizeof(bool);
            case SpecialType.System_Char:
                return sizeof(char);
            case SpecialType.System_SByte:
                return sizeof(sbyte);
            case SpecialType.System_Byte:
                return sizeof(byte);
            case SpecialType.System_Int16:
                return sizeof(short);
            case SpecialType.System_UInt16:
                return sizeof(ushort);
            case SpecialType.System_Int32:
                return sizeof(int);
            case SpecialType.System_UInt32:
                return sizeof(uint);
            case SpecialType.System_Int64:
                return sizeof(long);
            case SpecialType.System_UInt64:
                return sizeof(ulong);
            case SpecialType.System_Decimal:
                return sizeof(decimal);
            case SpecialType.System_Single:
                return sizeof(float);
            case SpecialType.System_Double:
                return sizeof(double);
        }
        return 0;
    }
    public static uint RecursiveSize(this INamedTypeSymbol namedTypeSymbol)
    {
        if (!namedTypeSymbol.IsUnmanagedType)
            return default;
        uint size = 0;
        foreach (var memberType in namedTypeSymbol.GetTypeMembers())
        {
            if (memberType.TypeKind == TypeKind.Struct)
            {
                var embedSize = namedTypeSymbol.RecursiveSize();
                if (embedSize == 0)
                    return 0;
                size += embedSize;
            }
            else
                size += memberType.GetBaseTypeSize();
        }
        return size;
    }
}