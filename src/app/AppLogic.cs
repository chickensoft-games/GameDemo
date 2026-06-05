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
  public AppLogic()
  {
    Preallocate<AppLogicState>();
  }

  public override IEnumerable<IDisposable> OnStartSubscriptions()
  {
    yield return Get<IAppRepo>().AutoChannel.Bind()
      .On((in IAppRepo.GameExited message) => (State as AppLogicState.InGame)?.OnGameExited(message.Action))
      .On((in IAppRepo.SplashScreenSkipped _) => (State as AppLogicState.SplashScreen)?.OnSplashScreenSkipped());
  }
}
