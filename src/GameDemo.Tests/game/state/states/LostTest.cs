namespace GameDemo.Tests;

using System.Linq;
using Godot;
using Shouldly;

public class LostTest(GodotHeadlessFixture godot)
{
  private StateTester _context = default!;
  private GameLogicState.Lost _state = default!;

  public LostTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogicState.Lost();
    _context = _state.Test();
  }

  [Fact]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogicState.Output.ShowLostScreen>();
  }

  [Fact]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogicState.Input.Start());
    result.ShouldBe(typeof(GameLogicState.RestartingGame));
  }

  [Fact]
  public void OnGoToMainMenu()
  {
    var result = _state.On(new GameLogicState.Input.GoToMainMenu());
    result.ShouldBe(typeof(GameLogicState.Quit));
  }
}
