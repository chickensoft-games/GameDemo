namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class JumpshroomLogic {
  public partial record State {
    [Meta]
    public partial record Cooldown : State, IGet<Input.CooldownCompleted> {
      public Cooldown() {
        this.OnEnter(() => Output(new Output.StartCooldownTimer()));
      }

      public Transition On(in Input.CooldownCompleted input) => To<Idle>();
    }
  }
}
