namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.Sync.Primitives;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
public class InGameUILogicTest : IDisposable
{
  private readonly InGameUILogic _logic = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly AutoValue<int> _numCoinsCollected = new(0);
  private readonly AutoValue<int> _numCoinsAtStart = new(0);

  public InGameUILogicTest()
  {
    _gameRepo.Setup(repo => repo.NumCoinsCollected).Returns(_numCoinsCollected);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart).Returns(_numCoinsAtStart);

    _logic.Set(_gameRepo.Object);
  }

  public void Dispose()
  {
    _logic.Stop();

    _numCoinsCollected.Dispose();
    _numCoinsAtStart.Dispose();
    GC.SuppressFinalize(this);
  }

  [Fact]
  public void SubscribesToNumCoinsCollected()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in InGameUILogicState.Output.NumCoinsChanged output) => outputs.Add(output)
    );

    _logic.Start<InGameUILogicState>();

    _numCoinsCollected.Value.ShouldBe(0);

    _numCoinsCollected.Value = 5;

    outputs.ShouldContain(new InGameUILogicState.Output.NumCoinsChanged(5, 0));
  }

  [Fact]
  public void SubscribesToNumCoinsAtStart()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.OnOutput(
      (in InGameUILogicState.Output.NumCoinsChanged output) => outputs.Add(output)
    );

    _logic.Start<InGameUILogicState>();

    _numCoinsAtStart.Value.ShouldBe(0);

    _numCoinsAtStart.Value = 10;

    outputs.ShouldContain(new InGameUILogicState.Output.NumCoinsChanged(0, 10));
  }

  [Fact]
  public void OnStopWithoutStartSucceeds()
  {
    var logic = new InGameUILogic();
    logic.OnStop();
  }
}
