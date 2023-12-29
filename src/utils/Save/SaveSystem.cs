namespace GameDemo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISaveManager<TSaveFile> {
  TSaveFile Save(TSaveFile saveFile);
  void Load(TSaveFile saveFile);
}

public interface ISaveSystem<TSaveFile> {
  ISaveSerializer<TSaveFile> Serializer { get; }

  event Action? SaveStarted;
  event Action? SaveCompleted;

  event Action? LoadStarted;
  event Action<TSaveFile>? LoadCompleted;

  void Manage(ISaveManager<TSaveFile> saveManager);

  void Load(string serializedContents);

  void Save(string path);
}

public abstract class SaveSystem<TSaveFile> : ISaveSystem<TSaveFile> {
  private readonly List<ISaveManager<TSaveFile>> _saveManagers = new();
  private bool _isSaving;

  protected SaveSystem(
    Func<TSaveFile> newSaveFile,
    ISaveSerializer<TSaveFile> serializer
  ) {
    NewSaveFile = newSaveFile;
    Serializer = serializer;
  }

  public Func<TSaveFile> NewSaveFile { get; }

  public ISaveSerializer<TSaveFile> Serializer { get; }
  public event Action? SaveStarted;
  public event Action? SaveCompleted;

  public event Action? LoadStarted;
  public event Action<TSaveFile>? LoadCompleted;

  public void Manage(ISaveManager<TSaveFile> saveManager) =>
    _saveManagers.Add(saveManager);

  public void Load(string serializedContents) {
    var saveFile = Serializer.Deserialize(serializedContents);

    LoadStarted?.Invoke();

    Task.Run(
      () => LoadData(saveFile).ContinueWith(_ => FinishLoad(saveFile))
    );
  }

  public void Save(string path) {
    if (_isSaving) {
      return;
    }

    // run the CreateSaveFile to serialize the game's state,
    // but do it on a dedicated thread to avoid blocking.

    SaveStarted?.Invoke();

    Task.Run(
      () => CreateSaveFile()
        .ContinueWith(task => FinishSave(path, task.Result))
    );
  }

  protected abstract Task SaveToDisk(string path, string serializedContents);

  private Task<TSaveFile> CreateSaveFile() {
    var saveFile = NewSaveFile();

    // Give each saver a chance to save data to the save file.
    foreach (var manager in _saveManagers) {
      saveFile = manager.Save(saveFile);
    }

    return Task.FromResult(saveFile);
  }

  private async Task FinishSave(string path, TSaveFile saveFile) {
    var fileContents = Serializer.Serialize(saveFile);
    await SaveToDisk(path, fileContents);
    _isSaving = false;
    SaveCompleted?.Invoke();
  }

  private Task LoadData(TSaveFile saveFile) {
    // Give each saver a chance to load data from the save file.
    foreach (var manager in _saveManagers) {
      manager.Load(saveFile);
    }

    return Task.FromResult(saveFile);
  }

  private Task FinishLoad(TSaveFile saveFile) {
    LoadCompleted?.Invoke(saveFile);
    return Task.FromResult(saveFile);
  }
}
