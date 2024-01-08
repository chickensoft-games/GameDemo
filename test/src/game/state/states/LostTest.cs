namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Shouldly;

public class LostTest : TestClass {
  private IFakeContext _context = default!;
  private GameLogic.State.Lost _state = default!;


  public LostTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup() {
    _state = new GameLogic.State.Lost();
    _context = _state.CreateFakeContext();
  }

  [Test]
  public void OnEnter() {
    _state.Enter();
    _context.Outputs.First().ShouldBeOfType<GameLogic.Output.ShowLostScreen>();
  }

  [Test]
  public void OnStartGame() {
    var result = _state.On(new GameLogic.Input.Start());
    result.ShouldBeOfType<GameLogic.State.RestartingGame>();
  }

  [Test]
  public void OnGoToMainMenu() {
    var result = _state.On(new GameLogic.Input.GoToMainMenu());
    result.ShouldBeOfType<GameLogic.State.Quit>();
  }
}
