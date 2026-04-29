namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface IAppLogic : ILogicBlock;

[Meta]
public partial class AppLogic : LogicBlock, IAppLogic
{
  public override Type GetInitialState() => typeof(BaseState.SplashScreen);
  public AppLogic()
  {
    Set(new BaseState.InGame());
    Set(new BaseState.LeavingGame());
    Set(new BaseState.LeavingMenu());
    Set(new BaseState.LoadingSaveFile());
    Set(new BaseState.MainMenu());
    Set(new BaseState.SplashScreen());
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return SetupSubscriptions(Get<IAppRepo>(), () => State);
  }

  public static IDisposable SetupSubscriptions(IAppRepo repo, Func<LogicBlockState?> stateFunc) =>
    repo.AutoChannel.Bind()
      .On((in IAppRepo.GameExited message) => stateFunc()?.Input(new Input.EndGame(message.Action)))
      .On((in IAppRepo.SplashScreenSkipped _) =>
      {
        if (stateFunc() is BaseState.SplashScreen state)
        {
          state.Output(new Output.HideSplashScreen());
        }
      });
}
