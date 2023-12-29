namespace GameDemo;

using System.Text.Json;
using Chickensoft.LogicBlocks;

public interface ISaveLogic {
  void Save(Utf8JsonWriter writer, JsonSerializerOptions options);
  void Load(Utf8JsonReader reader, JsonSerializerOptions options);

  void OnSave();
  void OnLoad();
}

public abstract class SaveLogic<TState> : LogicBlock<TState>, ISaveLogic
where TState : class, LogicBlock<TState>.IStateLogic {
  public void Save(Utf8JsonWriter writer, JsonSerializerOptions options) {
    writer.WriteStartObject();
    writer.WritePropertyName("state");
    options.TypeInfoResolver = new LogicBlockStateJsonTypeResolver(LogicSerialization.StateTypesToDerivedTypes);
    JsonSerializer.Serialize(writer, Value, options);
    writer.WriteEndObject();
  }
  public void Load(Utf8JsonReader reader, JsonSerializerOptions options) { }

  public virtual void OnLoad() { }
  public virtual void OnSave() { }
}
