namespace GameDemo.Tests;

using System.Diagnostics.CodeAnalysis;
using Chickensoft.GoDotTest;
using Godot;
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
  private IAppRepo _appRepo = default!;
  private IGameRepo _gameRepo = default!;

  public MenuBackdropTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new AppRepo();
    _gameRepo = new GameRepo();
    _state = new GameLogic.BaseState.MenuBackdrop();
    _context = _state.Test();
    _context.Set(_appRepo);
    _context.Set(_gameRepo);
    GameLogic.SetupSubscriptions(_appRepo, () => _state);
    GameLogic.SetupSubscriptions(_gameRepo, () => _state);
  }

  [Cleanup]
  public void Cleanup()
  {
    _appRepo.Dispose();
    _gameRepo.Dispose();
  }

  [Test]
  public void OnEnter()
  {
    _gameRepo.SetIsMouseCaptured(true);

    _state.Enter();

    _gameRepo.IsMouseCaptured.Value.ShouldBe(false);
  }

  [Test]
  public void OnGameEntered()
  {
    _appRepo.OnEnterGame();

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

    var result = _state.On(new GameLogic.Input.Initialize(numCoins));

    result.ShouldBe(_state.GetType());
    _gameRepo.NumCoinsAtStart.Value.ShouldBe(10);
  }
}
