namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Idle(IContext Context) : State(Context),
    IGet<Input.Hit> {
      public IState On(Input.Hit input) {
        return new Loading(Context, input.Target);
      }
    }
  }
}
