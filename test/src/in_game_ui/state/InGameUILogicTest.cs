namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is disposed in cleanup"
  )
]
public class InGameUILogicTest : TestClass
{
  private InGameUILogic _logic = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private AutoValue<int> _numCoinsCollected = default!;
  private AutoValue<int> _numCoinsAtStart = default!;

  public InGameUILogicTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _logic = new InGameUILogic();
    _gameRepo = new();
    _numCoinsCollected = new AutoValue<int>(0);
    _numCoinsAtStart = new AutoValue<int>(0);

    _gameRepo.Setup(repo => repo.NumCoinsCollected).Returns(_numCoinsCollected);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart).Returns(_numCoinsAtStart);

    _logic.Set(_gameRepo.Object);
  }

  [Test]
  public void Initializes()
  {
    _logic
      .GetInitialState().State
      .ShouldBeAssignableTo<InGameUILogic.State>();
  }

  [Test]
  public void SubscribesToNumCoinsCollected()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.Handle(
      (in InGameUILogic.Output.NumCoinsChanged output) => outputs.Add(output)
    );

    _logic.Start();

    _numCoinsCollected.Value.ShouldBe(0);

    _numCoinsCollected.Value = 5;

    outputs.ShouldContain(new InGameUILogic.Output.NumCoinsChanged(5, 0));
  }

  [Test]
  public void SubscribesToNumCoinsAtStart()
  {
    var outputs = new List<object>();
    using var binding = _logic.Bind();

    binding.Handle(
      (in InGameUILogic.Output.NumCoinsChanged output) => outputs.Add(output)
    );

    _logic.Start();

    _numCoinsAtStart.Value.ShouldBe(0);

    _numCoinsAtStart.Value = 10;

    outputs.ShouldContain(new InGameUILogic.Output.NumCoinsChanged(0, 10));
  }

  [Test]
  public void OnStopWithoutStartSucceeds()
  {
    var logic = new InGameUILogic();
    logic.OnStop();
  }

  [Cleanup]
  public void Cleanup()
  {
    _logic.Stop();

    _numCoinsCollected.Dispose();
    _numCoinsAtStart.Dispose();
  }
}
