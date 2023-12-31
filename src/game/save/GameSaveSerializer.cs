namespace GameDemo;

using System.Text.Json;

public interface IGameSaveSerializer : ISaveSerializer<GameSaveFile> {
}

public class GameSaveSerializer : IGameSaveSerializer {
  public string Serialize(GameSaveFile saveFile) =>
    JsonSerializer.Serialize(saveFile);

  public GameSaveFile Deserialize(string fileContents) =>
    JsonSerializer.Deserialize<GameSaveFile>(fileContents)!;
}
