namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Idle : State, IGet<Input.Hit> {
      public IState On(in Input.Hit input) {
        return new Loading(input.Target);
      }
    }
  }
}
