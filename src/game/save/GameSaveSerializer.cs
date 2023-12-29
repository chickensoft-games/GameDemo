namespace GameDemo;

using System.Text.Json;

public class GameSaveSerializer : ISaveSerializer<GameSaveFile> {
  public string Serialize(GameSaveFile saveFile) =>
    JsonSerializer.Serialize(saveFile);

  public GameSaveFile Deserialize(string fileContents) =>
    JsonSerializer.Deserialize<GameSaveFile>(fileContents)!;
}
