namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
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
public class MenuBackdropTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.MenuBackdrop _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public MenuBackdropTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new ();
    _gameRepo = new ();

    _state = new GameLogicState.MenuBackdrop();

    _context = _state.Test();

    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(new Mock<IAutoValue<bool>>().Object);
    _gameRepo.Setup(repo => repo.IsPaused).Returns(new Mock<IAutoValue<bool>>().Object);
  }

  [Fact]
  public void OnEnter()
  {
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetIsMouseCaptured(false));

    _state.Enter();

    _gameRepo.VerifyAll();
  }

  [Fact]
  public void OnGameEntered()
  {
    _state.OnGameEntered();

    _context.Inputs.ShouldBe([new GameLogicState.Input.Start()]);
  }

  [Fact]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogicState.Input.Start());
    result.ShouldBe(typeof(GameLogicState.Playing));
  }

  [Fact]
  public void OnInitialize()
  {
    var numCoins = 10;
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetNumCoinsAtStart(numCoins));

    var result = _state.On(new GameLogicState.Input.Initialize(numCoins));

    result.ShouldBe(_state.GetType());
    _gameRepo.VerifyAll();
  }
}
