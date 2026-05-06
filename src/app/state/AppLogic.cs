namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;

public interface IAppLogic : ILogicBlock;

[Meta]
public partial class AppLogic : AutoBlock, IAppLogic
{
  public override Type GetInitialState() => typeof(BaseState.SplashScreen);
  public AppLogic()
  {
    Preallocate<BaseState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameExited message) => (State as BaseState.InGame)?.OnGameExited(message.Action))
      .On((in IAppRepo.SplashScreenSkipped _) => (State as BaseState.SplashScreen)?.OnSplashScreenSkipped());
  }
}
