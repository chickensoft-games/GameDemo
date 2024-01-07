namespace GameDemo.Tests;

using System;
using Chickensoft.GoDotCollections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class InGameUITest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<ILabel> _coinsLabel = default!;
  private Mock<IInGameUILogic> _logic = default!;

  private Logic<InGameUILogic.IState, Func<object, InGameUILogic.IState>,
    InGameUILogic.IState, Action<InGameUILogic.IState?>>.IFakeBinding _binding =
    default!;

  private InGameUI _ui = default!;

  public InGameUITest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();
    _gameRepo = new();
    _coinsLabel = new();
    _logic = new();

    _binding = InGameUILogic.CreateFakeBinding();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
    _logic.Setup(logic => logic.Start());

    _ui = new() {
      CoinsLabel = _coinsLabel.Object, InGameUILogic = _logic.Object
    };

    _ui.FakeDependency(_appRepo.Object);
    _ui.FakeDependency(_gameRepo.Object);
  }

  [Test]
  public void Initializes() {
    _ui.Setup();

    _ui.InGameUILogic.ShouldBeOfType<InGameUILogic>();
  }

  [Test]
  public void OnExitTree() {
    _logic.Reset();
    _logic.Setup(logic => logic.Stop());
    _ui.InGameUIBinding = _binding;

    _ui.OnExitTree();

    _logic.VerifyAll();
  }

  [Test]
  public void NumCoinsCollectedChanged() {
    _ui.OnResolved();

    var numCoinsAtStart = new AutoProp<int>(2);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart).Returns(numCoinsAtStart);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogic.Output.NumCoinsCollectedChanged(1)
    );

    _coinsLabel.VerifyAll();
  }

  [Test]
  public void NumCoinsAtStartChanged() {
    _ui.OnResolved();

    var numCoinsCollected = new AutoProp<int>(1);
    _gameRepo.Setup(repo => repo.NumCoinsCollected).Returns(numCoinsCollected);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogic.Output.NumCoinsAtStartChanged(2)
    );

    _coinsLabel.VerifyAll();
  }
}
