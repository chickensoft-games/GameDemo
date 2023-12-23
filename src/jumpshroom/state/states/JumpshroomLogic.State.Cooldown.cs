namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic : LogicBlock<JumpshroomLogic.IState> {
  public partial record State {
    public record Cooldown : State, IGet<Input.CooldownCompleted> {
      public Cooldown() {
        OnEnter<Cooldown>(
          (previous) => Context.Output(new Output.StartCooldownTimer())
        );
      }

      public IState On(Input.CooldownCompleted input) => new Idle();
    }
  }
}
