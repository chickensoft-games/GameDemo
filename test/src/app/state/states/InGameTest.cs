namespace GameDemo.Tests;

using System.Linq;
using Chickensoft.GoDotTest;
using Godot;
using Moq;
using Shouldly;

public class InGameTest : TestClass
{
  private StateTester _tester = default!;
  private AppLogic.BaseState.InGame _state = default!;
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
    _appRepo.Setup(repo => repo.OnEnterGame());

    _state.Enter();

    _appRepo.VerifyAll();
    _tester.Outputs[0].ShouldBeOfType<AppLogic.Output.ShowGame>();
  }

  [Test]
  public void OnExit()
  {
    _state.Exit();

    _tester.Outputs[0].ShouldBeOfType<AppLogic.Output.HideGame>();
  }

  [Test]
  public void OnRestartGameRequested()
  {
    _state.OnRestartGameRequested();

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogic.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnGameExited()
  {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogic.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Test]
  public void OnEndGame()
  {
    _tester.Set(new AppLogic.BaseState.LeavingGame());

    var result =
      _state.On(new AppLogic.Input.EndGame(PostGameAction.RestartGame));

    result.IsAssignableTo(typeof(AppLogic.BaseState.LeavingGame)).ShouldBeTrue();
  }
}
