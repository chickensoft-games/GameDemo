namespace GameDemo.Tests;

using System.Linq;
using Shouldly;

public class LostTest
{
  private readonly StateTester _context;
  private readonly GameLogicState.Lost _state = new();

  public LostTest()
  {
    _context = _state.Test();
  }

  [Fact]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs[0].ShouldBeOfType<GameLogicState.Output.ShowLostScreen>();
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
