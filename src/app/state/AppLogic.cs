namespace GameDemo;

using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Generator;

public interface IAppLogic : ILogicBlock<AppLogic.IState>;

[StateDiagram(typeof(State))]
public partial class AppLogic : LogicBlock<AppLogic.IState>, IAppLogic {
  public override IState GetInitialState() => new State.SplashScreen();

  public AppLogic(IAppRepo appRepo) {
    Set(appRepo);
  }
}
