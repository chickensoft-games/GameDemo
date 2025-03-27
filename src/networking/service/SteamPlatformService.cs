using Steamworks;
using System;

public class SteamPlatformService
{
  public event Action<bool>? Initialized;
  public event Action<string>? PersonaNameReceived;

  private bool _initialized = false;


  public void Init() {
    if (!SteamAPI.Init()) {
      Initialized?.Invoke(false);
      return;
    }

    _initialized = true;
    Initialized?.Invoke(true);

    string name = SteamFriends.GetPersonaName();
    PersonaNameReceived?.Invoke(name);
  }

  public void Tick() {
    if (_initialized) {
      SteamAPI.RunCallbacks();
    }
  }

  public void Shutdown() {
    if (_initialized) {
      SteamAPI.Shutdown();
      _initialized = false;
    }
  }
}
