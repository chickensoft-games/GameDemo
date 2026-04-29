namespace GameDemo.Tests;

using System.Collections.Generic;
using System.Linq;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class InGameTest : TestClass
{
  private StateTester _tester = default!;
  private AppLogic.BaseState.InGame _state = default!;
  private IAppRepo _appRepo = default!;
  private AppLogic.Data _data = default!;

  public InGameTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new AppRepo();
    _data = new();
    _tester = _state.Test();
    _tester.Set(_appRepo);
    _tester.Set(_data);
    AppLogic.SetupSubscriptions(_appRepo, () => _state);
  }

  [Cleanup]
  public void Cleanup() => _appRepo.Dispose();

  [Test]
  public void OnEnter()
  {
    var values = new List<int>();

    using var binding = _appRepo.AutoChannel.Bind()
      .On((in IAppRepo.GameEntered _) => values.Add(1));

    _state.Enter();

    values.ShouldBe([1]);
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
    _appRepo.OnExitGame(PostGameAction.RestartGame);

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
