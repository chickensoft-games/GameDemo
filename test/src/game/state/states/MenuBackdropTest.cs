namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Godot;
using Moq;
using Shouldly;

public class MenuBackdropTest : TestClass
{
  private IFakeContext _context = default!;
  private GameLogic.State.MenuBackdrop _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public MenuBackdropTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _appRepo = new();
    _gameRepo = new();

    _state = new GameLogic.State.MenuBackdrop();

    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_gameRepo.Object);

    _gameRepo.Setup(repo => repo.IsMouseCaptured).Returns(new Mock<IAutoValue<bool>>().Object);
    _gameRepo.Setup(repo => repo.IsPaused).Returns(new Mock<IAutoValue<bool>>().Object);
  }

  [Test]
  public void Subscribes()
  {
    _state.Attach(_context);

    _appRepo.VerifyAdd(x => x.GameEntered += _state.OnGameEntered);

    _state.Detach();

    _appRepo.VerifyRemove(x => x.GameEntered -= _state.OnGameEntered);
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
    _state.Attach(_context);

    _state.OnGameEntered();

    _context.Inputs.First().ShouldBeOfType<GameLogic.Input.Start>();
  }

  [Test]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogic.Input.Start());
    result.State.ShouldBeOfType<GameLogic.State.Playing>();
  }

  [Test]
  public void OnInitialize()
  {
    var numCoins = 10;
    _gameRepo.Reset();
    _gameRepo.Setup(repo => repo.SetNumCoinsAtStart(numCoins));

    var result = _state.On(new GameLogic.Input.Initialize(numCoins));

    result.State.ShouldBe(_state);
    _gameRepo.VerifyAll();
  }
}
