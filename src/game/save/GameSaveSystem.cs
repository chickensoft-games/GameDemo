namespace GameDemo;

using System.Threading.Tasks;
using Godot;

public interface IGameSaveSystem : ISaveSystem<GameSaveFile> {
}

public class GameSaveSystem : SaveSystem<GameSaveFile>, IGameSaveSystem {
  public GameSaveSystem(
    GameSaveSerializer serializer
  ) : base(
    () => new GameSaveFile(
      Player: default!,
      Map: default!
    ), serializer
  ) {
  }

  protected override Task SaveToDisk(string path, string serializedContents) {
    using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
    file.StoreString(serializedContents);
    return Task.CompletedTask;
  }
}
