namespace GameDemo.Tests;

using System.Linq;
using Godot;
using Moq;
using Shouldly;

public class InGameTest : TestClass
{
  private StateTester _tester = default!;
  private AppLogicState.InGame _state = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.Data _data = default!;

  public InGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new ();
    _data = new();
    _tester = _state.Test();
    _tester.Set(_appRepo.Object);
    _tester.Set(_data);
  }

  [Test]
  public void OnEnter()
  {
    _appRepo.Setup(repo => repo.OnEnteringGame());

    _state.Enter();

    _appRepo.VerifyAll();
    _tester.Outputs[0].ShouldBeOfType<AppLogicState.Output.ShowGame>();
  }

  [Test]
  public void OnExit()
  {
    _state.Exit();

    _tester.Outputs[0].ShouldBeOfType<AppLogicState.Output.HideGame>();
  }

  [Test]
  public void OnRestartGameRequested()
  {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogicState.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnGameExited()
  {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogicState.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnEndGame()
  {
    _tester.Set(new AppLogicState.LeavingGame());

    var result =
      _state.On(new AppLogicState.Input.EndGame(PostGameAction.RestartGame));

    result.IsAssignableTo(typeof(AppLogicState.LeavingGame)).ShouldBeTrue();
  }
}
