namespace GameDemo;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Chickensoft.LogicBlocks;

public static class LogicSerialization {
  public static Dictionary<Type, HashSet<Type>> StateTypesToDerivedTypes { get; } = new();

  static LogicSerialization() {
    // Find all the logic block implementations in the codebase.
    foreach (
      var logicType in Introspection.TypesByParent[typeof(LogicBlock<>)]
    ) {
      // Find the state type for the logic block by finding the first type
      // nested inside the logic block type that implements IStateLogic.
      var stateType = Introspection.TypesByContainingType[logicType]
        .First(
          type => Introspection
            .TypesBySupertype[typeof(LogicBlock<>.IStateLogic)]
            .Contains(type)
        );

      var stateTypes = new HashSet<Type> { stateType };
      stateTypes.UnionWith(GetDerivedTypes(stateType));

      StateTypesToDerivedTypes.Add(stateType, stateTypes);
    }
  }

  public static HashSet<Type> GetDerivedTypesForLogicState(Type stateType) {
    return StateTypesToDerivedTypes[stateType];
  }

  private static IEnumerable<Type> GetDerivedTypes(Type type) {
    if (!Introspection.TypesByParent.ContainsKey(type)) {
      yield break;
    }
    foreach (var childType in Introspection.TypesByParent[type]) {
      yield return childType;
      foreach (var descendent in GetDerivedTypes(childType)) {
        yield return descendent;
      }
    }
  }
}

public class LogicBlockStateJsonTypeResolver : DefaultJsonTypeInfoResolver {
  public Dictionary<Type, HashSet<JsonDerivedType>> TypesToDerivedTypes { get; }

  public LogicBlockStateJsonTypeResolver(
    Dictionary<Type, HashSet<Type>> typesToDerivedTypes
  ) {
    TypesToDerivedTypes = typesToDerivedTypes.ToDictionary(
      pair => pair.Key,
      pair => pair.Value.Select(
        derivedType => new JsonDerivedType(derivedType, derivedType.Name)
      ).ToHashSet()
    );
  }

  public override JsonTypeInfo GetTypeInfo(
    Type type,
    JsonSerializerOptions options
  ) {
    var jsonTypeInfo = base.GetTypeInfo(type, options);

    if (TypesToDerivedTypes.TryGetValue(type, out var derivedTypes)) {
      jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions {
        IgnoreUnrecognizedTypeDiscriminators = false,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
      };

      foreach (var derivedType in derivedTypes) {
        jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
      }
    }

    return jsonTypeInfo;
  }
}
