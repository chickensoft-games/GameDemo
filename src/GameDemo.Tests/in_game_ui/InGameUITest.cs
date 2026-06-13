namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
[Collection(Constants.HEADLESS)]
public class InGameUITest
{
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly Mock<IGameRepo> _gameRepo = new();
  private readonly Mock<ILabel> _coinsLabel = new();
  private readonly Mock<IInGameUILogic> _logic = new();
  private readonly LogicBlock.FakeBinding _binding = LogicBlock.CreateFakeBinding();

  private readonly InGameUI _ui;

  public InGameUITest()
  {
    _logic.Setup(logic => logic.Bind()).Returns(_binding);
    _logic.Setup(logic => logic.Start<InGameUILogicState>(true));

    _ui = new()
    {
      CoinsLabel = _coinsLabel.Object,
      InGameUILogic = _logic.Object
    };

    _ui.FakeDependency(_appRepo.Object);
    _ui.FakeDependency(_gameRepo.Object);

    _ui._Notification(-1);
  }

  [Fact]
  public void Initializes()
  {
    _ui.Setup();

    _ui.InGameUILogic.ShouldBeOfType<InGameUILogic>();
  }

  [Fact]
  public void OnExitTree()
  {
    _logic.Reset();
    _logic.Setup(logic => logic.Stop());
    _ui.InGameUIBinding = _binding;

    _ui.OnExitTree();

    _logic.VerifyAll();
  }

  [Fact]
  public void NumCoinsCollectedChanged()
  {
    _ui.OnResolved();

    var numCoinsAtStart = new AutoValue<int>(2);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart).Returns(numCoinsAtStart);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogicState.Output.NumCoinsChanged(1, 2)
    );

    _coinsLabel.VerifyAll();
  }

  [Fact]
  public void NumCoinsAtStartChanged()
  {
    _ui.OnResolved();

    var numCoinsCollected = new AutoValue<int>(1);
    _gameRepo.Setup(repo => repo.NumCoinsCollected).Returns(numCoinsCollected);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogicState.Output.NumCoinsChanged(1, 2)
    );

    _coinsLabel.VerifyAll();
  }
}
