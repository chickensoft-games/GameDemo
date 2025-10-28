namespace GameDemo;

using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface ICoin : INode3D
{
  ICoinLogic CoinLogic { get; }
}

[Meta(typeof(IAutoNode))]
public partial class Coin : Node3D, ICoin
{
  public override void _Notification(int what) => this.Notify(what);

  #region Nodes

  [Node("%AnimationPlayer")] public IAnimationPlayer AnimationPlayer { get; set; } = default!;
  [Node("%CoinModel")] public INode3D CoinModel { get; set; } = default!;

  #endregion Nodes

  #region Exports

  public double CollectionTimeInSeconds { get; set; } = 1.0f;

  #endregion Exports

  #region State
  [Dependency] public EntityTable EntityTable => this.DependOn<EntityTable>();
  [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

  public ICoinLogic CoinLogic { get; set; } = default!;
  public CoinLogic.Settings Settings { get; set; } = default!;

  public CoinLogic.IBinding CoinBinding { get; set; } = default!;

  #endregion State

  #region PackedScenes

  public static PackedScene CollectorDetector =>
    GD.Load<PackedScene>("res://src/coin/CollectorDetector.tscn");

  #endregion PackedScenes

  public void Setup()
  {
    Settings = new CoinLogic.Settings(CollectionTimeInSeconds);
    CoinLogic = new CoinLogic();

    CoinLogic.Set(this as ICoin);
    CoinLogic.Set(Settings);
    CoinLogic.Set(GameRepo);
    CoinLogic.Save(() => new CoinLogic.Data());
    CoinLogic.Set(EntityTable);
  }

  public void OnReady()
  {
    // We lazily add the area 3D to the scene tree that detects coin collectors
    // (just the player, but could be anything that implements ICoinCollector).
    //
    // Why? Because the Godot editor has a bug with "snap object to floor" that
    // looks at the biggest collision shape inside a node, recursively, even
    // though it shouldn't. And this isn't a collision shape for physics, it's
    // just a collision shape for area detection :P
    var collectorDetector = CollectorDetector.Instantiate<Area3D>();

    collectorDetector.BodyEntered += OnCollectorDetectorBodyEntered;
    AddChild(collectorDetector);
  }

  public void OnResolved()
  {
    EntityTable.Set(Name, this);
    CoinBinding = CoinLogic.Bind();

    CoinBinding
      .When<CoinLogic.State.Collecting>(_ =>
      {
        // We want to start receiving physics ticks so we can orient ourselves
        // toward the entity that's collecting us.
        SetPhysicsProcess(true);
        // We basically turn ourselves into a static body once we're in the
        // process of being collected.
        AnimationPlayer.Play("collect");
      })
      .Handle(
        (in CoinLogic.Output.Move output) =>
          GlobalPosition = output.GlobalPosition
      )
      .Handle(
        // We're done being collected, so we can remove ourselves from the
        // scene tree.
        (in CoinLogic.Output.SelfDestruct output) => QueueFree()
      );

    CoinLogic.Start();
  }

  // This doesn't get called unless we're in the Collecting state, since that's
  // the only state that cares about physics ticks.
  public void OnPhysicsProcess(double delta) =>
    CoinLogic.Input(new CoinLogic.Input.PhysicsProcess(delta, GlobalPosition));

  public void OnCollectorDetectorBodyEntered(Node body)
  {
    if (body is ICoinCollector target)
    {
      // Whenever we come into contact with a coin collector, we begin the
      // collection process.
      CoinLogic.Input(new CoinLogic.Input.StartCollection(target));
    }
  }

  public void OnExitTree()
  {
    CoinLogic.Stop();
    CoinBinding.Dispose();
    EntityTable.Remove(Name);
  }
}
