namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class LostTest : TestClass
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

  [Test]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs[0].ShouldBeOfType<GameLogicState.Output.ShowLostScreen>();
  }

  [Test]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogicState.Input.Start());
    result.ShouldBe(typeof(GameLogicState.RestartingGame));
  }

  [Test]
  public void OnGoToMainMenu()
  {
    var result = _state.On(new GameLogicState.Input.GoToMainMenu());
    result.ShouldBe(typeof(GameLogicState.Quit));
  }
}
