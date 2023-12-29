namespace GameDemo;

using System;
using Chickensoft.GoDotCollections;
using Godot;

public interface IGameRepo : IDisposable {
  /// <summary>Player's position in global coordinates.</summary>
  IAutoProp<Vector3> PlayerGlobalPosition { get; }

  /// <summary>Camera's global transform basis.</summary>
  IAutoProp<Basis> CameraBasis { get; }

  /// <summary>Camera's global forward direction vector.</summary>
  Vector3 GlobalCameraDirection { get; }

  /// <summary>Sets the camera's global transform basis.</summary>
  /// <param name="cameraBasis">Camera global transform basis.</param>
  void SetCameraBasis(Basis cameraBasis);
  /// <summary>Sets the player's global position.</summary>
  /// <param name="playerGlobalPosition">Player's global position in world
  /// coordinates.</param>
  void SetPlayerGlobalPosition(Vector3 playerGlobalPosition);
}

/// <summary>
/// Game repository — stores pure game logic that's not directly related to the
/// game node's overall view.
/// </summary>
public class GameRepo : IGameRepo {
  public IAutoProp<Vector3> PlayerGlobalPosition => _playerGlobalPosition;
  private readonly AutoProp<Vector3> _playerGlobalPosition;

  public IAutoProp<Basis> CameraBasis => _cameraBasis;
  private readonly AutoProp<Basis> _cameraBasis;

  public Vector3 GlobalCameraDirection => -_cameraBasis.Value.Z;

  private bool _disposedValue;

  public GameRepo() {
    _playerGlobalPosition = new AutoProp<Vector3>(Vector3.Zero);
    _cameraBasis = new AutoProp<Basis>(Basis.Identity);
  }

  internal GameRepo(
    AutoProp<Vector3> playerGlobalPosition,
    AutoProp<Basis> cameraBasis
  ) {
    _playerGlobalPosition = playerGlobalPosition;
    _cameraBasis = cameraBasis;
  }

  public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition) =>
      _playerGlobalPosition.OnNext(playerGlobalPosition);

  public void SetCameraBasis(Basis cameraBasis) =>
    _cameraBasis.OnNext(cameraBasis);

  protected void Dispose(bool disposing) {
    if (!_disposedValue) {
      if (disposing) {
        // Dispose managed objects.
        _playerGlobalPosition.OnCompleted();
        _playerGlobalPosition.Dispose();

        _cameraBasis.OnCompleted();
        _cameraBasis.Dispose();
      }

      _disposedValue = true;
    }
  }

  public void Dispose() {
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
