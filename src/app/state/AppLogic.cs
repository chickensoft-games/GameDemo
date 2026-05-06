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
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameExited message) => (State as BaseState.InGame)?.OnGameExited(message.Action))
      .On((in IAppRepo.SplashScreenSkipped _) => (State as BaseState.SplashScreen)?.OnSplashScreenSkipped());
  }
}
