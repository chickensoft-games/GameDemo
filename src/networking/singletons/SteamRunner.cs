using GameDemo;
using Godot;

public partial class SteamRunner : Node
{
  public static INetworkingRepo NetworkingRepo { get; private set; } = null!;

  private SteamPlatformService _platformService;

  public SteamRunner(SteamPlatformService platformService, INetworkingRepo networkingRepo) {
    _platformService = platformService;
    NetworkingRepo = networkingRepo;
  }

  public override void _Ready() {
    _platformService.Init();
  }

  public override void _Process(double delta) {
    _platformService.Tick();
  }

  public override void _ExitTree() {
    NetworkingRepo.Dispose();
    _platformService.Shutdown();
  }
}
