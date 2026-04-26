namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class LostTest : TestClass
{
  private StateTester _context = default!;
  private GameLogic.BaseState.Lost _state = default!;

  public LostTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new GameLogic.BaseState.Lost();
    _context = _state.Test();
  }

  [Test]
  public void OnEnter()
  {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogic.Output.ShowLostScreen>();
  }

  [Test]
  public void OnStartGame()
  {
    var result = _state.On(new GameLogic.Input.Start());
    result.ShouldBe(typeof(GameLogic.BaseState.RestartingGame));
  }

  [Test]
  public void OnGoToMainMenu()
  {
    var result = _state.On(new GameLogic.Input.GoToMainMenu());
    result.ShouldBe(typeof(GameLogic.BaseState.Quit));
  }
}
