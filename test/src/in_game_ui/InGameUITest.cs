namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.AutoInject;
using Chickensoft.Collections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

[
  SuppressMessage(
    "Design",
    "CA1001",
    Justification = "Disposable field is Godot object; Godot will dispose"
  )
]
public class InGameUITest : TestClass
{
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;
  private Mock<ILabel> _coinsLabel = default!;
  private Mock<IInGameUILogic> _logic = default!;
  private InGameUILogic.IFakeBinding _binding = default!;

  private InGameUI _ui = default!;

  public InGameUITest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _gameRepo = new();
    _coinsLabel = new();
    _logic = new();

    _binding = InGameUILogic.CreateFakeBinding();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
    _logic.Setup(logic => logic.Start());

    _ui = new()
    {
      CoinsLabel = _coinsLabel.Object,
      InGameUILogic = _logic.Object
    };

    _ui.FakeDependency(_appRepo.Object);
    _ui.FakeDependency(_gameRepo.Object);

    _ui._Notification(-1);
  }

  [Test]
  public void Initializes()
  {
    _ui.Setup();

    _ui.InGameUILogic.ShouldBeOfType<InGameUILogic>();
  }

  [Test]
  public void OnExitTree()
  {
    _logic.Reset();
    _logic.Setup(logic => logic.Stop());
    _ui.InGameUIBinding = _binding;

    _ui.OnExitTree();

    _logic.VerifyAll();
  }

  [Test]
  public void NumCoinsCollectedChanged()
  {
    _ui.OnResolved();

    var numCoinsAtStart = new AutoProp<int>(2);
    _gameRepo.Setup(repo => repo.NumCoinsAtStart).Returns(numCoinsAtStart);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogic.Output.NumCoinsChanged(1, 2)
    );

    _coinsLabel.VerifyAll();
  }

  [Test]
  public void NumCoinsAtStartChanged()
  {
    _ui.OnResolved();

    var numCoinsCollected = new AutoProp<int>(1);
    _gameRepo.Setup(repo => repo.NumCoinsCollected).Returns(numCoinsCollected);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogic.Output.NumCoinsChanged(1, 2)
    );

    _coinsLabel.VerifyAll();
  }
}
