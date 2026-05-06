namespace GameDemo.Tests;

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
    Justification = "Disposable fields are disposed in cleanup"
  )
]
public class MenuBackdropTest : TestClass
{
  private StateTester _context = default!;
  private GameLogic.BaseState.MenuBackdrop _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public MenuBackdropTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new ();
    _gameRepo = new ();

    _state = new GameLogic.BaseState.MenuBackdrop();

    _context = _state.Test();

    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(new Mock<IAutoValue<bool>>().Object);
    _gameRepo.Setup(repo => repo.IsPaused).Returns(new Mock<IAutoValue<bool>>().Object);
  }

  [Test]
  public void OnEnter()
  {
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetIsMouseCaptured(false));

    _state.Enter();

    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnGameEntered()
  {
    _state.OnGameEntered();

    _context.Inputs.ShouldBe([new GameLogic.Input.Start()]);
  }

  [Test]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogic.Input.Start());
    result.ShouldBe(typeof(GameLogic.BaseState.Playing));
  }

  [Test]
  public void OnInitialize()
  {
    var numCoins = 10;
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetNumCoinsAtStart(numCoins));

    var result = _state.On(new GameLogic.Input.Initialize(numCoins));

    result.ShouldBe(_state.GetType());
    _gameRepo.VerifyAll();
  }
}
