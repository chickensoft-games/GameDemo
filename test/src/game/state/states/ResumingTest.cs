namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class ResumingTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Resuming _state = default!;
  private Mock<IGameRepo> _gameRepo = default!;

  public ResumingTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Resuming();
    _context = _state.CreateFakeContext();

    _gameRepo = new();
    _context.Set(_gameRepo.Object);
  }

  [Test]
  public void OnEnter() {
    _gameRepo.Setup(repo => repo.Resume());
    _state.Enter();
    _gameRepo.VerifyAll();
  }

  [Test]
  public void OnExit() {
    _state.Exit();
    _context.Outputs.Single().ShouldBeOfType<GameLogic.Output.HidePauseMenu>();
  }

  [Test]
  public void OnPauseMenuTransitioned() {
    var result = _state.On(new GameLogic.Input.PauseMenuTransitioned());
    result.ShouldBeOfType<GameLogic.State.Playing>();
  }
}
