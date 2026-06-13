namespace GameDemo.Tests;

using System.Linq;
using Moq;
using Shouldly;

public class InGameTest
{
  private readonly StateTester _tester;
  private readonly AppLogicState.InGame _state = new();
  private readonly Mock<IAppRepo> _appRepo = new();
  private readonly AppLogic.Data _data = new();

  public InGameTest()
  {
    _tester = _state.Test();
    _tester.Set(_appRepo.Object);
    _tester.Set(_data);
  }

  [Fact]
  public void OnEnter()
  {
    _appRepo.Setup(repo => repo.OnEnteringGame());

    _state.Enter();

    _appRepo.VerifyAll();
    _tester.Outputs[0].ShouldBeOfType<AppLogicState.Output.ShowGame>();
  }

  [Fact]
  public void OnExit()
  {
    _state.Exit();

    _tester.Outputs[0].ShouldBeOfType<AppLogicState.Output.HideGame>();
  }

  [Fact]
  public void OnRestartGameRequested()
  {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogicState.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Fact]
  public void OnGameExited()
  {
    _state.OnGameExited(PostGameAction.RestartGame);

    var input = _tester.Inputs.Single()
      .ShouldBeOfType<AppLogicState.Input.EndGame>();

    input.PostGameAction.ShouldBe(PostGameAction.RestartGame);
  }

  [Fact]
  public void OnEndGame()
  {
    _tester.Set(new AppLogicState.LeavingGame());

    var result =
      _state.On(new AppLogicState.Input.EndGame(PostGameAction.RestartGame));

    result.IsAssignableTo(typeof(AppLogicState.LeavingGame)).ShouldBeTrue();
  }
}
