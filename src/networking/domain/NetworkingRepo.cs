namespace GameDemo;

using Chickensoft.Collections;
using System;

public interface INetworkingRepo : IDisposable
{
  IAutoProp<bool> IsSteamInitialized { get; }
  IAutoProp<string> PersonaName { get; }
}

public class NetworkingRepo : INetworkingRepo
{
  private readonly SteamPlatformService _platform;

  public IAutoProp<bool> IsSteamInitialized => _isSteamInitialized;
  private readonly AutoProp<bool> _isSteamInitialized = new(false);

  public IAutoProp<string> PersonaName => _personaName;
  private readonly AutoProp<string> _personaName = new("");

  public NetworkingRepo(SteamPlatformService platform) {
    _platform = platform;

    _platform.Initialized += (success) => _isSteamInitialized.OnNext(success);
    _platform.PersonaNameReceived += (name) => _personaName.OnNext(name);
  }

  public void Dispose() {
    _isSteamInitialized.Dispose();
    _personaName.Dispose();
  }
}
