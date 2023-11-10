namespace GameDemo.Tests;

using Chickensoft.GoDotCollections;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameUITest : TestClass {
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<ILabel> _coinsLabel = default!;
  private Mock<IInGameUILogic> _logic = default!;
  private InGameUILogic.IFakeBinding _binding = default!;
  private InGameUI _ui = default!;

  public InGameUITest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _appRepo = new();
    _coinsLabel = new();
    _logic = new();

    _binding = InGameUILogic.CreateFakeBinding();

    _logic.Setup(logic => logic.Bind()).Returns(_binding);
    _logic.Setup(logic => logic.Start());

    _ui = new() {
      CoinsLabel = _coinsLabel.Object,
      InGameUILogic = _logic.Object
    };

    _ui.FakeDependency(_appRepo.Object);
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
    _appRepo.Setup(repo => repo.NumCoinsAtStart).Returns(numCoinsAtStart);

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
    _appRepo.Setup(repo => repo.NumCoinsCollected).Returns(numCoinsCollected);

    _coinsLabel.SetupSet(label => label.Text = "1/2");

    _binding.Output(
      new InGameUILogic.Output.NumCoinsAtStartChanged(2)
    );

    _coinsLabel.VerifyAll();
  }
}
