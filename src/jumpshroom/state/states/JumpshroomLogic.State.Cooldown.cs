namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Cooldown : State, IGet<Input.CooldownCompleted> {
      public Cooldown() {
        this.OnEnter(() => Output(new Output.StartCooldownTimer()));
      }

      public IState On(in Input.CooldownCompleted input) => new Idle();
    }
  }
}
