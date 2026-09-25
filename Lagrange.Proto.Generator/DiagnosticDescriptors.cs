using Microsoft.CodeAnalysis;

namespace Lagrange.Proto.Generator;

public static class DiagnosticDescriptors
{
    public static DiagnosticDescriptor MustBePartialClass { get; } = new(
        id: "PROTO001",
        title: "ProtoPackable class {0} must be partial class",
        messageFormat: "ProtoPackable class {0} must be partial class",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor DuplicateFieldNumber { get; } = new(
        id: "PROTO002",
        title: "Duplicate field number {0} in class {1}",
        messageFormat: "Duplicate field number {0} in class {1}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor InvalidNumberHandling { get; } = new(
        id: "PROTO003",
        title: "Invalid number handling for field {0} in class {1}",
        messageFormat: "Invalid number handling for field {0} in class {1}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor NestedTypeMustBeProtoPackable { get; } = new(
        id: "PROTO004",
        title: "Nested type {0} contained in {1} must be ProtoPackable",
        messageFormat: "Nested type {0} contained in {1} must be ProtoPackable",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor UnableToGetSymbol { get; } = new(
        id: "PROTO005",
        title: "Unable to get symbol for {0}",
        messageFormat: "Unable to get symbol for {0}",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor MustContainParameterlessConstructor { get; } = new(
        id: "PROTO006",
        title: "ProtoPackable class {0} must contain public parameterless constructor",
        messageFormat: "ProtoPackable class {0} must contain public parameterless constructor",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
    
    public static DiagnosticDescriptor MustNotBeStatic { get; } = new(
        id: "PROTO007",
        title: "ProtoMember field {0} in class {1} must not be static",
        messageFormat: "ProtoMember field {0} in class {1} must not be static",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );

    public static DiagnosticDescriptor InvalidNodesWireType { get; } = new(
        id: "PROTO008",
        title: "The wire type must be explicitly set for field {0} as the wire type for the ProtoNode, ProtoValue, and ProtoArray types is not known at compile time, to set the wire type, use the NodesWireType Property in ProtoMember attribute",
        messageFormat: "The wire type must be explicitly set for field {0} as the wire type for the ProtoNode, ProtoValue, and ProtoArray types is not known at compile time, to set the wire type, use the NodesWireType Property in ProtoMember attribute",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}