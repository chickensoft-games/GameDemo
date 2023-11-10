namespace GameDemo;

using Chickensoft.GodotNodeInterfaces;
using Chickensoft.PowerUps;
using Godot;
using SuperNodes.Types;

public interface IMap : INode3D {
  /// <summary>Get the number of coins in the world.</summary>
  int GetCoinCount();
}

[SuperNode(typeof(AutoNode))]
public partial class Map : Node3D, IMap {
  public override partial void _Notification(int what);

  #region Nodes
  [Node]
  public INode3D Coins { get; set; } = default!;
  #endregion Nodes

  public int GetCoinCount() => Coins.GetChildCount();
}
