namespace GameDemo.Tests;

using System.Linq;
using Godot;
using Moq;
using Shouldly;

public class WonTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.Won _state = default!;
  private Mock<IAppRepo> _appRepo = default!;

  public WonTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogicState.Won();
    _context = _state.Test();

    _appRepo = new Mock<IAppRepo>();
    _context.Set(_appRepo.Object);
  }

  [Fact]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogicState.Output.ShowWonScreen>();
  }

  [Fact]
  public void OnGoToMainMenu()
  {
    _appRepo.Setup(repo => repo.OnExitGame(PostGameAction.GoToMainMenu));

    var result = _state.On(new GameLogicState.Input.GoToMainMenu());

    _appRepo.VerifyAll();
    result.ShouldBe(_state.GetType());
  }
}
